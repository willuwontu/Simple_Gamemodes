using HarmonyLib;
using Simple_Gamemodes.Extentions;
using Simple_Gamemodes.Gamemodes;
using System;
using System.Collections.Generic;
using System.Text;
using UnboundLib;
using UnityEngine;

namespace Simple_Gamemodes.Patches
{
    [Serializable]
    [HarmonyPatch(typeof(HealthHandler), "DoDamage")]
    internal class HealthHandlerPatchDoDamage
    {
        private static bool Prefix(HealthHandler __instance, ref Vector2 damage, ref Player damagingPlayer)
        {
            
            if (((CharacterStatModifiers)__instance.GetFieldValue("stats")).GetAditionalData().invanerable)
            {
                __instance.GetComponentInChildren<PlayerSkinHandler>().BlinkColor(Color.black);
                return false;
            }

            if (damagingPlayer != null && GM_Timed_Deathmatch.instance != null)
            {
                if (ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(damagingPlayer.data).isAIMinion)
                    GM_Timed_Deathmatch.instance.lastPlayerDamage[((CharacterData)__instance.GetFieldValue("data")).player.playerID] = ModdingUtils.AIMinion.Extensions.CharacterDataExtension.GetAdditionalData(damagingPlayer.data).spawner.playerID;
                else 
                    GM_Timed_Deathmatch.instance.lastPlayerDamage[((CharacterData)__instance.GetFieldValue("data")).player.playerID] = damagingPlayer.playerID;
            }

            if(((CharacterStatModifiers)__instance.GetFieldValue("stats")).GetAditionalData().damageCap != 0)
            {
                damage = clampMagnatued(damage, 
                    (((CharacterData)__instance.GetFieldValue("data")).maxHealth * ((CharacterStatModifiers)__instance.GetFieldValue("stats")).GetAditionalData().damageCap) - ((CharacterStatModifiers)__instance.GetFieldValue("stats")).GetAditionalData().damageCapFilled);
            }
            return true;
        }

        private static void Postfix(HealthHandler __instance, ref Vector2 damage, ref Player damagingPlayer)
        {
            ((CharacterStatModifiers)__instance.GetFieldValue("stats")).GetAditionalData().damageCapFilled += damage.magnitude;
        }

        private static Vector2 clampMagnatued(Vector2 vector, float max)
        {
            float oldMag = vector.magnitude;
            if (oldMag <= max) return vector;
            return vector.normalized * max;
        }
    }
}
