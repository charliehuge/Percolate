using System;
using DerelictComputer.DCTree;
using UnityEngine;

namespace DerelictComputer
{
    public class Percolator : MonoBehaviour
    {
        [SerializeField] private Metronome _metronome;
        [SerializeField] private Instrument _instrument;
        [SerializeField, Range(1, 8)] private int _tickDivider = 1;
        [SerializeField] private TextAsset _dcTreeJson;
        [SerializeField] private Blackboard _blackboard;

        private BehaviorTree _tree;
        private int _currentTick;

        private void OnEnable()
        {
            _tree = BehaviorTree.LoadForRuntime(_dcTreeJson, _instrument, _blackboard);

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
                _tree.Tick(tickDspTime);
            }
        }
    }
}
