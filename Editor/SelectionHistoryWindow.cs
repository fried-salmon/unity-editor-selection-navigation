using System.Linq;
using EditorSelectionNavigation;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorNavigation
{
    public class SelectionHistoryWindow : EditorWindow
    {
        private AnimBool settingAnimation;
        private bool settingExpanded;
        private AnimBool clearAnimation;
        private bool historyVisible = true;
        private string iconPrefix => EditorGUIUtility.isProSkin ? "d_" : "";

        public static void Open()
        {
            SelectionHistoryWindow window = GetWindow<SelectionHistoryWindow> ();

            //Options
            window.autoRepaintOnSceneChange = true;
            window.titleContent.image = EditorGUIUtility.IconContent(EditorGUIUtility.isProSkin
                ? "d_UnityEditor.SceneHierarchyWindow"
                : "UnityEditor.SceneHierarchyWindow").image;
            window.titleContent.text = " Selection History";
            window.wantsMouseMove = true;

            //Show
            window.Show();
        }


        private void OnEnable()
        {
            settingAnimation = new AnimBool(false);
            settingAnimation.valueChanged.AddListener(this.Repaint);
            settingAnimation.speed = 4f;
            clearAnimation = new AnimBool(false);
            clearAnimation.valueChanged.AddListener(this.Repaint);
            clearAnimation.speed = settingAnimation.speed;
        }

        private Vector2 scrollPos;


        private void OnGUI()
        {

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(SelectionNavigationUtility.Recorder.Count == 0))
                {
                    using (new EditorGUI.DisabledScope(SelectionNavigationUtility.Recorder.HasPrevious()))
                    {
                        if (GUILayout.Button(
                                new GUIContent(EditorGUIUtility.IconContent(iconPrefix + "back@2x").image,
                                    "Select previous (Left bracket key)"), EditorStyles.miniButtonLeft,
                                GUILayout.Height(20f),
                                GUILayout.Width(30f)))
                        {
                            SelectionNavigationUtility.NavigateBack();
                        }
                    }

                    using (new EditorGUI.DisabledScope(SelectionNavigationUtility.Recorder.HasNext()))
                    {
                        if (GUILayout.Button(
                                new GUIContent(EditorGUIUtility.IconContent(iconPrefix + "forward@2x").image,
                                    "Select next (Right bracket key)"), EditorStyles.miniButtonRight,
                                GUILayout.Height(20),
                                GUILayout.Width(30f)))
                        {
                            SelectionNavigationUtility.NavigateForward();
                        }
                    }

                    if (GUILayout.Button(
                            new GUIContent(EditorGUIUtility.IconContent(iconPrefix + "TreeEditor.Trash").image,
                                "Clear history"), EditorStyles.miniButton))
                    {
                        historyVisible = false;
                    }
                }

                GUILayout.FlexibleSpace();

                settingExpanded = GUILayout.Toggle(settingExpanded,
                    new GUIContent(EditorGUIUtility.IconContent(iconPrefix + "Settings").image, "Edit settings"),
                    EditorStyles.miniButtonMid);
                settingAnimation.target = settingExpanded;
            }

            if (EditorGUILayout.BeginFadeGroup(settingAnimation.faded))
            {
                EditorGUILayout.Space();

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Record", EditorStyles.boldLabel, GUILayout.Width(100f));
                    Preferences.RecordHierarchy = EditorGUILayout.ToggleLeft("Hierarchy", Preferences.RecordHierarchy, GUILayout.MaxWidth(80f));
                    Preferences.RecordProject = EditorGUILayout.ToggleLeft("Project window", Preferences.RecordProject);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("History size", EditorStyles.boldLabel, GUILayout.Width(100f));
                    Preferences.MaxHistorySize = EditorGUILayout.IntField(Preferences.MaxHistorySize, GUILayout.MaxWidth(40f));
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndFadeGroup();

            clearAnimation.target = !historyVisible;

            EditorGUILayout.LabelField(SelectionNavigationUtility.Recorder.Count.ToString());
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox,
                GUILayout.MaxHeight(this.maxSize.y - 20f));
            {
                EditorGUILayout.BeginFadeGroup(1f - clearAnimation.faded);
                var i = 0;
                foreach (var current in SelectionNavigationUtility.Recorder)
                {
                    i++;
                    DrawEntryRow(i, current);
                }

                EditorGUILayout.EndFadeGroup();
            }
            EditorGUILayout.EndScrollView();

            //Once the list is collapse, clear the collection
            if (clearAnimation.faded >= 1f)
            {
                SelectionNavigationUtility.Recorder.Clear();
            }

            //Reset
            if (SelectionNavigationUtility.Recorder.Count == 0) historyVisible = true;
        }


        private void DrawEntryRow(int i, SelectionEntry current)
        {
            var rect = EditorGUILayout.BeginHorizontal();
            
            GUI.color = i % 2 == 0
                ? Color.grey * (EditorGUIUtility.isProSkin ? 1f : 1.7f)
                : Color.grey * (EditorGUIUtility.isProSkin ? 1.05f : 1.66f);

            //Hover color
            if (rect.Contains(Event.current.mousePosition) || SelectionNavigationUtility.Recorder.Current == (current))
            {
                GUI.color = EditorGUIUtility.isProSkin ? Color.grey * 1.1f : Color.grey * 1.5f;
            }

            //Selection outline
            if (SelectionNavigationUtility.Recorder.Current == (current))
            {
                Rect outline = rect;
                outline.x -= 1;
                outline.y -= 1;
                outline.width += 2;
                outline.height += 2;
                EditorGUI.DrawRect(outline, EditorGUIUtility.isProSkin ? Color.gray * 1.5f : Color.gray);
            }

            //Background
            EditorGUI.DrawRect(rect, GUI.color);

            GUI.color = Color.white;
            GUI.backgroundColor = Color.white;
            if (current.IsValid)
            {
                var firstObject = current.objects[0];
                var firstObjectName = firstObject.name;
                var label = current.objects.Length > 1
                    ? $"{firstObjectName}... ({current.objects.Length.ToString()})"
                    : $"{firstObjectName}";
                
                if (GUILayout.Button(new GUIContent(label,
                            EditorGUIUtility.ObjectContent(firstObject, firstObject.GetType()).image),
                        EditorStyles.label, GUILayout.MaxHeight(17f)))
                {
                    //  SetSelection(current);
                }
            }
            else
            {
                GUILayout.Label("INVALID");
            }
            

            EditorGUILayout.EndHorizontal();
        }
    }
}