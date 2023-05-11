using GridSystem.Partitioning;
using UnityEditor;
using UnityEngine;

namespace GridSystem 
{
	[CustomEditor(typeof(CellSpacePartitioning))]
    public class CellPartitioningEditor : Editor 
	{
		private CellSpacePartitioning _csp;

		private void OnEnable()
		{
			_csp = (CellSpacePartitioning)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Generate Partitions", GUILayout.Width(250)))
				{
					_csp.GeneratePartitions();
					EditorUtility.SetDirty(_csp);
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}
	}
}
