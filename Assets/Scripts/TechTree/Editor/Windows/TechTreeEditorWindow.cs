using System;
using System.IO;
using TechTree.ScriptableObjects;
using TechTree.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace TechTree.Windows
{
	public class TechTreeEditorWindow : EditorWindow
	{
		private const string DEFAULT_FILE_NAME = "TechTreeFileName";
		private static TextField _fileNameTextField;
		private Button _saveButton;
		private VisualElement _mainElement;
		private VisualElement _sideBarElement;
		private TechTreeGraphView _graphView;
		private Button _miniMapButton;

		[MenuItem("Tools/Tech Tree Editor")]
		public static void StartWindow()
		{
			TechTreeEditorWindow wnd = GetWindow<TechTreeEditorWindow>("Tech Tree Graph");
		}

		public static void StartWindowLoad(TechTree_SO techTree = null)
		{
			TechTreeEditorWindow wnd = GetWindow<TechTreeEditorWindow>("Tech Tree Graph");

			if (techTree != null)
				wnd.LoadTechTree(techTree);
		}

		public void EnableSaving()
		{
			_saveButton.SetEnabled(true);
		}

		public void DisableSaving()
		{
			_saveButton.SetEnabled(false);
		}

		private void CreateGUI()
		{
			CreateMainElement();
			AddGraphView();
			//CreateSideBar();
			AddToolbar();
			AddStyles();
		}

		private void AddToolbar()
		{
			Toolbar toolbar = new Toolbar();

			_fileNameTextField = TechTreeUtilities.CreateTextField(DEFAULT_FILE_NAME, "File Name:", callback =>
			{
				_fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
			});

			_saveButton = TechTreeUtilities.CreateButton("Save", () => Save());

			Button loadButton = TechTreeUtilities.CreateButton("Load", () => Load());
			Button clearButton = TechTreeUtilities.CreateButton("Clear", () => Clear());
			Button resetButton = TechTreeUtilities.CreateButton("Reset", () => ResetGraph());
			_miniMapButton = TechTreeUtilities.CreateButton("Minimap", () => ToggleMiniMap());

			toolbar.Add(_fileNameTextField);
			toolbar.Add(_saveButton);
			toolbar.Add(loadButton);
			toolbar.Add(clearButton);
			toolbar.Add(resetButton);
			toolbar.Add(_miniMapButton);
			toolbar.AddStyleSheets("TechnologyTreeSystem/TechnologyTreeToolbarStyles.uss");
			rootVisualElement.Add(toolbar);
		}

		// Toolbar actions
		private void Save()
		{
			if (string.IsNullOrEmpty(_fileNameTextField.value))
			{
				EditorUtility.DisplayDialog("Invalid FileName", "Hear me out, you need ensure the file name you've typed in is valid.", "Poggers");
				return;
			}

			TechTreeIOUtility.Initialize(_graphView, _fileNameTextField.value);
			TechTreeIOUtility.Save();
		}

		private void Load()
		{
			string filePath = EditorUtility.OpenFilePanel("Technology Tree Graphs", "Assets/Scripts/TechTree/Editor/Graphs", "asset");

			if (string.IsNullOrEmpty(filePath))
				return;

			Clear();

			TechTreeIOUtility.Initialize(_graphView, Path.GetFileNameWithoutExtension(filePath));

			TechTreeIOUtility.Load();
		}
        public void LoadTechTree(TechTree_SO techTree)
        {
            if (techTree == null)
            {
                Debug.LogError("TechTree_SO is null. Cannot load tech tree.");
                return;
            }

            // Convert the TechTree_SO to a path
            string assetPath = AssetDatabase.GetAssetPath(techTree);

            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Could not find asset path for TechTree_SO.");
                return;
            }

            // Construct the full path with the expected format
            string directoryPath = Path.Combine(Application.dataPath, "Scripts/TechTree/Editor/Graphs");
            string fullPath = Path.Combine(directoryPath, Path.GetFileNameWithoutExtension(assetPath) + "Graph.asset");

            // Normalize the path to use forward slashes
            fullPath = fullPath.Replace("\\", "/");

            Clear();

            TechTreeIOUtility.Initialize(_graphView, Path.GetFileNameWithoutExtension(fullPath));

            TechTreeIOUtility.Load();
        }

        private void Clear()
		{
			_graphView.ClearGraph();
		}

		private void ResetGraph()
		{
			Clear();
			UpdateFileName(DEFAULT_FILE_NAME);
		}

		private void ToggleMiniMap()
		{
			_graphView.ToggleMiniMap();
			_miniMapButton.ToggleInClassList("techtree-toolbar__button__selected");
		}

		public static void UpdateFileName(string newFileName)
		{
			_fileNameTextField.value = newFileName;
		}

		private void CreateMainElement()
		{
			_mainElement = new VisualElement();
			_mainElement.StretchToParentSize();
			_mainElement.AddStyleSheets("TechnologyTreeSystem/TechnologyTreeStyles.uss");
			_mainElement.AddClasses("techtree__main-element-container");
			rootVisualElement.Add(_mainElement);
		}

		private void CreateSideBar()
		{
			VisualElement sideBarElement = new VisualElement();
			sideBarElement.style.backgroundColor = new Color(32f / 255f, 32f / 255f, 32f / 255f, 1);
			sideBarElement.AddClasses("techtree__sidebar-element-container");

			Label sideBarTitle = new Label("Inspector");
			sideBarTitle.style.height = 32;
			sideBarTitle.style.color = Color.white;
			sideBarTitle.style.paddingTop = 50;
			sideBarElement.Add(sideBarTitle);
			_mainElement.Add(sideBarElement);
		}

		private void AddGraphView()
		{
			_graphView = new TechTreeGraphView(this);
			_graphView.AddClasses("techtree__graph-element-container");
			_mainElement.Add(_graphView);
		}

		private void AddStyles()
		{
			rootVisualElement.AddStyleSheets("TechnologyTreeSystem/TechnologyTreeVariables.uss");
		}
	}
}