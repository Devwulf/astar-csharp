using System;
using System.Collections;
using System.Collections.Generic;

namespace AStar.Utils
{
    /// <summary>
    /// Ascending: 1, 2, 3, 4...
    /// Descending: 9, 8, 7, 6...
    /// </summary>
    public enum SortedBy
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// A doubly linked list that automatically sorts any added value into it.
    /// </summary>
    /// <typeparam name="T">Any type that implements IComparable so its value can be compared to others.</typeparam>
    public class SortedLinkedList<T> : IEnumerable<T> where T : class, IComparable<T>
    {
        // Represents a node in a linked list
        public class Node<TType>
        {
            public TType Value;
            public Node<TType> Prev;
            public Node<TType> Next;

            public Node(TType value)
            {
                Value = value;
            }
        }

        public int Count { get; private set; }

        // Dummy node, prev points to back, next points to front
        private readonly Node<T> _head;
        private SortedBy _sorting;

        /// <summary>
        /// Initializes the object with the sorting set to Ascending by default.
        /// </summary>
        public SortedLinkedList()
        {
            _head = new Node<T>(default(T));
            _sorting = SortedBy.Ascending;
        }

        /// <summary>
        /// Initializes the object with the given sorting.
        ///
        /// Note: For now, a new object has to be created to change the sorting. 
        /// </summary>
        /// <param name="sorting"></param>
        public SortedLinkedList(SortedBy sorting)
        {
            _head = new Node<T>(default(T));
            _sorting = sorting;
        }

        /// <summary>
        /// Adds and sorts the value from the front.
        /// </summary>
        /// <param name="value"></param>
        public void SortedAddFront(T value)
        {
            // No need to sort if list is empty.
            if (Count <= 0)
            {
                AddFront(value);
                return;
            }

            var newNode = new Node<T>(value);
            var current = _head.Next;
            while (current != null)
            {
                // Could be -1 (for less than), 0 (for equal), or 1 (for greater than)
                int result = newNode.Value.CompareTo(current.Value);
                if (_sorting == SortedBy.Ascending)
                {
                    if (result <= 0)
                    {
                        InsertBefore(current, newNode);
                        return;
                    }
                }
                else
                {
                    if (result >= 0)
                    {
                        InsertBefore(current, newNode);
                        return;
                    }
                }

                current = current.Next;
            }

            // If the new value isn't inserted yet, it must be the
            // biggest/smallest value, so add it at the back.
            AddBack(value);
        }

        /// <summary>
        /// Adds and sorts the value from the back.
        /// </summary>
        /// <param name="value"></param>
        public void SortedAddBack(T value)
        {
            if (Count <= 0)
            {
                AddBack(value);
                return;
            }

            var newNode = new Node<T>(value);
            var current = _head.Prev;
            while (current != null)
            {
                int result = newNode.Value.CompareTo(current.Value);
                if (_sorting == SortedBy.Ascending)
                {
                    if (result >= 0)
                    {
                        InsertAfter(current, newNode);
                        return;
                    }
                }
                else
                {
                    if (result <= 0)
                    {
                        InsertAfter(current, newNode);
                        return;
                    }
                }

                current = current.Prev;
            }

            AddFront(value);
        }

        /// <summary>
        /// Removes and returns the value at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException();
            
            int counter = 0;
            var current = _head.Next;
            while (counter < index)
            {
                current = current.Next;
                counter++;
            }

            var node = Remove(current);
            return node.Value;
        }

        /// <summary>
        /// Removes and returns the value at the front.
        /// </summary>
        /// <returns></returns>
        public T PopFront()
        {
            if (Count <= 0)
                return default(T);

            var node = Remove(_head.Next);
            return node.Value;
        }

        /// <summary>
        /// Removes and returns the value at the back.
        /// </summary>
        /// <returns></returns>
        public T PopBack()
        {
            if (Count <= 0)
                return default(T);

            var node = Remove(_head.Prev);
            return node.Value;
        }

        /// <summary>
        /// Returns the value at the front, yet does not remove it from the list.
        /// </summary>
        /// <returns></returns>
        public T PeekFront()
        {
            if (Count <= 0)
                return default(T);

            return _head.Next.Value;
        }

        /// <summary>
        /// Returns the value at the back, yet does not remove it from the list.
        /// </summary>
        /// <returns></returns>
        public T PeekBack()
        {
            if (Count <= 0)
                return default(T);

            return _head.Prev.Value;
        }

        /// <summary>
        /// Checks if the list contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value)
        {
            foreach (var node in this)
            {
                if (node.Equals(value))
                    return true;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var current = _head.Next;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }

        private void AddFront(T value)
        {
            var newNode = new Node<T>(value);
            if (_head.Next != null)
            {
                newNode.Next = _head.Next;
                _head.Next.Prev = newNode;
            }
            else
                _head.Prev = newNode;

            _head.Next = newNode;
            Count++;
        }

        private void AddBack(T value)
        {
            var newNode = new Node<T>(value);
            if (_head.Prev != null)
            {
                newNode.Prev = _head.Prev;
                _head.Prev.Next = newNode;
            }
            else
                _head.Next = newNode;

            _head.Prev = newNode;
            Count++;
        }

        private void InsertBefore(Node<T> current, Node<T> newNode)
        {
            if (current == null || newNode == null)
                throw new ArgumentNullException();

            if (current.Prev != null)
            {
                newNode.Prev = current.Prev;
                current.Prev.Next = newNode;
            }
            else
            {
                _head.Next = newNode;
            }

            newNode.Next = current;
            current.Prev = newNode;

            Count++;
        }

        private void InsertAfter(Node<T> current, Node<T> newNode)
        {
            if (current == null || newNode == null)
                throw new ArgumentNullException();

            if (current.Next != null)
            {
                newNode.Next = current.Next;
                current.Next.Prev = newNode;
            }
            else
            {
                _head.Prev = newNode;
            }

            newNode.Prev = current;
            current.Next = newNode;

            Count++;
        }

        private Node<T> Remove(Node<T> node)
        {
            if (Count == 1)
            {
                _head.Next = null;
                _head.Prev = null;
                Count = 0;
                return node;
            }

            if (node.Prev != null)
                node.Prev.Next = node.Next;
            else
                _head.Next = node.Next;

            if (node.Next != null)
                node.Next.Prev = node.Prev;
            else
                _head.Prev = node.Prev;

            Count--;
            return node;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /*
        // Leaving it here for reference for later.

        public void InsertBefore(T value, int index)
        {
            if (Count <= index)
                throw new ArgumentOutOfRangeException();

            int counter = 0;
            var current = _head.Next;
            while (counter < index)
            {
                current = current.Next;
                counter++;
            }

            var newNode = new Node<T>(value);
            InsertBefore(current, newNode);
        }

        public void InsertAfter(T value, int index)
        {
            if (Count <= index)
                throw new ArgumentOutOfRangeException();

            int counter = 0;
            var current = _head.Next;
            while (counter < index)
            {
                current = current.Next;
                counter++;
            }

            var newNode = new Node<T>(value);
            InsertAfter(current, newNode);
        }
        /**/
    }
}
