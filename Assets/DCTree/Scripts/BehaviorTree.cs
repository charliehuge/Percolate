using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace DerelictComputer.DCTree
{
    [Serializable]
    public class BehaviorTree
    {
        [SerializeField] private List<SerializableNode> _serializableNodes = new List<SerializableNode>();

        private Node _rootNode;

        public Result Tick(double tickDspTime)
        {
            if (_rootNode == null)
            {
                return Result.Failure;
            }

            var result = _rootNode.Tick(tickDspTime);

            if (result != Result.Running)
            {
                _rootNode.Reset();
            }

            return result;
        }

        public static BehaviorTree LoadForRuntime(TextAsset json, Instrument targetInstrument, Blackboard blackboard)
        {
            BehaviorTree tree = JsonUtility.FromJson<BehaviorTree>(json.text);
            tree.CreateRuntimeTree(targetInstrument, blackboard);
            return tree;
        }

#if UNITY_EDITOR
        public static List<SerializableNode> LoadForEditor(string path)
        {
            var json = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            if (json == null)
            {
                Debug.LogError("File is not a text asset, or doesn't exist at " + path);
                return null;
            }

            var tree = JsonUtility.FromJson<BehaviorTree>(json.text);
            return tree._serializableNodes;
        }
#endif

#if UNITY_EDITOR
        public static void Save(string path, List<SerializableNode> nodes)
        {
            BehaviorTree tree = new BehaviorTree();
            tree._serializableNodes = nodes;
            File.WriteAllText(path, JsonUtility.ToJson(tree, true));
            AssetDatabase.Refresh();
        }
#endif

        private void CreateRuntimeTree(Instrument targetInstrument, Blackboard blackboard)
        {
            if (_serializableNodes.Count == 0)
            {
                return;
            }

            int index = 0;
            _rootNode = CreateRuntimeNodes(ref index, targetInstrument, blackboard);
        }

        private Node CreateRuntimeNodes(ref int index, Instrument targetInstrument, Blackboard blackboard)
        {
            var sNode = _serializableNodes[index];
            Node[] children = new Node[sNode.ChildCount];

            for (int i = 0; i < sNode.ChildCount; i++)
            {
                ++index;
                children[i] = CreateRuntimeNodes(ref index, targetInstrument, blackboard);
            }

            return InstantiateRuntimeNode(sNode, children, targetInstrument, blackboard);
        }

        /// <summary>
        /// AOT-safe instantiate, instead of using Activator
        /// </summary>
        /// <param name="sNode"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        private Node InstantiateRuntimeNode(SerializableNode sNode, Node[] children, Instrument targetInstrument, Blackboard blackboard)
        {
            Type type = Type.GetType(sNode.AssemblyQualifiedTypeName);

            if (type == null)
            {
                Debug.LogError("No type for " + sNode.AssemblyQualifiedTypeName);
                return null;
            }

            if (type == typeof(Charger))
            {
                if (children.Length == 0)
                {
                    Debug.LogError("Need to specify a child for " + sNode.AssemblyQualifiedTypeName);
                    return null;
                }

                return new Charger(sNode, children[0]);
            }
            if (type == typeof(FiniteRepeater))
            {
                if (children.Length == 0)
                {
                    Debug.LogError("Need to specify a child for " + sNode.AssemblyQualifiedTypeName);
                    return null;
                }

                return new FiniteRepeater(sNode, children[0]);
            }
            if (type == typeof(Inverter))
            {
                if (children.Length == 0)
                {
                    Debug.LogError("Need to specify a child for " + sNode.AssemblyQualifiedTypeName);
                    return null;
                }

                return new Inverter(sNode, children[0]);
            }
            if (type == typeof(PlayNote))
            {
                return new PlayNote(sNode, targetInstrument);
            }
            if (type == typeof(Repeater))
            {
                if (children.Length == 0)
                {
                    Debug.LogError("Need to specify a child for " + sNode.AssemblyQualifiedTypeName);
                    return null;
                }

                return new Repeater(sNode, children[0]);
            }
            if (type == typeof(RepeatUntilFailure))
            {
                if (children.Length == 0)
                {
                    Debug.LogError("Need to specify a child for " + sNode.AssemblyQualifiedTypeName);
                    return null;
                }

                return new RepeatUntilFailure(sNode, children[0]);
            }
            if (type == typeof(RepeatUntilSuccess))
            {
                if (children.Length == 0)
                {
                    Debug.LogError("Need to specify a child for " + sNode.AssemblyQualifiedTypeName);
                    return null;
                }

                return new RepeatUntilSuccess(sNode, children[0]);
            }
            if (type == typeof(Selector))
            {
                return new Selector(sNode, children);
            }
            if (type == typeof(Sequence))
            {
                return new Sequence(sNode, children);
            }
            if (type == typeof(Succeeder))
            {
                if (children.Length == 0)
                {
                    Debug.LogError("Need to specify a child for " + sNode.AssemblyQualifiedTypeName);
                    return null;
                }

                return new Succeeder(sNode, children[0]);
            }
            if (type == typeof (BlackboardFloatThreshold))
            {
                if (children.Length == 0)
                {
                    Debug.LogError("Need to specify a child for " + sNode.AssemblyQualifiedTypeName);
                    return null;
                }

                return new BlackboardFloatThreshold(sNode, children[0], blackboard);
            }

            Debug.LogError("Unsupported Node: " + sNode.AssemblyQualifiedTypeName);
            return null;
        }
    }
}
