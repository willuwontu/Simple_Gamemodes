using ModdingUtils.GameModes;
using ModdingUtils.MonoBehaviours;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple_Gamemodes.Monos
{
    internal class Fighter : ReversibleEffect
    {

        public override void OnStart()
        {
            characterStatModifiersModifier.sizeMultiplier_mult = 0.5f;
            SetLivesToEffect(int.MaxValue);
        }
    }
}
