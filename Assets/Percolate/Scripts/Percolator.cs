using System;
using DerelictComputer.DCTree;
using UnityEngine;

namespace DerelictComputer
{
    public class Percolator : MonoBehaviour
    {
        [SerializeField] private Metronome _metronome;
        [SerializeField] private OneShot _oneShot;
        [SerializeField, Range(1, 8)] private int _tickDivider = 1;

        private Node _rootNode;
        private int _currentTick;

        private void OnEnable()
        {

            _currentTick = 0;

            _metronome.Ticked += OnTick;
        }

        private void OnDisable()
        {
            _metronome.Ticked -= OnTick;
        }

        private void OnTick(double tickDspTime)
        {
            if (_currentTick++ % _tickDivider == 0)
            {
                _rootNode.Tick(tickDspTime);
            }
        }
    }
}
