using Photon.Pun;
using RWF.GameModes;
using Simple_Gamemodes.Cards.TitanFall;
using Simple_Gamemodes.Extentions;
using Simple_Gamemodes.Monos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnboundLib;
using UnboundLib.GameModes;
using UnboundLib.Networking;

namespace Simple_Gamemodes.Gamemodes
{
    public class GM_TitanFall : RWFGameMode
    {
        public Player titan = null;
        public static GM_TitanFall instance;
        public List<Player> titanQueue = new List<Player>();

        protected override void Awake()
        {
            GM_TitanFall.instance = this;
            base.Awake();
        }
        public void Update()
        {
            if(titan != null)
            {
                titan.data.stats.GetAditionalData().damageCapFilled = UnityEngine.Mathf.Clamp(titan.data.stats.GetAditionalData().damageCapFilled - (TimeHandler.deltaTime * titan.data.stats.GetAditionalData().damageCap * titan.data.maxHealth), 0, int.MaxValue);
            }
        }

        public override IEnumerator DoRoundStart()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                List<Player> players = PlayerManager.instance.players.ToList();
                if (titanQueue.Count == 0)
                {
                    do
                    {
                        players.Shuffle();
                    } while (players.First() == titan);
                    titanQueue = players;
                }
                titan = titanQueue.First();
                titanQueue.RemoveAt(0);
                NetworkingManager.RPC(typeof(GM_TitanFall), nameof(Sync), titan.playerID);
            }
            return base.DoRoundStart();
        }
         
        [UnboundRPC]
        public static void Sync(int playerID)
        {
            instance.titan = PlayerManager.instance.players.Where(p => p.playerID == playerID).First();
            List<Player> players = PlayerManager.instance.players.ToList();

            instance.titan.gameObject.AddComponent<TITAN>();
            for (int i = 0; i < players.Count; i++)
            {
                if (players.ElementAt(i) != instance.titan)
                    players.ElementAt(i).gameObject.AddComponent<Fighter>(); 
            }
        }

        public override void PlayerDied(Player killedPlayer, int teamsAlive)
        {
            if(killedPlayer == titan)
            {
                TimeHandler.instance.DoSlowDown();
                if (PhotonNetwork.IsMasterClient)
                {
                    List<int> teams = new List<int>();
                    foreach(Player player in PlayerManager.instance.players)
                    {
                        if(player != titan && !player.data.dead) teams.Add(player.teamID);
                    }
                    NetworkingManager.RPC(typeof(RWFGameMode), "RPCA_NextRound", 
                        teams.ToArray(), teamPoints, teamRounds);
                }
            }
            else if (teamsAlive == 1)
            {
                TimeHandler.instance.DoSlowDown();
                if (PhotonNetwork.IsMasterClient)
                {
                    int id = titan.teamID;
                    teamPoints[id] += 3;
                    NetworkingManager.RPC(typeof(RWFGameMode), "RPCA_NextRound", new int[1]
                    {
                        id
                    }, teamPoints, teamRounds);
                }
            }
        }

        public override void RoundOver(int[] winningTeamIDs)
        {
            int id = titan.teamID;

            int points_to_win = (int)GameModeManager.CurrentHandler.Settings["pointsToWinRound"];
            for(int i = 0; i < teamPoints.Count; i++)
            {
                if(teamPoints[i] >= points_to_win) teamPoints[i] -= points_to_win;
            }
            previousRoundWinners = winningTeamIDs.ToArray();
            StartCoroutine(RoundTransition(winningTeamIDs));
        }
    }
}
