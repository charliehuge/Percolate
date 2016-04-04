using System;
using System.Runtime.InteropServices;
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

        private IntPtr _nativePtr = IntPtr.Zero;
        private AudioSource _audioSource;

        [DllImport("VolumeEnvelopeNative")]
        private static extern IntPtr VolumeEnvelope_New(double sampleDuration);

        [DllImport("VolumeEnvelopeNative")]
        private static extern void VolumeEnvelope_Delete(IntPtr env);

        [DllImport("VolumeEnvelopeNative")]
        private static extern void VolumeEnvelope_SetEnvelope(IntPtr env, double startTime,
            double attackDuration, double sustainDuration, double releaseDuration);

        [DllImport("VolumeEnvelopeNative")]
        private static extern bool VolumeEnvelope_ProcessBuffer(IntPtr env, [In, Out] float[] buffer, int numSamples,
            int numChannels, double dspTime);

        [DllImport("VolumeEnvelopeNative")]
        private static extern bool VolumeEnvelope_IsAvailable(IntPtr env);

        public bool Available
        {
            get { return VolumeEnvelope_IsAvailable(_nativePtr); }
        }

        public void Play(double playTime, int note, OneShotSampleConfig config)
        {
            if (_nativePtr != IntPtr.Zero)
            {
                VolumeEnvelope_SetEnvelope(_nativePtr, playTime, config.AttackTime, config.SustainTime, config.ReleaseTime);
            }

            _audioSource.pitch = config.ScalePitch ? SemitonesToPitch(note - config.BottomNote) : 1;
            _audioSource.clip = config.Clip;
            _audioSource.timeSamples = 0;
            _audioSource.PlayScheduled(playTime);
        }

        private void OnEnable()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.Stop();

            _nativePtr = VolumeEnvelope_New(1.0/AudioSettings.outputSampleRate);
        }

        private void OnDisable()
        {
            if (_nativePtr != IntPtr.Zero)
            {
                VolumeEnvelope_Delete(_nativePtr);
                _nativePtr = IntPtr.Zero;
            }
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
            if (_nativePtr != IntPtr.Zero)
            {
                VolumeEnvelope_ProcessBuffer(_nativePtr, buffer, buffer.Length, numChannels, AudioSettings.dspTime);
            }
        }

        private static float SemitonesToPitch(float semitones)
        {
            return Mathf.Pow(2f, semitones / 12f);
        }
    }
}
