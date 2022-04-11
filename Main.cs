using UnboundLib;
using UnboundLib.Cards;
using HarmonyLib;
using UnboundLib.GameModes;
using BepInEx;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using System.Collections.Generic;
using System.Linq;
using Simple_Gamemodes.Gamemodes;
using Simple_Gamemodes.GamemodeHandlers;
using Photon.Pun;
using BepInEx.Configuration;
using UnboundLib.Networking;
using UnityEngine;
using UnboundLib.Utils.UI;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using Simple_Gamemodes.Cards.TitanFall;

namespace Simple_Gamemodes
{
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("io.olavim.rounds.rwf", BepInDependency.DependencyFlags.HardDependency)]
    //[BepInDependency("com.willuwontu.rounds.itemshops", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class Main : BaseUnityPlugin
    {

        private const string ModId = "Root.Simple.Gamemodes";
        private const string ModName = "Simple_Gamemodes";
        public const string Version = "1.3.2"; // What version are we on (major.minor.patch)?

        public static ConfigEntry<int> TimedDeathmatch_Time;
        public static ConfigEntry<bool> TimedDeathmatch_Inverted;
        public static ConfigEntry<int> StockBattle_Lives;
        public static ConfigEntry<int> StockBattle_Timed;

        void Awake()
        {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();

            TimedDeathmatch_Time = Config.Bind(ModName, "TD_Time", 180, "Duration of Timed Deathmatch rounds in seconds");
            TimedDeathmatch_Inverted = Config.Bind(ModName, "TD_Invert", false, "Enable to use deaths instead of kills for scoring in Timed Deathmatch");
            StockBattle_Lives = Config.Bind(ModName, "SB_Lives", 3, "Number of lives in Stock Battle");
            StockBattle_Timed = Config.Bind(ModName, "SB_Timed", 0, "Enable to use the timer for Stock Battle");
        }

        void Start()
        {
           // GameModeManager.AddHandler<GM_Reverse_Rounds>(Reverse_Rounds_Handler.GameModeID, new Reverse_Rounds_Handler());
           // GameModeManager.AddHandler<GM_Reverse_Rounds>(Team_Reverse_Rounds_Handler.GameModeID, new Team_Reverse_Rounds_Handler());
            GameModeManager.AddHandler<GM_Timed_Deathmatch>(Timed_Deathmatch_Handler.GameModeID, new Timed_Deathmatch_Handler());
            GameModeManager.AddHandler<GM_Timed_Deathmatch>(Team_Timed_Deathmatch_Handler.GameModeID, new Team_Timed_Deathmatch_Handler());
            GameModeManager.AddHandler<GM_Stock_Battle>(Stock_Battle_Handler.GameModeID, new Stock_Battle_Handler());
            GameModeManager.AddHandler<GM_Stock_Battle>(Team_Stock_Battle_Handler.GameModeID, new Team_Stock_Battle_Handler());
            GameModeManager.AddHandler<GM_TitanFall>(TitanFall_Handler.GameModeID, new TitanFall_Handler());
            GameModeManager.AddHandler<GM_Rollover_Deathmatch>(Rollover_Deathmatch_Handler.GameModeID, new Rollover_Deathmatch_Handler());
            GameModeManager.AddHandler<GM_Rollover_Deathmatch>(Team_Rollover_Deathmatch_Handler.GameModeID, new Team_Rollover_Deathmatch_Handler());
            Unbound.RegisterHandshake(ModId, this.OnHandShakeCompleted);


            CustomCard.BuildCard<Titan_Card>(Titan_Card.callback);
            CustomCard.BuildCard<Fighter_Card>(Fighter_Card.callback);

            Unbound.RegisterMenu(ModName, () => { }, this.NewGUI, null, false);
        }

        private void OnHandShakeCompleted()
        { 
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkingManager.RPC_Others(typeof(Main), nameof(SyncSettings), new object[] { TimedDeathmatch_Time.Value, TimedDeathmatch_Inverted.Value, StockBattle_Lives.Value, StockBattle_Timed.Value});
            }
        }

        [UnboundRPC]
        private static void SyncSettings(int host_TD_Time, bool host_TD_Invert, int host_SB_Lives, int host_SB_Timed)
        {
            TimedDeathmatch_Time.Value = host_TD_Time;
            TimedDeathmatch_Inverted.Value = host_TD_Invert;
            StockBattle_Lives.Value = host_SB_Lives;
            StockBattle_Timed.Value = host_SB_Timed;
        }

        private void NewGUI(GameObject menu)
        { 
            TextMeshProUGUI TD_Timer = null;
            TextMeshProUGUI SB_Timer = null;
            MenuHandler.CreateText(ModName + " Options", menu, out TextMeshProUGUI _, 60);
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);
            MenuHandler.CreateSlider("Duration of Timed Deathmatch rounds in seconds", menu, 30, 60, 300, TimedDeathmatch_Time.Value, TD_Timer_Changed, out _, true);
            MenuHandler.CreateText(get_time_text(TimedDeathmatch_Time.Value), menu, out TD_Timer, 75);
            MenuHandler.CreateToggle(TimedDeathmatch_Inverted.Value, "Enable to use deaths instead of kills for scoring in Timed Deathmatch", menu, (value) => TimedDeathmatch_Inverted.Value = value, 30);
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);
            MenuHandler.CreateSlider("Number of Lives for Stock Battle", menu, 30, 2, 99, StockBattle_Lives.Value, (value) => StockBattle_Lives.Value = (int)value, out _, true);


            MenuHandler.CreateSlider("Enable to use the timer for Stock Battle", menu, 30, 0, 300, StockBattle_Timed.Value, SB_Timer_Changed, out _, true);
            MenuHandler.CreateText(get_time_text(StockBattle_Timed.Value,true), menu, out SB_Timer, 75);

            void TD_Timer_Changed(float val)
            {
                TimedDeathmatch_Time.Value = UnityEngine.Mathf.RoundToInt(val);
                OnHandShakeCompleted();

                if (TD_Timer != null)
                    TD_Timer.text = get_time_text(TimedDeathmatch_Time.Value);
            }
            void SB_Timer_Changed(float val)
            {
                StockBattle_Timed.Value = UnityEngine.Mathf.RoundToInt(val);
                OnHandShakeCompleted();

                if (SB_Timer != null)
                    SB_Timer.text = get_time_text(StockBattle_Timed.Value, true);
            }

            string get_time_text(int value,bool is_sb = false)
            {
                if (is_sb && value == 0) return "";
                int m = UnityEngine.Mathf.FloorToInt(value / 60f);
                int s = UnityEngine.Mathf.FloorToInt(value - (m * 60f));
                return $"{m}:{(s < 10 ? "0" : "")}{s}";
            }

        }

        

    }
}