using CharTween;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

namespace GFG
{
    public class CharTweenerView : MonoBehaviour
    {
        public TextMeshProUGUI TextToTween;

        public float ShakeDuration = 1;
        public float ShakeStrength = 10;
        public int ShakeVibrato = 50;

        CharTweener _tweener;

        private void Awake()
        {
            _tweener = TextToTween.GetCharTweener();
            Tween2(0, TextToTween.textInfo.characterCount);
        }

        private void Tween2(int start, int end)
        {
            for (var i = start; i <= end; ++i)
            {
                var timeOffset = Mathf.Lerp(0, 1, (i - start) / (float)(end - start + 1));
                _tweener.DOShakePosition(i, ShakeDuration, ShakeStrength, ShakeVibrato, 90, false, false)
                    .SetLoops(-1, LoopType.Restart);
                /*var colorTween = _tweener.DOColor(i, UnityEngine.Random.ColorHSV(0, 1, 1, 1, 1, 1),
                        UnityEngine.Random.Range(0.1f, 0.5f))
                    .SetLoops(-1, LoopType.Yoyo);
                colorTween.fullPosition = timeOffset;*/
            }
        }
    }
}