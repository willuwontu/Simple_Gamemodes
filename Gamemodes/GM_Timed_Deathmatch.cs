using RWF.GameModes;
using Simple_Gamemodes.Monos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace Simple_Gamemodes.Gamemodes
{
    public class GM_Timed_Deathmatch : RWFGameMode
    {

        internal static GM_Timed_Deathmatch instance;
        private List<int> awaitingRespawn = new List<int>() { };
        private Dictionary<int, int> deathsThisBattle = new Dictionary<int, int>() { };
        private Dictionary<int, int> lastPlayerDamage = new Dictionary<int, int>() { };
        private Dictionary<int, int> KillsThisBattle = new Dictionary<int, int>() { };

        private const float delayPenaltyPerDeath = 0f;
        private const float baseRespawnDelay = 1f;
        private float TimeLeftInRound = 0f;

        bool inRound = false;
        private GameObject timer;



        protected override void Awake()
        {
            GM_Timed_Deathmatch.instance = this;
            base.Awake();
        }

        public void Start()
        {
            timer = new GameObject("Timer");
            timer.GetOrAddComponent<TextMeshProUGUI>().text = "";
            timer.GetOrAddComponent<TextMeshProUGUI>().fontSize = 2.5f;
            timer.transform.localPosition = Vector3.up * 17;
            timer.GetOrAddComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            timer.GetOrAddComponent<Canvas>().sortingLayerName = "MostFront";
        }
        public void Update()
        {
            if (TimeLeftInRound <= 0)
            {
                if (inRound)
                {
                    timer.GetOrAddComponent<TextMeshProUGUI>().text = "";
                    Dictionary<int, int> teamKills = new Dictionary<int, int>() { };
                    foreach (Player player in PlayerManager.instance.players)
                    {
                        if (!teamKills.ContainsKey(player.teamID)) { teamKills[player.teamID] = 0; }
                        teamKills[player.teamID] += KillsThisBattle[player.playerID];
                    }
                    int minKills = teamKills[teamKills.Keys.OrderBy(t => teamKills[t]).First()];
                    int maxKills = teamKills[teamKills.Keys.OrderBy(t => -teamKills[t]).First()];
                    teamKills.Keys.Where(t => teamKills[t] == maxKills).ToArray();
                    NetworkingManager.RPC(typeof(RWFGameMode), "RPCA_NextRound", new object[3]
                       {
                        maxKills != minKills? teamKills.Keys.Where(t => teamKills[t] == maxKills).ToArray() :  new int[] { },
                        teamPoints,
                        teamRounds
                       });
                }
                inRound = false;
            }
            else
            {
                FormatTimer();
                inRound = true;
                foreach (Player player in PlayerManager.instance.players)
                {
                    if(player.data.lastSourceOfDamage != null)
                    {
                        lastPlayerDamage[player.playerID] = player.data.lastSourceOfDamage.playerID;
                    }
                }
            }
            TimeLeftInRound -= Time.deltaTime;
        }

        private void FormatTimer()
        {
            int m = UnityEngine.Mathf.FloorToInt(TimeLeftInRound / 60f);
            int s = UnityEngine.Mathf.FloorToInt(TimeLeftInRound - (m*60f));
            int ms = UnityEngine.Mathf.FloorToInt((TimeLeftInRound*100) - (UnityEngine.Mathf.Floor(TimeLeftInRound)*100));
            if(m <= 0 && s <= 10)
            {
                timer.GetOrAddComponent<TextMeshProUGUI>().text = $"{s}.{(ms < 10 ? "0" : "")}{ms}";
            }
            else
            {
                timer.GetOrAddComponent<TextMeshProUGUI>().text = $"{m}:{(s<10? "0":"")}{s}";
            }
        }

        public override void PlayerDied(Player killedPlayer, int teamsAlive)
        {
            if (this.awaitingRespawn.Contains(killedPlayer.playerID))
            {
                return;
            }

            if (killedPlayer.data.lastSourceOfDamage != null)
            {
                lastPlayerDamage[killedPlayer.playerID] = killedPlayer.data.lastSourceOfDamage.playerID;
            }

            this.deathsThisBattle[killedPlayer.playerID]++;
            if(lastPlayerDamage[killedPlayer.playerID] == killedPlayer.playerID)
            {
                KillsThisBattle[killedPlayer.playerID]--;
            }
            else
            {
                if(GetPlayerWithID(lastPlayerDamage[killedPlayer.playerID]).teamID == killedPlayer.teamID)
                    KillsThisBattle[lastPlayerDamage[killedPlayer.playerID]]--;
                else
                    KillsThisBattle[lastPlayerDamage[killedPlayer.playerID]]++;
            }
            this.StartCoroutine(UpdateScores());
            this.awaitingRespawn.Add(killedPlayer.playerID);
            this.StartCoroutine(this.IRespawnPlayer(killedPlayer, delayPenaltyPerDeath * (this.deathsThisBattle[killedPlayer.playerID] - 1) + baseRespawnDelay));
        }


        public IEnumerator UpdateScores()
        {

            Dictionary<int, int> teamKills = new Dictionary<int, int>() { };
            foreach (Player player in PlayerManager.instance.players)
            {
                if (!teamKills.ContainsKey(player.teamID)) { teamKills[player.teamID] = 0; }
                teamKills[player.teamID] += KillsThisBattle[player.playerID];
            }
            int maxKills = teamKills[teamKills.Keys.OrderBy(t => -teamKills[t]).First()];
            int minKills = teamKills[teamKills.Keys.OrderBy(t => teamKills[t]).First()];
            foreach (Player player in PlayerManager.instance.players)
            {
                Color color = Color.yellow;
                if (minKills != maxKills)
                {
                    if (teamKills[player.teamID] == minKills)
                        color = Color.red;
                    if (teamKills[player.teamID] == maxKills) 
                        color = Color.green;
                }
                if (GameModeManager.CurrentHandler.AllowTeams)
                    player.GetComponent<Timed_Kills>().UpdateScore($"{KillsThisBattle[player.playerID]} ({teamKills[player.teamID]})", color);
                else
                    player.GetComponent<Timed_Kills>().UpdateScore($"{KillsThisBattle[player.playerID]}", color);
            }
            yield break;
        }

        public IEnumerator IRespawnPlayer(Player player, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            if (this.awaitingRespawn.Contains(player.playerID))
            {
                player.transform.position = this.GetFarthestSpawn(player.teamID);
                player.data.healthHandler.Revive(true);
                player.GetComponent<GeneralInput>().enabled = true;
                lastPlayerDamage[player.playerID] = player.playerID;
                this.awaitingRespawn.Remove(player.playerID);
            }
        }

        private Vector3 GetFarthestSpawn(int teamID)
        {
            Vector3[] spawns = MapManager.instance.GetSpawnPoints().Select(s => s.localStartPos).ToArray();
            float dist = -1f;
            Vector3 best = Vector3.zero;
            foreach (Vector3 spawn in spawns)
            {
                float thisDist = PlayerManager.instance.players.Where(p => !p.data.dead && p.teamID != teamID).Select(p => Vector3.Distance(p.transform.position, spawn)).Sum();
                if (thisDist > dist)
                {
                    dist = thisDist;
                    best = spawn;
                }
            }
            return best;
        }

        public override void PlayerJoined(Player player)
        {
            this.deathsThisBattle[player.playerID] = 0;
            this.KillsThisBattle[player.playerID] = 0;
            this.lastPlayerDamage[player.playerID] = player.playerID;
            player.gameObject.GetOrAddComponent<Timed_Kills>();
            base.PlayerJoined(player);
        }
        public override IEnumerator DoPointStart()
        {
            foreach (Player player in PlayerManager.instance.players)
            {
                this.deathsThisBattle[player.playerID] = 0;
                this.KillsThisBattle[player.playerID] = 0;
                this.lastPlayerDamage[player.playerID] = player.playerID;
            }
            yield return base.DoPointStart();
            this.StartCoroutine(UpdateScores());
            resetRoundTimer();
        }


        public override IEnumerator DoRoundStart()
        {
            foreach (Player player in PlayerManager.instance.players)
            {
                this.deathsThisBattle[player.playerID] = 0;
                this.KillsThisBattle[player.playerID] = 0;
                this.lastPlayerDamage[player.playerID] = player.playerID;
            }
            yield return base.DoRoundStart();
            this.StartCoroutine(UpdateScores());
            resetRoundTimer();
        }


        internal static Player GetPlayerWithID(int playerID)
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                if (PlayerManager.instance.players[i].playerID == playerID)
                {
                    return PlayerManager.instance.players[i];
                }
            }
            return null;
        }

        public override void OnDisable()
        {
            Destroy(timer);
            base.OnDisable();
        }

        private void resetRoundTimer()
        {
            TimeLeftInRound = Main.TimedDeathmatch_Time.Value;
        }
    }
}
