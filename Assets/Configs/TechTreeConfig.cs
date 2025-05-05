using Sirenix.OdinInspector;
using TechTree.ScriptableObjects;
using UnityEngine;

#if UNITY_EDITOR
using System.Reflection;
using System;
using UnityEditor;
#endif

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Tech Tree Manager Config")]
    public class TechTreeConfig : Config<TechTreeConfig>
    {
        #if UNITY_EDITOR
        [Button("Open TechTree Editor")]
        private void OpenTechTreeEditor()
        {
            // Use reflection to access the StartWindowLoad method of TechTreeEditorWindow
            var editorWindowType = Type.GetType("TechTree.Windows.TechTreeEditorWindow, Assembly-CSharp-Editor");
            var method = editorWindowType?.GetMethod("StartWindowLoad", BindingFlags.Public | BindingFlags.Static);
            method?.Invoke(null, new object[] { techTreeSO });
        }
        #endif

        public TechTree_SO techTreeSO;
    }
}