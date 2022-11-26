using System.Collections;
using System.Collections.Generic;

namespace EditorSelectionNavigation
{
    public class Recorder<T> : IEnumerable<T>
    {
        private bool _muteRecording;
        private SelectionNode<T> _current;
        private SelectionNode<T> _latest;
        public T Current => _current.Entry;
        public int Count => _latest?.Index ?? 0;

        public void Record(T newEntry)
        {
            var newNode = new SelectionNode<T>()
            {
                Entry = newEntry,
                Previous = _current,
                Index = 1
            };

            if (_current != null)
            {
                _current.Next = newNode;
                newNode.Index += _current.Index;
            }

            _latest = newNode;
            _current = newNode;
        }

        private bool SetCurrent(SelectionNode<T> node)
        {
            if (node == null) return false;
            _current = node;
            return true;
        }

        public bool HasNext()
        {
            return _current?.Next != null;
        }

        public bool HasPrevious()
        {
            return _current?.Previous != null;
        }

        public bool MoveToNext()
        {
            return SetCurrent(_current?.Next);
        }

        public bool MoveToPrevious()
        {
            return SetCurrent(_current?.Previous);
        }

        public void Clear()
        {
            _current = null;
            _latest = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_latest == null) yield break;
            var current = _latest;
            do
            {
                yield return current.Entry;
            } while ((current = current.Previous) != null);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        private class SelectionNode<T>
        {
            public T Entry;
            public SelectionNode<T> Next;
            public SelectionNode<T> Previous;
            public int Index;
        }
    }
}