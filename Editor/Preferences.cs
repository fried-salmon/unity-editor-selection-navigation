using UnityEditor;

namespace EditorNavigation
{
    public static class Preferences
    {
        public static bool RecordHierarchy
        {
            get => EditorPrefs.GetBool(PlayerSettings.productName + "_EditorNav_RecordHierachy", true);
            set => EditorPrefs.SetBool(PlayerSettings.productName + "_EditorNav_RecordHierachy", value);
        }

        public static bool RecordProject
        {
            get => EditorPrefs.GetBool(PlayerSettings.productName + "_EditorNav_RecordProject", true);
            set => EditorPrefs.SetBool(PlayerSettings.productName + "_EditorNav_RecordProject", value);
        }

        public static int MaxHistorySize
        {
            get => EditorPrefs.GetInt(PlayerSettings.productName + "_EditorNav_MaxHistorySize", 50);
            set => EditorPrefs.SetInt(PlayerSettings.productName + "_EditorNav_MaxHistorySize", value);
        }
    }
}