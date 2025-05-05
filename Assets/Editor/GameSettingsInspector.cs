using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Managers;
using Sirenix.OdinInspector.Editor.Internal;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sirenix.OdinInspector.Editor
{
    //
    // Summary:
    //     Access the StaticInspectorWindow from Tools > Odin > Inspector > Static Inspector.
    public class GameSettingsInspector : OdinEditorWindow
    {
        //
        // Summary:
        //     Member filter for access modifiers.
        [Flags]
        public enum AccessModifierFlags
        {
            //
            // Summary:
            //     include public members.
            Public = 2,
            //
            // Summary:
            //     Include Non-public members.
            Private = 4,
            //
            // Summary:
            //     Include both public and non-public members.
            All = 6
        }

        //
        // Summary:
        //     Member filter for member types.
        [Flags]
        public enum MemberTypeFlags
        {
            //
            // Summary:
            //     No members included.
            None = 0,
            //
            // Summary:
            //     Include field members.
            Fields = 1,
            //
            // Summary:
            //     Include property members.
            Properties = 2,
            //
            // Summary:
            //     Include method members.
            Methods = 4,
            //
            // Summary:
            //     Include group members.
            Groups = 8,
            //
            // Summary:
            //     Include members from the base types.
            BaseTypeMembers = 0x10,
            //
            // Summary:
            //     Include members marked with the Obsolete attribute.
            Obsolete = 0x20,
            //
            // Summary:
            //     Include all members except members marked with the Obsolete attribute.
            AllButObsolete = 0x1F
        }

        private static GUIStyle btnStyle;

        private const string TargetTypeAssemblyCategoriesPrefKey = "OdinStaticInspectorWindow.TargetTypeAssemblyCategories";

        private const string MemberTypeFlagsPrefKey = "OdinStaticInspectorWindow.MemberTypeFlags";

        private const string AccessModifierFlagsPrefKey = "OdinStaticInspectorWindow.AccessModifierFlags";

        [SerializeField]
        [HideInInspector]
        private Type targetType;

        [SerializeField]
        [HideInInspector]
        private AssemblyCategory targetTypeFlags;

        [SerializeField]
        [HideInInspector]
        private MemberTypeFlags memberTypes;

        [SerializeField]
        [HideInInspector]
        private AccessModifierFlags accessModifiers;

        [SerializeField]
        [HideInInspector]
        private string showMemberNameFilter;

        [SerializeField]
        [HideInInspector]
        private string searchFilter;

        [NonSerialized]
        private PropertyTree tree;

        [NonSerialized]
        private AccessModifierFlags currAccessModifiers;

        [NonSerialized]
        private MemberTypeFlags currMemberTypes;

        [NonSerialized]
        private int focusSearch;

        private List<Type> cachedGameManagerTypes;

        private void CacheGameManagerTypes()
        {
            cachedGameManagerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttributes(typeof(GameManagerAttribute), inherit: true).Any())
                .ToList();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CacheGameManagerTypes();
        }

        //
        // Summary:
        //     Shows the window.
        public static void ShowWindow()
        {
            InspectType(null);
        }

        [MenuItem("Game Settings/Game Settings")]
        public static void OpenStaticInspectorWindow()
        {
            GameSettingsInspector.ShowWindow();
        }
        
        //
        // Summary:
        //     Opens a new static inspector window for the given type.
        public static GameSettingsInspector InspectType(Type type, AccessModifierFlags? accessModifies = null, MemberTypeFlags? memberTypeFlags = null)
        {
            GameSettingsInspector staticInspectorWindow = ScriptableObject.CreateInstance<GameSettingsInspector>();
            staticInspectorWindow.titleContent = new GUIContent("Game Settings", EditorIcons.MagnifyingGlass.Highlighted);
            staticInspectorWindow.position = GUIHelper.GetEditorWindowRect().AlignCenter(700f, 400f);
            staticInspectorWindow.targetTypeFlags = (AssemblyCategory)EditorPrefs.GetInt("OdinStaticInspectorWindow.TargetTypeAssemblyCategories", 7);
            if (accessModifies.HasValue)
            {
                staticInspectorWindow.accessModifiers = accessModifies.Value;
            }
            else
            {
                staticInspectorWindow.accessModifiers = (AccessModifierFlags)EditorPrefs.GetInt("OdinStaticInspectorWindow.AccessModifierFlags", 6);
            }

            if (memberTypeFlags.HasValue)
            {
                staticInspectorWindow.memberTypes = memberTypeFlags.Value;
            }
            else
            {
                staticInspectorWindow.memberTypes = MemberTypeFlags.Fields;
            }

            staticInspectorWindow.currMemberTypes = staticInspectorWindow.memberTypes;
            staticInspectorWindow.currAccessModifiers = staticInspectorWindow.accessModifiers;
            staticInspectorWindow.focusSearch = 0;
            staticInspectorWindow.targetType = type;
            staticInspectorWindow.Show();
            if (type != null)
            {
                staticInspectorWindow.titleContent = new GUIContent(type.GetNiceName());
            }

            staticInspectorWindow.Repaint();
            return staticInspectorWindow;
        }

        //
        // Summary:
        //     Draws the Odin Editor Window.
        protected override void OnImGUI()
        {
            btnStyle = btnStyle ?? new GUIStyle(EditorStyles.toolbarDropDown);
            btnStyle.fixedHeight = 21f;
            btnStyle.stretchHeight = false;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(150)); // Adjust width as needed
            DrawTypeButtons();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            base.OnImGUI();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTypeButtons()
        {
            foreach (Type type in cachedGameManagerTypes)
            {
                if (GUILayout.Button(type.Name))
                {
                    targetType = type;
                    titleContent = new GUIContent(targetType.GetNiceName()); 

                    // Try to find and inspect corresponding config
                    var configTypeName = type.Name.Replace("Manager", "Config"); // e.g. SeasonManager -> SeasonConfig
                    var configType = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(asm => asm.GetTypes())
                        .FirstOrDefault(t => t.Name == configTypeName && typeof(ScriptableObject).IsAssignableFrom(t));

                    if (configType != null)
                    {
                        var configAsset = AssetDatabase.FindAssets($"t:{configType.Name}")
                            .Select(guid => AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid)))
                            .FirstOrDefault();

                        if (configAsset != null)
                        {
                            Selection.activeObject = configAsset;
                            EditorGUIUtility.PingObject(configAsset);
                        }
                        else
                        {
                            Debug.LogWarning($"Could not find any asset of type {configType.Name} in the project.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Could not resolve config type for manager {type.Name}");
                    }
                }
            }
        }

        private void DrawFirstToolbar()
        {
            GUILayout.Space(1f);
            string text = "       " + ((targetType == null) ? "Select Manager" : targetType.GetNiceFullName()) + "   ";
            Rect rect = GUILayoutUtility.GetRect(0f, 21f, SirenixGUIStyles.ToolbarBackground);
            Rect rect2 = rect.AlignRight(80f);
            Rect rect3 = rect.SetXMax(rect2.xMin);

            //
            //DrawSelectorDropdown(rect3, new GUIContent(text), SelectType, btnStyle);
            //

            if (Event.current.type == EventType.Repaint)
            {
                Texture2D assetThumbnail = GUIHelper.GetAssetThumbnail(null, targetType ?? typeof(int), preferObjectPreviewOverFileIcon: false);
                if (assetThumbnail != null)
                {
                    rect3.x += 8f;
                    GUI.DrawTexture(rect3.AlignLeft(16f).AlignMiddle(16f), assetThumbnail, ScaleMode.ScaleToFit);
                }
            }
        }

        private void DrawSearchField(Rect rect)
        {
            rect = rect.HorizontalPadding(5f).AlignMiddle(16f);
            rect.xMin += 3f;
            searchFilter = SirenixEditorGUI.SearchField(rect, searchFilter, focusSearch++ < 4, "SirenixSearchField" + GetInstanceID());
        }

        //
        // Summary:
        //     Draws the editor for the this.CurrentDrawingTargets[index].
        protected override void DrawEditor(int index)
        {
            DrawTree();
            GUILayout.FlexibleSpace();
        }

        private void DrawTree()
        {
            if (targetType == null)
            {
                if (tree != null)
                {
                    tree.Dispose();
                }

                tree = null;
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                currMemberTypes = memberTypes;
                currAccessModifiers = accessModifiers;
            }

            if (tree == null || tree.TargetType != targetType)
            {
                if (targetType.IsGenericType && !targetType.IsFullyConstructedGenericType())
                {
                    SirenixEditorGUI.ErrorMessageBox("Cannot statically inspect generic type definitions");
                    return;
                }

                if (tree != null)
                {
                    tree.Dispose();
                }

                tree = PropertyTree.CreateStatic(targetType);
            }

            bool flag = (currMemberTypes & MemberTypeFlags.Obsolete) == MemberTypeFlags.Obsolete;
            PropertyContext<bool> global = tree.RootProperty.Context.GetGlobal("ALLOW_OBSOLETE_STATIC_MEMBERS", defaultValue: false);
            if (global.Value != flag)
            {
                global.Value = flag;
                tree.RootProperty.RefreshSetup();
            }

            tree.BeginDraw(withUndo: false);
            bool flag2 = true;
            if (tree.AllowSearchFiltering && tree.RootProperty.Attributes.HasAttribute<SearchableAttribute>())
            {
                SearchableAttribute attribute = tree.RootProperty.GetAttribute<SearchableAttribute>();
                if (attribute.Recursive)
                {
                    SirenixEditorGUI.WarningMessageBox("This type has been marked as recursively searchable. Be *CAREFUL* with using this search - recursively searching a static inspector can be *very dangerous* and can lead to freezes, crashes or other nasty errors if the static inspector search ends up recursing deeply into, for example, the .NET runtime internals, which would result in recursively searching through hundreds of thousands to millions of internal properties.");
                }

                if (tree.DrawSearch())
                {
                    flag2 = false;
                }
            }

            if (flag2)
            {
                foreach (InspectorProperty item in tree.EnumerateTree(includeChildren: false))
                {
                    if (DrawProperty(item))
                    {
                        if (item.Info.PropertyType != PropertyType.Group && item.Info.GetMemberInfo() != null && item.Info.GetMemberInfo().DeclaringType != targetType)
                        {
                            item.Draw(new GUIContent(item.Info.GetMemberInfo().DeclaringType.GetNiceName() + " -> " + item.NiceName));
                        }
                        else
                        {
                            item.Draw();
                        }
                    }
                    else
                    {
                        item.Update();
                    }
                }
            }

            tree.EndDraw();
        }

        private bool DrawProperty(InspectorProperty property)
        {
            if (!string.IsNullOrEmpty(searchFilter) && !Utilities.StringExtensions.Contains(property.NiceName.Replace(" ", ""), searchFilter.Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            if (property.Info.PropertyType == PropertyType.Group)
            {
                return (currMemberTypes & MemberTypeFlags.Groups) == MemberTypeFlags.Groups;
            }

            MemberInfo memberInfo = property.Info.GetMemberInfo();
            if (memberInfo != null)
            {
                if ((currMemberTypes & MemberTypeFlags.BaseTypeMembers) != MemberTypeFlags.BaseTypeMembers && memberInfo.DeclaringType != null && memberInfo.DeclaringType != targetType)
                {
                    return false;
                }

                bool flag = (currAccessModifiers & AccessModifierFlags.Public) == AccessModifierFlags.Public;
                bool flag2 = (currAccessModifiers & AccessModifierFlags.Private) == AccessModifierFlags.Private;
                bool flag3 = (currMemberTypes & MemberTypeFlags.Fields) == MemberTypeFlags.Fields;
                bool flag4 = (currMemberTypes & MemberTypeFlags.Properties) == MemberTypeFlags.Properties;
                if (!flag || !flag2)
                {
                    bool flag5 = true;
                    FieldInfo fieldInfo = memberInfo as FieldInfo;
                    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                    MethodInfo methodInfo = memberInfo as MethodInfo;
                    if (fieldInfo != null)
                    {
                        flag5 = fieldInfo.IsPublic;
                    }
                    else if (propertyInfo != null)
                    {
                        MethodInfo getMethod = propertyInfo.GetGetMethod();
                        flag5 = getMethod != null && getMethod.IsPublic;
                    }
                    else if (methodInfo != null)
                    {
                        flag5 = methodInfo.IsPublic;
                    }

                    if (flag5 && !flag)
                    {
                        return false;
                    }

                    if (!flag5 && !flag2)
                    {
                        return false;
                    }
                }

                if (memberInfo is FieldInfo && !flag3)
                {
                    return false;
                }

                if (memberInfo is PropertyInfo && !flag4)
                {
                    return false;
                }
            }

            if (property.Info.PropertyType == PropertyType.Method && (currMemberTypes & MemberTypeFlags.Methods) != MemberTypeFlags.Methods)
            {
                return false;
            }

            return true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (tree != null)
            {
                tree.Dispose();
            }
        }
    }
}
