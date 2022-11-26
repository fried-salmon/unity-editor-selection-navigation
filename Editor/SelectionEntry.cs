using System.Linq;
using UnityEngine;

namespace EditorSelectionNavigation
{
    public class SelectionEntry
    {
        public Object[] objects;
        public bool IsValid => objects.All(o => o != null);
    }
}