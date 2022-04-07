﻿using ModdingUtils.MonoBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace Simple_Gamemodes.Monos
{
    internal class Timed_Kills : ReversibleEffect
    {
        GameObject kills;
        public override void OnStart()
        {
            SetLivesToEffect(int.MaxValue);
            kills = new GameObject("Score_Tracker");
            kills.transform.SetParent(player.transform.Find("WobbleObjects"), false);
            kills.transform.localPosition = Vector3.up * 1.75f;
            kills.GetOrAddComponent<TextMeshProUGUI>().text = "";
            kills.GetOrAddComponent<TextMeshProUGUI>().color = Color.yellow;
            kills.GetOrAddComponent<TextMeshProUGUI>().fontSize = 0.75f;
            kills.GetOrAddComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            kills.GetOrAddComponent<Canvas>().sortingLayerName = "MostFront";
        }


        public override void OnOnDestroy()
        {
            Destroy(kills);
        }

        public void UpdateScore(string points, Color color)
        {
            kills.GetOrAddComponent<TextMeshProUGUI>().text = points;
            kills.GetOrAddComponent<TextMeshProUGUI>().color = color;
        }
    }
}
