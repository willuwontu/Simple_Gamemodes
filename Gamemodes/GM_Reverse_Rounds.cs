using RWF;
using RWF.GameModes;
using RWF.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;
using ItemShops.Utils;
using ModdingUtils.Utils;
using Simple_Gamemodes.Extentions;

namespace Simple_Gamemodes.Gamemodes
{
    public class GM_Reverse_Rounds : RWFGameMode
    {

        public override IEnumerator DoStartGame()
        {
            CardBarHandler.instance.Rebuild();
            UIHandler.instance.InvokeMethod("SetNumberOfRounds", 1);
            ArtHandler.instance.NextArt();
            yield return GameModeManager.TriggerHook("GameStart");
            GameManager.instance.battleOngoing = false;
            UIHandler.instance.ShowJoinGameText("LETS GOO!", PlayerSkinBank.GetPlayerSkinColors(1).winText);
            yield return new WaitForSecondsRealtime(0.25f);
            UIHandler.instance.HideJoinGameText();
            PlayerSpotlight.CancelFade(disable_shadow: true);
            PlayerManager.instance.SetPlayersSimulated(simulated: false);
            PlayerManager.instance.InvokeMethod("SetPlayersVisible", false);
            MapManager.instance.LoadNextLevel();
            TimeHandler.instance.DoSpeedUp(); 
            List<Player> pickOrder = PlayerManager.instance.players;
            int cards = (int)GameModeManager.CurrentHandler.Settings["roundsToWinGame"];
            yield return new WaitForSecondsRealtime(1f);
            for (int _ = 0; _ < cards; _++)
            {
                yield return GameModeManager.TriggerHook("PickStart");
                foreach (Player player in pickOrder)
                {
                    yield return WaitForSyncUp();
                    yield return GameModeManager.TriggerHook("PlayerPickStart");
                    CardChoiceVisuals.instance.Show(player.playerID, animateIn: true);
                    yield return CardChoice.instance.DoPick(1, player.playerID, PickerType.Player);
                    yield return GameModeManager.TriggerHook("PlayerPickEnd");
                    yield return new WaitForSecondsRealtime(0.1f);
                }

                yield return WaitForSyncUp();
                CardChoiceVisuals.instance.Hide();
                yield return GameModeManager.TriggerHook("PickEnd");
            }
            yield return new WaitForSecondsRealtime(1f);
            foreach (Player player in pickOrder)
            {
                try
                {
                    ShopManager.instance.CreateShop("Card_Remove_Shop" + player.playerID.ToString());
                }
                catch {
                    ShopManager.instance.Shops["Card_Remove_Shop" + player.playerID.ToString()].RemoveAllItems();
                }
                Shop shop = ShopManager.instance.Shops["Card_Remove_Shop" + player.playerID.ToString()];
                CardInfo[] cardInfos = player.data.currentCards.ToArray();
                foreach(CardInfo card in cardInfos)
                {
                    try
                    {
                        shop.AddItem(card.name, new RemoveCard(card, new Dictionary<string, int>(), new Tag[0], card.name));
                    }catch(Exception e) { UnityEngine.Debug.LogError(e); }
                }
            }
            PlayerSpotlight.FadeIn();
            MapManager.instance.CallInNewMapAndMovePlayers(MapManager.instance.currentLevelID);
            TimeHandler.instance.DoSpeedUp();
            TimeHandler.instance.StartGame();
            GameManager.instance.battleOngoing = true;
            UIHandler.instance.ShowRoundCounterSmall(teamPoints, teamRounds);
            PlayerManager.instance.InvokeMethod("SetPlayersVisible", true);
            StartCoroutine(DoRoundStart());
        }


        public override IEnumerator RoundTransition(int[] winningTeamIDs)
        {
            yield return GameModeManager.TriggerHook("PointEnd");
            yield return GameModeManager.TriggerHook("RoundEnd");
            if (GameModeManager.CurrentHandler.GetGameWinners().Any())
            {
                GameOver(winningTeamIDs);
                yield break;
            }

            StartCoroutine(PointVisualizer.instance.DoWinSequence(teamPoints, teamRounds, winningTeamIDs));
            yield return new WaitForSecondsRealtime(1f);
            MapManager.instance.LoadNextLevel();
            yield return new WaitForSecondsRealtime(1.3f);
            PlayerManager.instance.SetPlayersSimulated(simulated: false);
            TimeHandler.instance.DoSpeedUp();
            yield return GameModeManager.TriggerHook("PickStart");
            PlayerManager.instance.InvokeMethod("SetPlayersVisible", false);
            List<Player> pickOrder = PlayerManager.instance.players;
            foreach (Player player in pickOrder)
            {
                if (winningTeamIDs.Contains(player.teamID))
                {
                    yield return WaitForSyncUp();
                    yield return GameModeManager.TriggerHook("PlayerPickStart");
                    player.data.stats.GetAditionalData().Removing = true;
                    ShopManager.instance.Shops["Card_Remove_Shop" + player.playerID.ToString()].Show(player);
                    while(ShopManager.instance.PlayerIsInShop(player)|| player.data.stats.GetAditionalData().Removing)
                    {
                        if (!player.data.stats.GetAditionalData().Removing)
                        {
                            ShopManager.instance.Shops["Card_Remove_Shop" + player.playerID.ToString()].Hide();
                        }
                        else if (!ShopManager.instance.PlayerIsInShop(player))
                        {
                            ShopManager.instance.Shops["Card_Remove_Shop" + player.playerID.ToString()].Show(player);
                        }
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                    yield return GameModeManager.TriggerHook("PlayerPickEnd");
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }

            PlayerManager.instance.InvokeMethod("SetPlayersVisible", true);
            yield return GameModeManager.TriggerHook("PickEnd");
            yield return StartCoroutine(WaitForSyncUp());
            PlayerSpotlight.FadeIn();
            TimeHandler.instance.DoSlowDown();
            MapManager.instance.CallInNewMapAndMovePlayers(MapManager.instance.currentLevelID);
            PlayerManager.instance.RevivePlayers();
            yield return new WaitForSecondsRealtime(0.3f);
            TimeHandler.instance.DoSpeedUp();
            GameManager.instance.battleOngoing = true;
            isTransitioning = false;
            UIHandler.instance.ShowRoundCounterSmall(teamPoints, teamRounds);

        }
    }

    public class RemoveCard : PurchasableCard
    {
        public RemoveCard(CardInfo card, Dictionary<string, int> cost, Tag[] tags, string name) : base(card, cost, tags, name)
        {
        }

        public override bool CanPurchase(Player player)
        {
            return player.data.currentCards.Contains(Card);
        }
        public override void OnPurchase(Player player, Purchasable item)
        {
            CardInfo card = ((PurchasableCard)item).Card;
            ModdingUtils.Utils.Cards.instance.RemoveCardFromPlayer(player, card, ModdingUtils.Utils.Cards.SelectionType.Newest, false);
            player.data.stats.GetAditionalData().Removing = false;
        }
    }

}
