using System;
using System.Collections;
using System.Collections.Generic;

namespace EditorSelectionNavigation
{
    public class Recorder<T> : IEnumerable<T>
    {
        private EntryNode _current;
        private EntryNode _latest;
        public T Current => IsEmpty ? throw new InvalidOperationException("Recorder is empty") : _current.Entry;
        public int Count => _latest?.Index ?? 0;
        public bool IsEmpty => _current == null;

        public void Record(T newEntry)
        {
            var newNode = new EntryNode()
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

        private bool SetCurrent(EntryNode node)
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
        
        private class EntryNode
        {
            public T Entry;
            public EntryNode Next;
            public EntryNode Previous;
            public int Index;
        }
    }
}
