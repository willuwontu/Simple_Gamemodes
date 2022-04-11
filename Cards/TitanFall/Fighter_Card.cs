using System;
using System.Collections.Generic;
using System.Text;
using UnboundLib.Cards;
using UnityEngine;

namespace Simple_Gamemodes.Cards.TitanFall
{
    internal class Fighter_Card : CustomCard
    {
        public static CardInfo cardInfo;
        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
        {
            statModifiers.sizeMultiplier = 0.5f;
        }
        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats)
        {
            
        }

        protected override GameObject GetCardArt()
        {
            return null;
        }

        protected override string GetDescription()
        {
            return "Help kill the TITAN";
        }

        protected override CardInfo.Rarity GetRarity()
        {
            return CardInfo.Rarity.Uncommon;
        }

        protected override CardInfoStat[] GetStats()
        {
            return new CardInfoStat[] { 
            
                new CardInfoStat()
                {
                    positive = true,
                    amount = "-50%",
                    simepleAmount = CardInfoStat.SimpleAmount.notAssigned,
                    stat = "amount"
                }
            };
        }

        protected override CardThemeColor.CardThemeColorType GetTheme()
        {
            return CardThemeColor.CardThemeColorType.DefensiveBlue;
        }

        protected override string GetTitle()
        {
            return "FIGHTER";
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
