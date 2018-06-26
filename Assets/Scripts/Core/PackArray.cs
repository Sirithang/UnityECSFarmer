using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackArray<T>
{
    protected List<T> _content;
    protected List<int> _nextId;

    protected int _nextFreeId = -1;

    public PackArray(int capacity)
    {
        _content = new List<T>(capacity);
        _nextId = new List<int>(capacity);

        _nextFreeId = -1;
    }

    public PackArray()
    {
        _content = new List<T>();
        _nextId = new List<int>();

        _nextFreeId = -1;
    }

    public bool IsFree(int index)
    {
        return _nextId[index] == -1;
    }

    public void Add(T obj)
    {
        if(_nextFreeId != -1)
        {
            int id = _nextFreeId;

            _nextFreeId = _nextId[id];

            _content[id] = obj;
            _nextId[id] = -1;
        }
        else
        {
            _content.Add(obj);
            _nextId.Add(-1);
        }
    }

    public void RemoveAt(int idx)
    {
        _nextId[idx] = _nextFreeId;
        _nextFreeId = idx;
    }

    public void Remove(T obj)
    {
        int idx = _content.IndexOf(obj);
        if (idx >= 0)
        {
            RemoveAt(idx);
        }
    }

    public T this[int i]
    {
        get { return _content[i]; }
        set { _content[i] = value; }
    }
}
