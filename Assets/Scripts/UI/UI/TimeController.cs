using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BladeBreaker.UI
{
    public class TimeController : MonoBehaviour
    {
        public Transform Min1;
        public Transform Min2;
        public Transform Min3;
        public Transform Sec1;
        public Transform Sec2;
        public Transform MSec1;
        public Transform MSec2;

        public Transform Colon;
        public Transform Dot;

        private List<Transform> _min1Nums = new List<Transform>();
        private List<Transform> _min2Nums = new List<Transform>();
        private List<Transform> _min3Nums = new List<Transform>();
        private List<Transform> _sec1Nums = new List<Transform>();
        private List<Transform> _sec2Nums = new List<Transform>();
        private List<Transform> _mSec1Nums = new List<Transform>();
        private List<Transform> _mSec2Nums = new List<Transform>();

        private List<Transform>[] _digits;

        private void Awake()
        {
            foreach (Transform child in Min1) _min1Nums.Add(child);
            foreach (Transform child in Min2) _min2Nums.Add(child);
            foreach (Transform child in Min3) _min3Nums.Add(child);
            foreach (Transform child in Sec1) _sec1Nums.Add(child);
            foreach (Transform child in Sec2) _sec2Nums.Add(child);
            foreach (Transform child in MSec1) _mSec1Nums.Add(child);
            foreach (Transform child in MSec2) _mSec2Nums.Add(child);

            _digits = new List<Transform>[] { _min3Nums, _min2Nums, _min1Nums, _sec2Nums, _sec1Nums, _mSec1Nums, _mSec2Nums };
        }

        public int FormatTime(float time)
        {
            string mins = ((time - (time % 60))/60).ToString();
            string secs = Mathf.FloorToInt(time % 60).ToString();
            string millisecs = Mathf.FloorToInt(100 * (time - Mathf.FloorToInt(time))).ToString();

            while (mins.Length < 3)
            {
                mins = $"0{mins}";
            }

            while (secs.Length < 2)
            {
                secs = $"0{secs}";
            }

            while (millisecs.Length < 2)
            {
                millisecs = $"0{millisecs}";
            }

            return int.Parse($"{mins}{secs}{millisecs}");
        }


        public IEnumerator TickUpTime(float tickTime, float time)
        {
            SetTimeDisplay(10000000);
            Colon.gameObject.SetActive(true);
            Dot.gameObject.SetActive(true);

            int timeResult = FormatTime(time);
            float ellapsedTime = 0;
            int newTime = 0;

            while (ellapsedTime < tickTime)
            {
                ellapsedTime += UnityEngine.Time.deltaTime;
                newTime = (int)Mathf.Round(timeResult * (ellapsedTime / tickTime));
                if (newTime > timeResult)
                {
                    newTime = timeResult;
                }
                SetTimeDisplay(10000000+newTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private void SetTimeDisplay(int newTime)
        {
            string scoreStr = newTime.ToString();

            for (int i = 1; i < 8; i++)
            {
                char digit = scoreStr[i];
                ChangeActive(i-1, digit);
            }
        }

        public void ChangeActive(int digit, char number)
        {
            List<Transform> digits = _digits[digit];
            foreach (Transform num in digits)
            {
                if (num.name == $"NUM{number}")
                {
                    num.gameObject.SetActive(true);
                }
                else
                {
                    num.gameObject.SetActive(false);
                }
            }
        }
    }
}
