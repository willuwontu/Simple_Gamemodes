using Photon.Pun;
using RWF.GameModes;
using RWF.UI;
using Simple_Gamemodes.Extentions;
using Simple_Gamemodes.Monos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnboundLib;
using UnboundLib.GameModes;
using UnboundLib.Networking;
using UnityEngine;

namespace Simple_Gamemodes.Gamemodes
{
    public class GM_Stock_Battle : RWFGameMode
    {

        internal static GM_Stock_Battle instance;
        private List<int> awaitingRespawn = new List<int>() { };
        internal Dictionary<int, int> deathsThisBattle = new Dictionary<int, int>() { };

        private const float RespawnDelay = 1f;
        private float TimeLeftInRound = 0f;

        bool inRound = false;

        private GameObject _timer;

        public GameObject Timer
        {
            get
            {
                if (!_timer)
                {
                    var _uiGo = GameObject.Find("/Game/UI");
                    var _gameGo = _uiGo.transform.Find("UI_Game").Find("Canvas").gameObject;

                    _timer = new GameObject("Timed Deathmatch Timer", typeof(RectTransform), typeof(TextMeshProUGUI));
                    _timer.transform.SetParent(_gameGo.transform);

                    var rect = _timer.GetComponent<RectTransform>();
                    rect.localScale = Vector3.one;
                    rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 1);
                    rect.offsetMax = new Vector2(100, -25);
                    rect.sizeDelta = new Vector2(200, 50);

                    var text = _timer.GetComponent<TextMeshProUGUI>();
                    text.text = "Timer";
                    text.alignment = TextAlignmentOptions.Center;
                    text.fontSize = 50f;

                    var fitter = _timer.AddComponent<UnityEngine.UI.ContentSizeFitter>();
                    fitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
                    fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
                }

                return _timer;
            }
        }

        protected override void Awake()
        {
            GM_Stock_Battle.instance = this;
            base.Awake();
        }

        public void Start()
        {
            this.StartCoroutine(this.Init());
        }
        public void Update()
        {
            if (Main.StockBattle_Timed.Value != 0)
            {
                if (TimeLeftInRound <= 0)
                {
                    if (inRound)
                    {
                        Timer.GetOrAddComponent<TextMeshProUGUI>().text = "";
                        if (GameModeManager.CurrentHandler.AllowTeams)
                        {
                            Dictionary<int, int> playersAlive = new Dictionary<int, int>();
                            foreach (Player player in PlayerManager.instance.players)
                            {
                                if (!playersAlive.ContainsKey(player.teamID)) { playersAlive[player.teamID] = 0; }
                                if(Main.StockBattle_Lives.Value - deathsThisBattle[player.playerID] > 0)
                                    playersAlive[player.teamID]++;
                            }
                            int mostAlive = playersAlive.Values.Max();
                            if(playersAlive.Values.Where((v) => v == mostAlive).Count() == 1)
                            {
                                if (PhotonNetwork.IsMasterClient)
                                    NetworkingManager.RPC(typeof(RWFGameMode), "RPCA_NextRound", new object[3]
                                    {
                                     playersAlive.Keys.Where((v) => playersAlive[v] == mostAlive).ToArray() ,
                                        teamPoints,
                                        teamRounds
                                    });
                            }
                            else
                            {
                                Dictionary<int, int> teamLives = new Dictionary<int, int>();
                                foreach (Player player in PlayerManager.instance.players)
                                {
                                    if (teamLives[player.teamID] != mostAlive) continue;
                                    if (!teamLives.ContainsKey(player.teamID)) { teamLives[player.teamID] = 0; }
                                    teamLives[player.teamID] += Main.StockBattle_Lives.Value - deathsThisBattle[player.playerID];
                                }
                                int mostLives = teamLives.Values.Max();
                                if (PhotonNetwork.IsMasterClient)
                                    NetworkingManager.RPC(typeof(RWFGameMode), "RPCA_NextRound", new object[3]
                                    {
                                     teamLives.Keys.Where((v) => teamLives[v] == mostLives).ToArray() ,
                                        teamPoints,
                                        teamRounds
                                    });
                            }
                        }
                        else
                        {
                            int mostLives = 0;

                            foreach (Player player in PlayerManager.instance.players)
                            {
                                int lives = Main.StockBattle_Lives.Value - deathsThisBattle[player.playerID];
                                if (lives > mostLives) mostLives = lives;
                            }
                            
                            if (PhotonNetwork.IsMasterClient)
                                NetworkingManager.RPC(typeof(RWFGameMode), "RPCA_NextRound", new object[3]
                                {
                                ToTeams(PlayerManager.instance.players.Where((player) => Main.StockBattle_Lives.Value - deathsThisBattle[player.playerID] == mostLives)).ToArray(),
                            teamPoints,
                            teamRounds
                                }); 

                        }
                    }
                    inRound = false;
                }
                else
                {
                    FormatTimer();
                    inRound = true;
                }
                TimeLeftInRound -= Time.deltaTime;
            }
        }


        private static IEnumerable<int> ToTeams(IEnumerable<Player> source)
        {
            foreach( Player player in source)
            {
                yield return player.teamID;
            }
        }

        private void FormatTimer()
        {
                int m = UnityEngine.Mathf.FloorToInt(TimeLeftInRound / 60f);
                int s = UnityEngine.Mathf.FloorToInt(TimeLeftInRound - (m * 60f));
                int ms = UnityEngine.Mathf.FloorToInt((TimeLeftInRound * 100) - (UnityEngine.Mathf.Floor(TimeLeftInRound) * 100));
                if (m <= 0 && s <= 10)
                {
                    Timer.GetOrAddComponent<TextMeshProUGUI>().text = $"{s}.{(ms < 10 ? "0" : "")}{ms}";
                }
                else
                {
                    Timer.GetOrAddComponent<TextMeshProUGUI>().text = $"{m}:{(s < 10 ? "0" : "")}{s}";
                }
        }

        public override void PlayerDied(Player killedPlayer, int teamsAlive)
        {
            if (this.awaitingRespawn.Contains(killedPlayer.playerID))
            {
                return;
            }

            instance.awaitingRespawn.Add(killedPlayer.playerID);
            if (PhotonNetwork.IsMasterClient)
            {               
                NetworkingManager.RPC(typeof(GM_Stock_Battle), nameof(RPC_DoRespawn), new object[] { killedPlayer.playerID, RespawnDelay, this.GetSpawn(killedPlayer.teamID) });
            }
           
        }

        public void CheckForWinner()
        {
            if (!PhotonNetwork.IsMasterClient) return;
            int winning_team = -1;
            foreach (Player player in PlayerManager.instance.players)
            {
                if (Main.StockBattle_Lives.Value - instance.deathsThisBattle[player.playerID] > 0)
                {
                    if (winning_team != -1 && winning_team != player.teamID) return;
                    winning_team = player.teamID;
                }
            }
            if (winning_team != -1)
            {

                if (Main.StockBattle_Timed.Value != 0)
                {
                    Timer.GetOrAddComponent<TextMeshProUGUI>().text = "";
                    inRound = false;
                    TimeLeftInRound = -1;
                }
                NetworkingManager.RPC(typeof(RWFGameMode), "RPCA_NextRound", new object[3]
                                {
                            new int[] { winning_team },
                            teamPoints,
                            teamRounds
                                });
            }
        }

        public void UpdateScores()
        {
            foreach (Player player in PlayerManager.instance.players)
            {
                int lives = Main.StockBattle_Lives.Value - deathsThisBattle[player.playerID];
                float lives_percent = (lives - 1) / (float)(Main.StockBattle_Lives.Value - 1);
                Color color = new Color(1 - lives_percent, lives_percent, 0,1);
                player.gameObject.GetOrAddComponent<Timed_Kills>().UpdateScore($"{lives}", color);
            }
        }


        [UnboundRPC]
        public static void RPC_DoRespawn(int playerID, float delay, Vector3 point)
        {
            instance.deathsThisBattle[playerID]++;
            UnityEngine.Debug.Log(instance.deathsThisBattle[playerID]);
            instance.UpdateScores();
            Player killedPlayer = PlayerManager.instance.players.Find(p => p.playerID == playerID);
            if (Main.StockBattle_Lives.Value - instance.deathsThisBattle[playerID] > 0)
                instance.StartCoroutine(instance.IRespawnPlayer(killedPlayer, delay, point));
            else
            {

                instance.awaitingRespawn.Remove(playerID);
                instance.CheckForWinner();
            }
        }

        public IEnumerator IRespawnPlayer(Player player, float delay, Vector3 point)
        {
            yield return new WaitForSecondsRealtime(delay);
            if (this.awaitingRespawn.Contains(player.playerID))
            {

                var _ = PlayerSpotlight.Cam;
                _ = PlayerSpotlight.Group;

                if (player.data.view.IsMine || PhotonNetwork.OfflineMode)
                {
                    PlayerSpotlight.FadeIn(0.1f);
                }

                player.transform.position = point; 
                player.data.playerVel.SetFieldValue("simulated", false);
                yield return new WaitForSecondsRealtime(2f);
                player.data.healthHandler.Revive(true);
                if (player.data.view.IsMine || PhotonNetwork.OfflineMode)
                {
                    PlayerSpotlight.FadeOut();
                }
                player.data.stats.GetAditionalData().invanerable = true;
                player.data.playerVel.SetFieldValue("simulated", true);
                player.GetComponent<GeneralInput>().enabled = true; 
                this.awaitingRespawn.Remove(player.playerID);
                yield return new WaitForSecondsRealtime(0.5f);
                player.data.stats.GetAditionalData().invanerable = false;

            }
        }

        private Vector3 GetSpawn(int teamID)
        {
            Vector3[] spawns = MapManager.instance.GetSpawnPoints().Select(s => s.localStartPos).ToArray();
            spawns.Shuffle();
            return spawns[0];
        }

        public override void PlayerJoined(Player player)
        {
            this.deathsThisBattle[player.playerID] = 0;
            player.gameObject.GetOrAddComponent<Timed_Kills>();
            base.PlayerJoined(player);
        }
        public override IEnumerator DoPointStart()
        {
            this.deathsThisBattle = new Dictionary<int, int>() { };
            foreach (Player player in PlayerManager.instance.players)
            {
                this.deathsThisBattle[player.playerID] = 0;
            }
            yield return base.DoPointStart();
            UpdateScores();
            resetRoundTimer();
        }


        public override IEnumerator DoRoundStart()
        {
            this.deathsThisBattle = new Dictionary<int, int>() { };
            foreach (Player player in PlayerManager.instance.players)
            {
                this.deathsThisBattle[player.playerID] = 0;
            }
            yield return base.DoRoundStart();
            UpdateScores();
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
            Destroy(Timer);
            base.OnDisable();
        }

        private void resetRoundTimer()
        {
            TimeLeftInRound = Main.StockBattle_Timed.Value;
        }
    }
}
