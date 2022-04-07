using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Simple_Gamemodes.Extentions
{
    [Serializable]
    public class CharacterStatModifiersExtraData
    {
        public bool Removing;
        public bool invanerable;

        public CharacterStatModifiersExtraData()
        {
            this.Removing = false;
            this.invanerable = false;
        }
        public void Reset()
        {
            this.invanerable = false;
        }
    }

    public static class CharacterStatModifiersExtension
    {
        public static readonly ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersExtraData> data =
          new ConditionalWeakTable<CharacterStatModifiers, CharacterStatModifiersExtraData>();


        public static CharacterStatModifiersExtraData GetAditionalData(this CharacterStatModifiers characterstats)
        {
            return data.GetOrCreateValue(characterstats);
        }


        public static void AddData(this CharacterStatModifiers characterstats, CharacterStatModifiersExtraData value)
        {
            try
            {
                data.Add(characterstats, value);
            }
            catch (Exception) { }
        }

    }
    [HarmonyPatch(typeof(CharacterStatModifiers), "ResetStats")]
    class CharacterStatModifiersPatchResetStats
    {
        private static void Prefix(CharacterStatModifiers __instance)
        {
            __instance.GetAditionalData().Reset();
        }
    }
}
