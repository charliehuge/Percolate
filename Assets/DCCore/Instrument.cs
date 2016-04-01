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

        protected abstract void OnPlayNote(double playTime, double duration, int midiNote);
    }
}
