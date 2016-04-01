using System.Collections.Generic;
using UnityEngine;

namespace DerelictComputer
{
    public class OneShot : Instrument
    {
        private const int InitialVoices = 8;

        public bool DebugPlay;
        [Range(0, 127)] public int DebugNote;

        public List<OneShotSampleConfig> Samples = new List<OneShotSampleConfig>();

        [SerializeField] private OneShotSamplePlayer _samplePlayerPrefab;

        private readonly List<OneShotSamplePlayer> _samplePlayers = new List<OneShotSamplePlayer>();

        protected override void OnPlayNote(double playTime, double duration, int midiNote)
        {
            foreach (var sample in Samples)
            {
                if (sample.BottomNote <= midiNote && midiNote <= sample.TopNote)
                {
                    GetSamplePlayer().Play(playTime, midiNote, sample);
                }
            }
        }

        private void Awake()
        {
            for (int i = 0; i < InitialVoices; i++)
            {
                _samplePlayers.Add(MakeSamplePlayer());
            }
        }

        private void Update()
        {
            if (DebugPlay)
            {
                PlayNote(AudioSettings.dspTime, DebugNote);
                DebugPlay = false;
            }
        }

        private OneShotSamplePlayer GetSamplePlayer()
        {
            for (int i = 0; i < _samplePlayers.Count; i++)
            {
                if (_samplePlayers[i].Available)
                {
                    return _samplePlayers[i];
                }
            }

            var sp = MakeSamplePlayer();
            _samplePlayers.Add(sp);
            return sp;
        }

        private OneShotSamplePlayer MakeSamplePlayer()
        {
            var ossp = Instantiate(_samplePlayerPrefab);
            ossp.transform.SetParent(transform);
            ossp.transform.localPosition = Vector3.zero;
            ossp.transform.localScale = Vector3.one;
            ossp.transform.localRotation = Quaternion.identity;
            return ossp;
        }
    }
}
