using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using DerelictComputer.DroneMachine;

namespace DerelictComputer.Subtractinator
{
    [RequireComponent(typeof(AudioSource))]
    public class Subtractinator : Instrument
    {
        [SerializeField] private bool _debugPlay;
        [SerializeField] private bool _debugStop;
        [SerializeField, Range(0f, 1f)] private double _detuneAmount;
        [SerializeField, Range(0f, 1f)] private double _filterCutoff;
        [SerializeField, Range(0f, 1f)] private double _filterEnvAmount;
        [SerializeField, Range(0f, 1f)] private double _filterResonance;
        [SerializeField, Range(0f, 0.25f)] private double _attack;
        [SerializeField, Range(0f, 0.25f)] private double _decay;
        [SerializeField, Range(0f, 1f)] private double _sustain;
        [SerializeField, Range(0f, 0.25f)] private double _release;
        [SerializeField, Range(0f, 0.25f)] private double _freqSlideTime;

        private AudioSource _audioSource;
        private IntPtr _subPtr = IntPtr.Zero;
        private double _fcOld, _feOld, _frOld;

        [DllImport("SubtractinatorNative")]
        private static extern IntPtr Sub_New(double sampleDuration);

        [DllImport("SubtractinatorNative")]
        private static extern void Sub_Delete(IntPtr sub);

        [DllImport("SubtractinatorNative")]
        private static extern void Sub_SetFrequency(IntPtr sub, double frequency, double slideTime, double detuneAmount);

        [DllImport("SubtractinatorNative")]
        private static extern void Sub_ProcessBuffer(IntPtr sub, [In, Out] float[] buffer, int numSamples, int numChannels, double dspTime);

        [DllImport("SubtractinatorNative")]
        private static extern void Sub_SetEnvelope(IntPtr sub, double attackDuration, double decayDuration,
            double sustainLevel, double releaseDuration);

        [DllImport("SubtractinatorNative")]
        private static extern void Sub_SetFilterCutoffBase(IntPtr sub, double cutoff);

        [DllImport("SubtractinatorNative")]
        private static extern void Sub_SetFilterEnvelopeAmount(IntPtr sub, double amount);

        [DllImport("SubtractinatorNative")]
        private static extern void Sub_SetFilterResonance(IntPtr sub, double resonance);

        [DllImport("SubtractinatorNative")]
        private static extern void Sub_Start(IntPtr sub, double startTime);

        [DllImport("SubtractinatorNative")]
        private static extern void Sub_Release(IntPtr sub, double releaseTime);

        protected override void OnPlayNote(double playTime, double duration, int midiNote)
        {
            if (_subPtr != IntPtr.Zero)
            {
                Sub_SetFrequency(_subPtr, MusicMathUtils.MidiNoteToFrequency(midiNote), _freqSlideTime, _detuneAmount);
                Sub_SetEnvelope(_subPtr, _attack, _decay, _sustain, _release);
                Sub_Start(_subPtr, playTime);
            }
        }

        protected override void OnReleaseNote(double releaseTime, int midiNote)
        {
            if (_subPtr != IntPtr.Zero)
            {
                Sub_Release(_subPtr, releaseTime);
            }
        }

        private void OnEnable()
        {
            _subPtr = Sub_New(1.0/AudioSettings.outputSampleRate);
            Sub_SetFrequency(_subPtr, 55, 0, _detuneAmount);
            Sub_SetEnvelope(_subPtr, _attack, _decay, _sustain, _release);
            Sub_SetFilterCutoffBase(_subPtr, _filterCutoff);
            Sub_SetFilterEnvelopeAmount(_subPtr, _filterEnvAmount);
            Sub_SetFilterResonance(_subPtr, _filterResonance);


            // create a dummy clip and start playing it so 3d positioning works
            var dummyClip = AudioClip.Create("dummyclip", 1, 1, AudioSettings.outputSampleRate, false);
            dummyClip.SetData(new float[] { 1 }, 0);
            var audioSource = GetComponent<AudioSource>();
            audioSource.clip = dummyClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        private void OnDisable()
        {
            if (_subPtr != IntPtr.Zero)
            {
                Sub_Delete(_subPtr);
                _subPtr = IntPtr.Zero;
            }
        }

        private void Update()
        {
            if (_debugPlay)
            {
                if (_subPtr != IntPtr.Zero)
                {
                    //Sub_SetFrequency(_subPtr, 20.0 + UnityEngine.Random.value * 1000, 1.0);
                    Sub_Start(_subPtr, AudioSettings.dspTime);
                }
                _debugPlay = false;
            }

            if (_debugStop)
            {
                if (_subPtr != IntPtr.Zero)
                {
                    Sub_Release(_subPtr, AudioSettings.dspTime);
                }

                _debugStop = false;
            }
        }

        private void OnValidate()
        {
            if (_subPtr != IntPtr.Zero)
            {
                if (_filterCutoff != _fcOld)
                {
                    _fcOld = _filterCutoff;
                    Sub_SetFilterCutoffBase(_subPtr, _filterCutoff);
                }

                if (_filterEnvAmount != _feOld)
                {
                    _feOld = _filterEnvAmount;
                    Sub_SetFilterEnvelopeAmount(_subPtr, _filterEnvAmount);
                }

                if (_filterResonance != _frOld)
                {
                    _frOld = _filterResonance;
                    Sub_SetFilterResonance(_subPtr, _filterResonance);
                }
            }
        }

        private void OnAudioFilterRead(float[] buffer, int numChannels)
        {
            if (_subPtr != IntPtr.Zero)
            {
                Sub_ProcessBuffer(_subPtr, buffer, buffer.Length, numChannels, AudioSettings.dspTime);
            }
        }
    }
}
