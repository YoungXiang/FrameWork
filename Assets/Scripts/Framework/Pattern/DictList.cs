using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DictList<T, V>  : IEnumerable
{
    public int Count { get { return _list.Count; } }
    public List<V> Lists { get { return _list; } }
    private List<V> _list = new List<V>();
    
    private Dictionary<T, int> _map = new Dictionary<T, int>();
    public Dictionary<T, int> Map
    {
        get { return _map; }
    }

    public V Get(T key)
    {
        if (_map.ContainsKey(key))
        {
            return _list[_map[key]];
        }

        return default(V);
    }

    public V this[T key]
    {
        get { return Get(key); }
        //set { Add(key, value); }
    }

    public bool ContainsKey(T key)
    {
        return _map.ContainsKey(key);
    }

    public void Add(T key, V val)
    {
        if (_map.ContainsKey(key))
        {
            Debug.LogFormat("Duplicated Key added : {0}", key);
            _list[_map[key]] = val;
        }
        else
        {
            _list.Add(val);
            _map.Add(key, _list.Count - 1);
        }
    }

    public void Clear()
    {
        _map.Clear();
        _list.Clear();
    }

    public void Remove(T key)
    {
        if (!_map.ContainsKey(key))
        {
            Debug.LogFormat("Key not found : {0}!", key);
            return;
        }

        int index = _map[key];
        _list.RemoveAt(index);

        T[] keys = new T[_map.Keys.Count];
        _map.Keys.CopyTo(keys, 0);
        for (int i = 0; i < keys.Length; i++)
        {
            if (_map[keys[i]] > index)
            {
                _map[keys[i]]--;
            }
        }
//        foreach (KeyValuePair<T, int> pair in _map)
        _map.Remove(key);
    }

    public IEnumerator GetEnumerator()
    {
        foreach (KeyValuePair<T, int> pair in _map)
        {
            yield return pair.Key;
        }
    }
}
