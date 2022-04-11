using ModdingUtils.GameModes;
using ModdingUtils.MonoBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnboundLib.GameModes;
using UnityEngine;

namespace Simple_Gamemodes.Monos
{
    internal class Fighter : ReversibleEffect
    {

        public override void OnStart()
        {
            characterStatModifiersModifier.sizeMultiplier_mult = 0.5f;
            applyImmediately = true;
            SetLivesToEffect(int.MaxValue);
            GameModeManager.AddHook(GameModeHooks.HookBattleStart, stats);
            GameModeManager.AddHook(GameModeHooks.HookRoundEnd, OnRoundEnd);
        }

        public IEnumerator stats(IGameModeHandler gm)
        {
            ClearModifiers();
            yield return new WaitForEndOfFrame();
            ApplyModifiers();
            yield break;
        }
        public override void OnOnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookGameStart, stats);
            GameModeManager.RemoveHook(GameModeHooks.HookRoundEnd, OnRoundEnd);
        }

        public IEnumerator OnRoundEnd(IGameModeHandler gm)
        {
            Destroy(this);
            yield break;
        }
    }
}
