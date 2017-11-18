#pragma warning disable 0168

using System;

namespace FrameWork
{
    public class Heap<T> where T : IHeapItem<T>
    {
        private const int defaultCapacity = 200;
        private T[] items;
        private int _itemCount;
        public int Count { get { return _itemCount; } }

        private int _capacity;
        public int capacity { get { return _capacity; } }

        public Heap()
        {
            _capacity = defaultCapacity;
            items = new T[_capacity];
            _itemCount = 0;
        }

        public Heap(int capacity)
        {
            _capacity = capacity;
            items = new T[_capacity];
            _itemCount = 0;
        }

        public T this[int index]
        {
            get { try { return items[index]; } catch (System.IndexOutOfRangeException e) { return default(T); } }
        }

        public void Clear()
        {
            Array.Clear(items, 0, _itemCount);
            _itemCount = 0;
        }

        public void Add(T item)
        {
            item.HeapIndex = _itemCount;
            items[_itemCount] = item;
            SortUp(item);
            _itemCount++;
        }

        public T PopFront()
        {
            T first = items[0];
            _itemCount--;
            items[0] = items[_itemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return first;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            while (true)
            {
                T parent = items[parentIndex];
                if (item.CompareTo(parent) > 0)
                {
                    Swap(item, parent);
                }
                else break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void SortDown(T item)
        {
            while (true)
            {
                int leftChild = item.HeapIndex * 2 + 1;
                int rightChild = item.HeapIndex * 2 + 2;
                int swapIndex = 0;
                if (leftChild < _itemCount)
                {
                    swapIndex = leftChild;

                    if (rightChild < _itemCount)
                    {
                        if (items[leftChild].CompareTo(items[rightChild]) < 0)
                        {
                            swapIndex = rightChild;
                        }
                    }

                    if (item.CompareTo(items[swapIndex]) < 0)
                    {
                        Swap(item, items[swapIndex]);
                    }
                    else return;
                }
                else return;
            }
        }

        void Swap(T a, T b)
        {
            items[a.HeapIndex] = b;
            items[b.HeapIndex] = a;
            int itemAIndex = a.HeapIndex;
            a.HeapIndex = b.HeapIndex;
            b.HeapIndex = itemAIndex;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}
