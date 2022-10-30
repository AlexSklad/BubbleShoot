using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pool<T> where T : class
{
    private readonly T[] _pool;
    private readonly int _size;
    private int _position;

    private readonly Func<T> _ctorDelegate;
    private readonly Action<T> _dtorDelegate;
    private readonly Action<T> _pullDelegate;
    private readonly Action<T> _pushDelegate;
    
    public int Count => this._size;

    public Pool(int capacity, Func<T> ctor, Action<T> dtor, Action<T> onPull, Action<T> onPush)
    {
        this._pool = new T[capacity];
        this._size = capacity;

        this._ctorDelegate = ctor ?? Pool<T>.DefaultCtorDelegate;
        this._dtorDelegate = dtor ?? Pool<T>.DefaultDtorDelegate;
        this._pullDelegate = onPull ?? Pool<T>.DefaultPullDelegate;
        this._pushDelegate = onPush ?? Pool<T>.DefaultPushDelegate;
    }

    private static T DefaultCtorDelegate() => default;
    private static void DefaultDtorDelegate(T obj) { }
    private static void DefaultPullDelegate(T obj) { }
    private static void DefaultPushDelegate(T obj) { }

    public bool Init()
    {
        if (this._position == this._size)
        {
            return false;
        }

        while (this._position < this._size)
        {
            var value = this._ctorDelegate.Invoke();
            this._pushDelegate(value);
            this._pool[this._position++] = value;
        }

        return true;
    }

    public bool Init(int count)
    {
        if (this._position == this._size)
        {
            return false;
        }

        for (int i = 0; i < count && this._position < this._size; ++i)
        {
            var value = this._ctorDelegate.Invoke();
            this._pushDelegate(value);
            this._pool[this._position++] = value;
        }

        return true;
    }

    public void Clear()
    {
        for (int i = 0; i < this._position; ++i)
        {
            this._dtorDelegate(this._pool[i]);
            this._pool[i] = default;
        }

        this._position = 0;
    }

    public T Pull()
    {
        T result;

        if (this._position == 0)
        {
            result = this._ctorDelegate.Invoke();
        }
        else
        {
            result = this._pool[--this._position];
            this._pool[this._position] = default;
        }

        this._pullDelegate.Invoke(result);
        return result;
    }

    public void Push(T value)
    {
        this._pushDelegate(value);

        if (this._position == this._size)
        {
            this._dtorDelegate(value);
        }
        else
        {
            this._pool[this._position++] = value;
        }
    }
}
