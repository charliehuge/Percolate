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
            _rootNode = new Repeater(new Sequence(new Node[]
            {
                new PlayNote(_oneShot, 0),
                new PlayNote(_oneShot, 12),
                new PlayNote(_oneShot, 7),
                new Charger(new Sequence(new Node[]
                {
                    new FiniteRepeater(new PlayNote(_oneShot, 17), UnityEngine.Random.Range(1, 8)),
                    new PlayNote(_oneShot, 19),
                    new Charger(new Sequence(new Node[]
                    {
                        new PlayNote(_oneShot, 7),
                        new PlayNote(_oneShot, 19),
                    }), (uint) UnityEngine.Random.Range(1, 8)),
                })
                    , (uint) UnityEngine.Random.Range(1, 4)),
            }));

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
