using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey>
{
    private readonly LinkedList<KeyValuePair<TKey, TValue>> _linkedList;

    public PriorityQueue()
    {
        _linkedList = new LinkedList<KeyValuePair<TKey, TValue>>();
    }

    public void Add(TKey key, TValue value)
    {
        Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        if (item.Key == null || item.Value == null)
            throw new ArgumentNullException();

        var node = new KeyValuePair<TKey, TValue>(item.Key, item.Value);

        // If empty add first item
        if (_linkedList.Count == 0)
        {
            _linkedList.AddFirst(node);
        }

        // If equal or less than first value add before
        else if (item.Key.CompareTo(_linkedList.First.Value.Key) < 0)
        {
            _linkedList.AddFirst(node);
        }
        else if (item.Key.CompareTo(_linkedList.Last.Value.Key) > 0)
        {
            _linkedList.AddLast(node);
        }
        else
        {
            var current = _linkedList.First;
            while (item.Key.CompareTo(current.Value.Key) > 0)
            {
                current = current.Next;
            }
            _linkedList.AddBefore(current, node);
        }

    }

    public TValue DeQueue()
    {
        var first = _linkedList.First;
        _linkedList.RemoveFirst();
        return first.Value.Value;
    }

    public TValue Peek()
    {
        return _linkedList.First.Value.Value;
    }

    public void Clear()
    {
        _linkedList.Clear();
    }

    public int Count { get { return _linkedList.Count; } }

    public bool IsSynchronized { get; set; }

    public object SyncRoot { get; set; }

    public bool IsReadOnly
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        _linkedList.Remove(item);

        return true;
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return _linkedList.GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return _linkedList.GetEnumerator();
    }
}
