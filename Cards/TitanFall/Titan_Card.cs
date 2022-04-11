using Simple_Gamemodes.Extentions;
using System;
using System.Collections.Generic;
using System.Text;
using UnboundLib.Cards;
using UnityEngine;

namespace Simple_Gamemodes.Cards.TitanFall
{
    internal class Titan_Card : CustomCard
    {
        public static CardInfo cardInfo;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            gun.damage = 2;
        }

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            characterStats.GetAditionalData().damageCap = 1.0f/PlayerManager.instance.players.Count;
        }

        protected override GameObject GetCardArt()
        {
            return null;
        }

        protected override string GetDescription()
        {
            return "You are the TITAN";
        }

        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Rare;
        }

        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[] {
                new CardInfoStat()
                {
                    positive = true,
                    amount = "+100%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned,
                    stat = "Damage"
                },
                new CardInfoStat()
                {
                    positive = true,
                    amount = "Damage resistance",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned,
                    stat = "per player"
                }
            };
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DestructiveRed;
        }

        protected override string GetTitle()
        {
            return "TITAN";
        }

        public override bool GetEnabled()
        {
            return false;
        }
        internal static void callback(CardInfo card)
        {
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(card);
            cardInfo = card;
        }
    }
}
