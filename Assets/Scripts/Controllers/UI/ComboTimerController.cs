using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Controllers.UI
{
    public class ComboTimerController : MonoBehaviour
    {
        public Transform timerMaskAnchor;
        public float TimerLength;
        public float RapidFillMod;
        
        private float _drainSpeed;
        private const float _pixelRound = 0.25f;

        private float _tickLength;
        private float _maxSize;
        private float _currentSize;

        void Awake()
        {
            _maxSize = timerMaskAnchor.localScale.x;
            _tickLength = TimerLength * (_pixelRound / _maxSize);
            _drainSpeed = _maxSize / (TimerLength / _tickLength);
            _currentSize = 0;
        }

        void LateUpdate()
        {
            timerMaskAnchor.localScale = new Vector3(_currentSize - (_currentSize % _pixelRound), 1, 1);
        }

        private IEnumerator TickDownMeter()
        {
            while (_currentSize > 0)
            {
                yield return new WaitForSecondsRealtime(_tickLength);
                _currentSize -= _drainSpeed;
            }
            
        }

        private IEnumerator RapidFillMeter()
        {
            while (_currentSize < _maxSize)
            {
                _currentSize += _drainSpeed;
                yield return new WaitForSecondsRealtime(_tickLength / RapidFillMod);
            }
            _currentSize = _maxSize;
            StartCoroutine(TickDownMeter());
        }

        public void RefreshMeter()
        {
            StopAllCoroutines();
            StartCoroutine(RapidFillMeter());
        }

        public void ResetMeter()
        {
            StopAllCoroutines();
            _currentSize = 0;
        }

        public bool IsMeterEmpty()
        {
            return _currentSize <= 0;
        }
    }
}
