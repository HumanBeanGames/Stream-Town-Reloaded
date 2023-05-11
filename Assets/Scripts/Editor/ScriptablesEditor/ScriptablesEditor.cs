using Globals;
using Scriptables;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptablesEditor
{
    public class ScriptablesEditor : EditorWindow
    {
        private const string SCRIPTABLES_FOLDER_DIRECTORY = "Assets/ScriptableObjects/";
        private const int SIDEBAR_WIDTH = 300;

        private static Color _sidebarColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        private static Color _titleBackgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
        private static Color _mainWindowStyleColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        private static Color _insetStyleColor = new Color(0.12f, 0.12f, 0.12f, 1f);
        private static Color _mainWindowStyleBorderColor = new Color(0.5f, 0.5f, 0.25f, 1f);

        // Scroll positions
        private Vector2 _sidebarScrollPosition;
        private Vector2 _mainLeftContextScrollPos;
        private Vector2 _mainRightContextScrollPos;

        private Dictionary<string, string[]> _fileNames;
        private string[] _folderNames;

        private int _folderSelected = 0;
        private int[] _itemSelections;

        // Styles
        GUIStyle _sidebarStyle;
        GUIStyle _mainWindowStyle;
        GUIStyle _insetWindowStyle;
        GUIStyle _buttonStyle;
        GUIStyle _titleStyle;

        private ScriptableObject _selectedObject;

        private static bool _loaded = false;

        [MenuItem("Tools/Scriptables Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(ScriptablesEditor));
            _loaded = false;
        }

        private void OnGUI()
        {
            if (!_loaded)
            {
                _loaded = true;
                UpdateDirectoriesAndFiles();
            }
            SetupStyles();
            OnSelectionChanged();
            //Reload Assets
            // Set up styles data

            // Draw all context layouts.
            EditorGUILayout.BeginVertical();
            {
                TitleContext();
                EditorUtils.DrawGUILine(new Color(0.5f, 0.5f, 0.5f, 1));
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical(GUILayout.Width(SIDEBAR_WIDTH));
                    {
                        SidebarContext();
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(8);
                    EditorGUILayout.BeginVertical();
                    {
                        MainContext();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void TitleContext()
        {
            EditorGUILayout.BeginHorizontal(_titleStyle);
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("Scriptables Editor");
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void MainContext()
        {
            EditorGUILayout.BeginVertical(_mainWindowStyle);
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Width((Screen.width - SIDEBAR_WIDTH)), GUILayout.MinWidth(400), GUILayout.ExpandHeight(true));
                {
                    // GUILayout.Space(5);

                    //Left Side Scrollable Context
                    MainLeftContext();

                    GUILayout.Space(12);
                    //Right Side Scrollable Context
                    MainRightContext();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void MainLeftContext()
        {
            float width = (Screen.width - SIDEBAR_WIDTH) / 2 - 10;
            _mainLeftContextScrollPos = GUILayout.BeginScrollView(_mainLeftContextScrollPos, _insetWindowStyle, GUILayout.Width(width));
            {
                //Draw Main Data for Scriptable, driven by other classes
                if (!_selectedObject)
                    return;

                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal(GUILayout.Width(width-10));
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical(_mainWindowStyle, GUILayout.Width(width - 20));

                if (_selectedObject.GetType() == typeof(BuildingDataScriptable))
                    BuildingScriptablesEditor.DrawBuildingScriptableData(_selectedObject);

                EditorGUILayout.EndVertical();
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            GUILayout.EndScrollView();
        }

        private void MainRightContext()
        {
            float width = (Screen.width - SIDEBAR_WIDTH) / 2 - 10;
            _mainRightContextScrollPos = GUILayout.BeginScrollView(_mainRightContextScrollPos, _insetWindowStyle, GUILayout.Width(width));
            {
                // Draw Secondary Data for scriptable
                if (!_selectedObject)
                    return;

                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal(GUILayout.Width(width - 10));
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical(_mainWindowStyle, GUILayout.Width(width - 20));

                if (_selectedObject.GetType() == typeof(BuildingDataScriptable))
                    BuildingScriptablesEditor.DrawBuildingScriptableData(_selectedObject);

                EditorGUILayout.EndVertical();
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
            GUILayout.EndScrollView();
        }

        private void SidebarContext()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(SIDEBAR_WIDTH));
            {
                // Buttons
                SidebarButtonsContext(25);

                // Scroll View
                SidebarScrollviewContext();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the top most context buttons displaying all types of scriptables found in the scriptables directory.
        /// </summary>
        /// <param name="height"></param>
        private void SidebarButtonsContext(int height)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(height));
            {
                EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(height));
                {
                    _folderSelected = GUILayout.SelectionGrid(_folderSelected, _folderNames, 3, GUILayout.MaxWidth(SIDEBAR_WIDTH), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws the scroll view context in the sidebar, displaying the scriptable objects of the selected type.
        /// </summary>
        private void SidebarScrollviewContext()
        {
            //Scroll view
            _sidebarScrollPosition = EditorGUILayout.BeginScrollView(_sidebarScrollPosition, _sidebarStyle, GUILayout.Width(SIDEBAR_WIDTH));
            {
                _itemSelections[_folderSelected] = GUILayout.SelectionGrid(_itemSelections[_folderSelected], _fileNames[_folderNames[_folderSelected]], 1, GUILayout.MaxWidth(SIDEBAR_WIDTH), GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndScrollView();

            //Add and Remove Buttons
            EditorGUILayout.BeginHorizontal(GUILayout.Height(40));
            {
                if (GUILayout.Button("Add"))
                {
                    // Create new scriptable Object
                    // Reload data
                }
                if (GUILayout.Button("Delete"))
                {
                    // Remove scriptable Object
                    // Reload data
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Sets up all of the styles used for the editor.
        /// </summary>
        private void SetupStyles()
        {
            // Main Skin.
            GUISkin skin = CreateInstance("GUISkin") as GUISkin;
            skin.box.normal.background = EditorUtils.MakeTexture(1, 1, _mainWindowStyleColor);

            // Border Style.
            RectOffset border;
            border = skin.box.border;

            // Main Window Style.
            _mainWindowStyle = new GUIStyle();
            _mainWindowStyle.normal.background = EditorUtils.MakeTexture(1, 1, _mainWindowStyleColor);

            _mainWindowStyle.border = border;

            // Sidebar Style.
            _sidebarStyle = new GUIStyle();
            _sidebarStyle.normal.background = EditorUtils.MakeTexture(1, 1, _sidebarColor);

            // Inset Window Style.
            _insetWindowStyle = new GUIStyle();
            _insetWindowStyle.normal.background = EditorUtils.MakeTexture(1, 1, _insetStyleColor);

            _insetWindowStyle.border = border;

            // Button Style.
            _buttonStyle = new GUIStyle();
            _buttonStyle.margin.left = 2;
            _buttonStyle.margin.right = 2;
            _buttonStyle.margin.top = 2;
            _buttonStyle.margin.bottom = 2;

            // Title Style.
            _titleStyle = new GUIStyle();
            _titleStyle.normal.background = EditorUtils.MakeTexture(1, 1, _titleBackgroundColor);
        }

        /// <summary>
        /// Updates all of the directories and their files, filling in the data required to view the scriptables.
        /// </summary>
        private void UpdateDirectoriesAndFiles()
        {
            _folderNames = EditorUtils.GetFolderNamesInPath(SCRIPTABLES_FOLDER_DIRECTORY);

            _fileNames = new Dictionary<string, string[]>();

            for (int i = 0; i < _folderNames.Length; i++)
            {
                _fileNames[_folderNames[i]] = EditorUtils.GetFileNamesInPath(SCRIPTABLES_FOLDER_DIRECTORY + _folderNames[i]);
            }

            _itemSelections = new int[_fileNames.Count];

            for (int i = 0; i < _itemSelections.Length; i++)
            {
                _itemSelections[i] = 0;
            }
        }

        /// <summary>
        /// Called when the current selection has changed.
        /// </summary>
        private void OnSelectionChanged()
        {
            _selectedObject = null;
            if (EditorUtils.GetAssetFile(out ScriptableObjectAssetData<ScriptableObject> asset, _fileNames[_folderNames[_folderSelected]][_itemSelections[_folderSelected]]))
            {
                _selectedObject = asset.Asset;
            }
            else
                Debug.LogError($"Lol Shit went wrong aye: {_fileNames[_folderNames[_folderSelected]][_itemSelections[_folderSelected]]}");

        }
    }
}