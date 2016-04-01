using System;
using UnityEngine;

namespace DerelictComputer
{
    public class Metronome : MonoBehaviour
    {
        public event Action<double> Ticked; 

        [Range(0.016f, 10f)] public double Interval = 0.125; // default = 16th note at 120 BPM

        private double _nextPulseTime;

        private void OnEnable()
        {
            _nextPulseTime = AudioSettings.dspTime;
        }

        private void Update()
        {
            while (AudioSettings.dspTime + Interval > _nextPulseTime)
            {
                var thisPulseTime = _nextPulseTime;

                _nextPulseTime += Interval;

                if (Ticked != null)
                {
                    Ticked(thisPulseTime);
                }
            }
        }

    }
}
