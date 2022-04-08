using ModdingUtils.MonoBehaviours;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TMPro;
using UnboundLib;
using UnboundLib.GameModes;
using UnityEngine;

namespace Simple_Gamemodes.Monos
{
    internal class Timed_Kills : MonoBehaviour
    {
        Player player;

        GameObject _kills;

        GameObject Kills
        {
            get
            {
                if (!_kills)
                {
                    _kills = new GameObject("Kill Counter", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(Canvas));
                    _kills.transform.SetParent(player.transform);

                    var rect = _kills.GetComponent<RectTransform>();
                    rect.localScale = Vector3.one;

                    _kills.transform.position = (player.transform.Find("WobbleObjects/Healthbar/Canvas/CrownPos").position + (Vector3.up * 1));

                    var text = _kills.GetComponent<TextMeshProUGUI>();
                    text.text = "Timer";
                    text.alignment = TextAlignmentOptions.Center;
                    text.fontSize = 1f;

                    var fitter = _kills.AddComponent<UnityEngine.UI.ContentSizeFitter>();
                    fitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
                    fitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
                }

                return _kills;
            }
        }
        public void Start()
        {
            this.player = this.GetComponentInParent<Player>();
        }

        public void Update()
        {
            this.Kills.transform.localScale = new Vector3(1 / player.transform.localScale.x, 1 / player.transform.localScale.y, 1 / player.transform.localScale.z);
        }


        public void OnDestroy()
        {
            UnityEngine.GameObject.Destroy(Kills);
        }

        public void UpdateScore(string points, Color color)
        {
            Kills.GetOrAddComponent<TextMeshProUGUI>().text = points;
            Kills.GetOrAddComponent<TextMeshProUGUI>().color = color;
        }
    }
}
