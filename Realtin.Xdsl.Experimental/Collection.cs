using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Realtin.Xdsl.Serialization;

namespace Realtin.Xdsl;

internal class Collection<T> : IEnumerable<T>
{
    public struct Enumerator : IEnumerator<T>
    {
        private readonly Collection<T> _list;

        private int _index;

        private T _current;

        public readonly T Current => _current;

        readonly object? IEnumerator.Current
        {
            get {
                return Current;
            }
        }

        internal Enumerator(Collection<T> list)
        {
            _list = list;
            _index = 0;
            _current = default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            Collection<T> list = _list;
            if ((uint)_index < (uint)list._size) {
                _current = list._items[_index];
                _index++;
                return true;
            }

            return MoveNextRare();
        }

        private bool MoveNextRare()
        {
            _index = _list._size + 1;
            _current = default!;
            return false;
        }

        void IEnumerator.Reset()
        {
            _index = 0;
            _current = default!;
        }

        readonly void IDisposable.Dispose()
        {
        }
    }

    [XdslIgnore]
    private T[] _items;

    [XdslIgnore]
    private int _size;

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if (index < 0 || index >= _size) {
                throw new IndexOutOfRangeException();
            }

            return _items[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            if (index < 0 || index >= _size) {
                throw new IndexOutOfRangeException();
            }

            _items[index] = value;
        }
    }

    public int Count => _size;

    [XdslIgnore]
    public int Capacity
    {
        get => _items.Length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set {
            if (value <= _items.Length) {
                throw new ArgumentOutOfRangeException(nameof(Capacity));
            }

            var temp = _items.AsSpan();
            _items = new T[value];

            temp.CopyTo(_items);
        }
    }

    public Collection()
    {
        _items = new T[4];
    }

    public Collection(int capacity)
    {
        _items = new T[capacity];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal void Add(T item)
    {
        if (_size + 1 >= Capacity) {
            Resize();
        }

        _items[_size++] = item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
            int size = _size;
            _size = 0;
            if (size > 0) {
                _items.AsSpan(0, size).Clear();
            }
        }
        else {
            _size = 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal bool Contains(T item)
    {
        if (_size != 0) {
            return IndexOf(item) >= 0;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(_items, 0, array, arrayIndex, _size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal int IndexOf(T item)
    {
        return Array.IndexOf(_items, item, 0, _size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal void Insert(int index, T item)
    {
        if ((uint)index > (uint)_size) {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (_size == _items.Length) {
            Resize();
        }
        if (index < _size) {
            Array.Copy(_items, index, _items, index + 1, _size - index);
        }
        _items[index] = item;
        _size++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal bool Remove(T item)
    {
        int index = IndexOf(item);
        if (index >= 0) {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected internal void RemoveAt(int index)
    {
        if ((uint)index >= (uint)_size) {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        _size--;
        if (index < _size) {
            Array.Copy(_items, index + 1, _items, index, _size - index);
        }
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
            _items[_size] = default!;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
        return new Span<T>(_items, 0, _size);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Resize()
    {
        Capacity += 6;
    }
}