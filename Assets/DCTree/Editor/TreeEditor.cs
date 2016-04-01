using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DerelictComputer.DCTree
{
    public class TreeEditor : EditorWindow
    {
        private List<NodeWindow> _nodeWindows;

        [MenuItem("Window/DCTree Editor")]
        private static void Init()
        {
            GetWindow<TreeEditor>().Show();
        }

        private void OnEnable()
        {
            Reset();

            // test
            var listOfNodes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(Node).IsAssignableFrom(assemblyType)
                            select assemblyType).ToArray();
            var nodeInfo = new List<NodeInfo>();
            foreach (var node in listOfNodes)
            {
                if (node.IsAbstract)
                {
                    continue;
                }
                Debug.Log(node.FullName);
                nodeInfo.Add(new NodeInfo(node));
                Debug.Log(nodeInfo[nodeInfo.Count-1]);
            }
        }

        private void OnGUI()
        {
            
        }

        private void Reset()
        {
            _nodeWindows = new List<NodeWindow>();
        }
    }
}