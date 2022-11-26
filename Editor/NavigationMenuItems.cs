using UnityEditor;

namespace EditorSelectionNavigation
{
    public static class NavigationMenuItems
    {
        
        [MenuItem("Navigate/← Back %[")]
        private static void Back()
        {
            SelectionNavigationUtility.NavigateBack();
        }
        
        [MenuItem("Navigate/← Back %[",true)]
        private static bool BackValidation()
        {
            return SelectionNavigationUtility.CanNavigateBack();
        }

        [MenuItem("Navigate/→ Forward %]")]
        private static void Forth()
        {
            SelectionNavigationUtility.NavigateForward();
        }
        
        [MenuItem("Navigate/→ Forward %]",true)]
        private static bool ForwardValidation()
        {
            return SelectionNavigationUtility.CanNavigateForward();
        }

    }
}