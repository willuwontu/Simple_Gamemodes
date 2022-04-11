using ModdingUtils.GameModes;
using ModdingUtils.MonoBehaviours;
using Simple_Gamemodes.Extentions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple_Gamemodes.Monos
{
    internal class TITAN : ReversibleEffect
    { 
        public override void OnStart()
        {
            gunStatModifier.damage_mult = 2;
            data.stats.GetAditionalData().damageCap = 1.0f / PlayerManager.instance.players.Count;
            SetLivesToEffect(int.MaxValue);
        }

        public override void OnOnEnable()
        {
            ApplyModifiers();
            data.stats.GetAditionalData().damageCap = 1.0f / PlayerManager.instance.players.Count;
        }

        public override void OnOnDisable()
        {
            ClearModifiers();
            data.stats.GetAditionalData().damageCap = 1.0f;
        }
        public override void OnOnDestroy()
        {
            data.stats.GetAditionalData().damageCap = 1.0f;
        }
    }
}
