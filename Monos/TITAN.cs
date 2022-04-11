using ModdingUtils.GameModes;
using ModdingUtils.MonoBehaviours;
using Simple_Gamemodes.Extentions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnboundLib.GameModes;
using UnityEngine;

namespace Simple_Gamemodes.Monos
{
    internal class TITAN : ReversibleEffect
    { 
        public override void OnStart()
        {
            gunStatModifier.damage_mult = 2;
            data.stats.GetAditionalData().damageCap = 1.0f / PlayerManager.instance.players.Count;
            applyImmediately = true;
            SetLivesToEffect(int.MaxValue);
            GameModeManager.AddHook(GameModeHooks.HookBattleStart, stats);
        }

        public IEnumerator stats(IGameModeHandler gm)
        {
            ClearModifiers();
            yield return new WaitForEndOfFrame();
            player.gameObject.transform.Find("WobbleObjects/Healthbar/Canvas/PlayerName").GetComponent<TextMeshProUGUI>().color = Color.red;
            ApplyModifiers();
            yield break;
        }

        public override void OnOnDestroy()
        {
            GameModeManager.RemoveHook(GameModeHooks.HookGameStart, stats);
            player.gameObject.transform.Find("WobbleObjects/Healthbar/Canvas/PlayerName").GetComponent<TextMeshProUGUI>().color = new Color(0.6132f, 0.6132f, 0.6132f);
            data.stats.GetAditionalData().damageCap = 1.0f;
        }
    }
}
