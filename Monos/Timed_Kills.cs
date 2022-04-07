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

        GameObject _killFrame;
        GameObject _kills;

        GameObject KillFrame
        {
            get
            {
                if (!_killFrame)
                {
                    _killFrame = Instantiate(player.transform.Find("WobbleObjects/Healthbar"), player.transform.Find("WobbleObjects")).gameObject;
                    _killFrame.name = "Kills Frame";
                    _killFrame.transform.localScale = Vector3.one;
                    _killFrame.transform.localPosition = new Vector3(0, 0.851f, 0);

                    UnityEngine.GameObject.Destroy(_killFrame.GetComponent<HealthBar>());

                    var killCanvas = _killFrame.transform.Find("Canvas").gameObject;

                    var COs = killCanvas.GetComponentsInChildren<Transform>().Where(child => child.parent == killCanvas.transform).Select(child => child.gameObject).ToArray();
                    foreach (var CO in COs)
                    {
                        UnityEngine.GameObject.Destroy(CO);
                    }
                }

                return _killFrame;
            }
        }
        GameObject Kills
        {
            get
            {
                if (!_kills)
                {
                    var _ = KillFrame;
                    _kills = new GameObject("Kill Counter", typeof(RectTransform), typeof(TextMeshProUGUI));
                    _kills.transform.SetParent(KillFrame.transform.Find("Canvas"));

                    var rect = _kills.GetComponent<RectTransform>();
                    rect.localScale = Vector3.one;
                    rect.localPosition = new Vector3(0, 150, 0);

                    var text = _kills.GetComponent<TextMeshProUGUI>();
                    text.text = "Timer";
                    text.alignment = TextAlignmentOptions.Center;
                    text.fontSize = 200f;

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

            this.UpdateScore("0", new Color32(255, 255, 255, 255));
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


        public void OnDestroy()
        {
            UnityEngine.GameObject.Destroy(KillFrame);
        }

        public void UpdateScore(string points, Color color)
        {
            Kills.GetOrAddComponent<TextMeshProUGUI>().text = points;
            Kills.GetOrAddComponent<TextMeshProUGUI>().color = color;
        }
    }
}
