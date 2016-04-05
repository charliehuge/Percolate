using UnityEngine;

namespace DerelictComputer
{
    public abstract class Instrument : MonoBehaviour
    {
        public void PlayNote(int midiNote)
        {
            OnPlayNote(AudioSettings.dspTime, 0, midiNote);
        }

        public void PlayNote(double playTime, int midiNote)
        {
            OnPlayNote(playTime, 0, midiNote);
        }

        public void PlayNote(double playTime, double duration, int midiNote)
        {
            OnPlayNote(playTime, duration, midiNote);
        }

        public void ReleaseNote(double releaseTime)
        {
            OnReleaseNote(releaseTime, 0);
        }

        public void ReleaseNote(int midiNote)
        {
            OnReleaseNote(AudioSettings.dspTime, midiNote);
        }

        public void ReleaseNote(double releaseTime, int midiNote)
        {
            OnReleaseNote(releaseTime, midiNote);
        }

        protected virtual void OnPlayNote(double playTime, double duration, int midiNote)
        {
            // empty
        }

        protected virtual void OnReleaseNote(double releaseTime, int midiNote)
        {
            // empty
        }
    }
}
