using HarmonyLib;
using Simple_Gamemodes.Gamemodes;
using System;
using System.Collections.Generic;
using System.Text;
using UnboundLib;

namespace Simple_Gamemodes.Patches
{
    [Serializable]
    [HarmonyPatch(typeof(HealthHandler), "DoDamage")]
    internal class HealthHandlerPatchDoDamage
    {
        private static void Prefix(HealthHandler __instance, Player damagingPlayer)
        {
            if (damagingPlayer != null && GM_Timed_Deathmatch.instance != null)
                GM_Timed_Deathmatch.instance.lastPlayerDamage[((CharacterData)__instance.GetFieldValue("data")).player.playerID] = damagingPlayer.playerID;
        }
    }
}
