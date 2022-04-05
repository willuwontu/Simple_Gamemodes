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
        public const string Version = "0.0.0"; // What version are we on (major.minor.patch)?

        public static ConfigEntry<float> TimedDeathmatch_Time;

        void Awake()
        {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();

            TimedDeathmatch_Time = Config.Bind(ModName, "TD_Time", 3f, "Duration of Timed Deathmatch rounds in minutes");
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
                NetworkingManager.RPC_Others(typeof(Main), nameof(SyncSettings), new object[] { TimedDeathmatch_Time.Value });
            }
        }

        [UnboundRPC]
        private static void SyncSettings(float host_TD_Time)
        {
            TimedDeathmatch_Time.Value = host_TD_Time;
        }

        private void NewGUI(GameObject menu)
        {

            MenuHandler.CreateText(ModName + " Options", menu, out TextMeshProUGUI _, 60);
            MenuHandler.CreateText(" ", menu, out TextMeshProUGUI _, 30);
            void TD_Timer_Changed(float val)
            {
                TimedDeathmatch_Time.Value = clampToForth(val);
                OnHandShakeCompleted();
            }
            MenuHandler.CreateSlider("Duration of Timed Deathmatch rounds in minutes", menu, 30, 1f, (float)5, TimedDeathmatch_Time.Value, TD_Timer_Changed, out UnityEngine.UI.Slider _, true);

        }

        
        private float clampToForth(float val)
        {
            float remainder = val - (int)val;
            if(remainder>= 0.5f)
            {
                return (val - (int)val) + remainder >= 0.75f? 0.75f:0.50f;
            }
            else
            {
                return (val - (int)val) + remainder >= 0.25f ? 0.25f : 0.00f;
            }
        }
    }
}
