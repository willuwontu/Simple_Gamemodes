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
        public const string Version = "1.1.0"; // What version are we on (major.minor.patch)?

        public static ConfigEntry<int> TimedDeathmatch_Time;
        public static ConfigEntry<bool> TimedDeathmatch_Inverted;

        void Awake()
        {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();

            TimedDeathmatch_Time = Config.Bind(ModName, "TD_Time", 180, "Duration of Timed Deathmatch rounds in seconds");
            TimedDeathmatch_Inverted = Config.Bind(ModName, "TD_Invert", false, "Enable to use deaths instead of kills for scoring in Timed Deathmatch");
        }

        void Start()
        {
           // GameModeManager.AddHandler<GM_Reverse_Rounds>(Reverse_Rounds_Handler.GameModeID, new Reverse_Rounds_Handler());
           // GameModeManager.AddHandler<GM_Reverse_Rounds>(Team_Reverse_Rounds_Handler.GameModeID, new Team_Reverse_Rounds_Handler());
            GameModeManager.AddHandler<GM_Timed_Deathmatch>(Timed_Deathmatch_Handler.GameModeID, new Timed_Deathmatch_Handler());
            GameModeManager.AddHandler<GM_Timed_Deathmatch>(Team_Timed_Deathmatch_Handler.GameModeID, new Team_Timed_Deathmatch_Handler());
            Unbound.RegisterHandshake(ModId, this.OnHandShakeCompleted);


            Unbound.RegisterMenu(ModName, () => { }, this.NewGUI, null, false);
        }

        private void OnHandShakeCompleted()
        { 
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkingManager.RPC_Others(typeof(Main), nameof(SyncSettings), new object[] { TimedDeathmatch_Time.Value, TimedDeathmatch_Inverted.Value });
            }
        }

        [UnboundRPC]
        private static void SyncSettings(int host_TD_Time, bool host_TD_Invert)
        {
            TimedDeathmatch_Time.Value = host_TD_Time;
            TimedDeathmatch_Inverted.Value = host_TD_Invert;
        }

        private void NewGUI(GameObject menu)
        { 
            TextMeshProUGUI Timer = null;
            MenuHandler.CreateText(ModName + " Options", menu, out TextMeshProUGUI _, 60);
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);
            MenuHandler.CreateSlider("Duration of Timed Deathmatch rounds in seconds", menu, 30, 60, 300, TimedDeathmatch_Time.Value, TD_Timer_Changed, out _, true);
            MenuHandler.CreateText(get_time_text(TimedDeathmatch_Time.Value), menu, out Timer, 75);
            MenuHandler.CreateToggle(TimedDeathmatch_Inverted.Value, "Enable to use deaths instead of kills for scoring in Timed Deathmatch", menu, (value) => TimedDeathmatch_Inverted.Value = value, 30);
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);

            void TD_Timer_Changed(float val)
            {
                TimedDeathmatch_Time.Value = UnityEngine.Mathf.RoundToInt(val);
                OnHandShakeCompleted();

                if (Timer != null)
                    Timer.text = get_time_text(TimedDeathmatch_Time.Value);
            }

            string get_time_text(int value)
            {
                int m = UnityEngine.Mathf.FloorToInt(value / 60f);
                int s = UnityEngine.Mathf.FloorToInt(value - (m * 60f));
                return $"{m}:{(s < 10 ? "0" : "")}{s}";
            }

        }

        

    }
}