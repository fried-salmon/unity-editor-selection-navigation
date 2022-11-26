using UnityEditor;
using UnityEngine;

namespace EditorSelectionNavigation
{
    [InitializeOnLoad]
    public static class SelectionNavigationUtility
    {
        public static readonly Recorder<SelectionEntry> Recorder = new Recorder<SelectionEntry>();
        private static bool _muteRecording;
        
        static SelectionNavigationUtility()
        {
            Selection.selectionChanged = OnSelectionChanged;
        }

        private static void OnSelectionChanged()
        {
            if (_muteRecording)
            {
                _muteRecording = false;
                return;
            }

            if (Selection.objects.Length == 0)
            {
                return;
            }
            Recorder.Record(new SelectionEntry()
            {
                objects = Selection.objects
            });
        }

        public static void NavigateForward()
        {
            if (Recorder.MoveToNext())
            {
                ApplySelectionEntry(Recorder.Current);
            }
        }
        
        public static bool CanNavigateForward()
        {
            return Recorder.HasNext();
        }

        public static void NavigateBack()
        {
            if (Recorder.MoveToPrevious())
            {
                ApplySelectionEntry(Recorder.Current);
            }
        }
        
        public static bool CanNavigateBack()
        {
            return Recorder.HasPrevious();
        }
        
        private static void ApplySelectionEntry(SelectionEntry entry)
        {
            _muteRecording = true;
            if (entry.IsValid)
            {
                Selection.objects = entry.objects;
            }
        }
    }
}