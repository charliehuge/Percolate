using System;
using UnityEngine;

namespace DerelictComputer
{
    [Serializable]
    public class OneShotSampleConfig
    {
        public AudioClip Clip;
        [Range(0, 127)] public int BottomNote;
        [Range(0, 127)] public int TopNote;
        public bool ScalePitch;
        public double AttackTime;
        public double SustainTime;
        public double ReleaseTime;

        public static OneShotSampleConfig Default
        {
            get
            {
                var config = new OneShotSampleConfig();
                config.BottomNote = 0;
                config.TopNote = 127;
                config.ScalePitch = true;
                config.AttackTime = 0;
                config.SustainTime = 1;
                config.ReleaseTime = 0.1;
                return config;
            }
        }
    }

    [RequireComponent(typeof(AudioSource))]
    public class OneShotSamplePlayer : MonoBehaviour
    {
        public bool DebugPlay;
        public AudioClip DebugClip;

        private AudioSource _audioSource;
        private double _sampleDuration;
        private OneShotSampleConfig _config;
        private double _playTime;
        private double _attackEndTime;
        private double _sustainEndTime;
        private double _releaseEndTime;

        public bool Available
        {
            get { return _playTime < 0; }
        }

        public void Play(double playTime, int note, OneShotSampleConfig config)
        {
            _playTime = playTime;
            _attackEndTime = _playTime + config.AttackTime;
            _sustainEndTime = _attackEndTime + config.SustainTime;
            _releaseEndTime = _sustainEndTime + config.ReleaseTime;

            _config = config;
            _audioSource.pitch = config.ScalePitch ? SemitonesToPitch(note - config.BottomNote) : 1;
            _audioSource.clip = config.Clip;
            _audioSource.PlayScheduled(playTime);
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.Stop();

            _sampleDuration = 1.0/AudioSettings.outputSampleRate;
            _playTime = -1;
        }

        private void Update()
        {
            if (DebugPlay)
            {
                var config = OneShotSampleConfig.Default;
                config.Clip = DebugClip;
                Play(AudioSettings.dspTime, 0, config);
                DebugPlay = false;
            }
        }

        private void OnAudioFilterRead(float[] buffer, int numChannels)
        {
            float volume = 0;
            double dspTime = AudioSettings.dspTime;

            // if we won't get to the play time during this buffer, skip it
            if (_playTime < 0 || (dspTime + buffer.Length*_sampleDuration) < _playTime)
            {
                return;
            }

            for (int i = 0; i < buffer.Length; i += numChannels)
            {
                // wait until we hit the play start time
                // optimization: shortcut if we hit the end of the release during this buffer
                if (_playTime < 0 || dspTime < _playTime)
                {
                    volume = 0;
                }
                else if (_config.AttackTime > 0 && dspTime < _attackEndTime)
                {
                    volume = (float)Math.Pow((dspTime - _playTime) / _config.AttackTime, 4);
                }
                else if (dspTime < _sustainEndTime)
                {
                    volume = 1;
                }
                else if (_config.ReleaseTime > 0 && dspTime < _releaseEndTime)
                {
                    volume = (float)Math.Pow((_releaseEndTime - dspTime) / _config.ReleaseTime, 4);
                }
                else
                {
                    volume = 0;
                    _playTime = -1;
                }

                for (int j = 0; j < numChannels; j++)
                {
                    buffer[i + j] *= volume;
                }

                dspTime += _sampleDuration;
            }
        }

        private static float SemitonesToPitch(float semitones)
        {
            return Mathf.Pow(2f, semitones / 12f);
        }
    }
}
