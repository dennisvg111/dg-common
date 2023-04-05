using System;
using System.Collections.Generic;
using System.Linq;

namespace DG.Common.Collections
{
    /// <summary>
    /// Represents a collection of key/value pairs that can be accessed using prefixes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class RadixDictionary<T>
    {
        private readonly RadixNode<T> _root;

        /// <summary>
        /// Initializes a new instance of <see cref="RadixDictionary{T}"/>.
        /// </summary>
        /// <param name="caseInsensitive"></param>
        public RadixDictionary(bool caseInsensitive)
        {
            _root = new RadixNode<T>(caseInsensitive);
        }

        public bool Add(string key, T value)
        {
            return _root.Add(key, value);
        }

        /// <summary>
        /// Returns a value indiciating whether this <see cref="RadixDictionary{T}"/> contains the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return _root.TryFindNode(key, out RadixNode<T> _);
        }

        public bool TryFind(string key, out T value)
        {
            if (_root.TryFindNode(key, out RadixNode<T> node))
            {
                value = node.Value;
                return true;
            }
            value = default(T);
            return false;
        }

        public T this[string key]
        {
            get
            {
                T value;
                if (!TryFind(key, out value))
                {
                    throw new KeyNotFoundException($"No item with the key {key} has been found.");
                }
                return value;
            }
            set
            {
                _root.Add(key, value, true);
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs in this <see cref="RadixDictionary{T}"/>.
        /// </summary>
        /// <returns></returns>
        public int Count => _root.Count();

        /// <summary>
        /// Gets a collection containing the keys of this <see cref="RadixDictionary{T}"/>.
        /// </summary>
        public IEnumerable<string> Keys => _root.GetKeys();

        public IEnumerable<T> FindByPrefix(string prefix)
        {
            var nodes = _root.GetNodesByPrefix(prefix);
            foreach (var node in nodes.Where(n => n.HasValue))
            {
                yield return node.Value;
            }
        }
    }

    /// <summary>
    /// This class represents a radix tree of keys and values, to optimise searching for keys based on their prefix.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class RadixNode<T>
    {
        private readonly bool _caseInsensitive;
        protected string _key;
        private T _value;
        private RadixNode<T> _parent;
        protected readonly List<RadixNode<T>> _children;

        public bool CaseInsensitive => _caseInsensitive;

        public string PartialKey => _key;
        public bool IsEmpty => string.IsNullOrEmpty(_key);

        public T Value => _value;
        public bool HasValue => _value != null;

        public RadixNode<T> Parent => _parent;
        public bool IsRoot => _parent == null;

        public IReadOnlyList<RadixNode<T>> Children => _children;
        public bool IsEndNode => !IsRoot && _children.Count == 0;

        /// <summary>
        /// Instantiates a new instance of the <see cref="RadixNode{T}"/>. This is the root of a new radix tree.
        /// </summary>
        internal RadixNode(bool caseInsensitive)
        {
            _caseInsensitive = caseInsensitive;
            _children = new List<RadixNode<T>>();
        }

        protected RadixNode(RadixNode<T> parent, string key, T value) : this(parent.CaseInsensitive)
        {
            _parent = parent;
            _key = key;
            _value = value;
        }

        /// <summary>
        /// Adds a new key and value to this radix tree.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        internal bool Add(string key, T value, bool overwrite = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }
            if (key.Length == 0)
            {
                if (overwrite)
                {
                    _value = value;
                    return true;
                }
                return false;
            }
            foreach (var child in Children)
            {
                if (child.IsMatch(key))
                {
                    return child.Add(key.Substring(child._key.Length), value);
                }
            }
            foreach (var child in Children)
            {
                int matchLength;
                if (child.IsPartialMatch(key, out matchLength))
                {
                    child.Split(matchLength, key, value);
                    return true;
                }
            }
            if (IsEndNode)
            {
                _children.Add(new RadixNode<T>(this, "", Value));
            }
            _children.Add(new RadixNode<T>(this, key, value));
            return true;
        }

        public bool TryFindNode(string key, out RadixNode<T> node)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }
            if (key.Length > 0)
            {
                var matchedChild = Children.FirstOrDefault(c => c.IsMatch(key));
                if (matchedChild != null)
                {
                    return matchedChild.TryFindNode(key.Substring(matchedChild._key.Length), out node);
                }
                node = null;
                return false;
            }
            if (IsEndNode)
            {
                node = this;
                return true;
            }
            foreach (var child in _children.Where(c => c.IsEmpty))
            {
                node = child;
                return true;
            }
            node = null;
            return false;
        }

        public IEnumerable<RadixNode<T>> GetNodesByPrefix(string prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix), "Prefix cannot be null.");
            }
            if (prefix.Length == 0)
            {
                return GetEndNodes();
            }
            foreach (var child in Children)
            {
                if (child.IsMatch(prefix))
                {
                    return child.GetNodesByPrefix(prefix.Substring(child._key.Length));
                }
            }
            List<RadixNode<T>> nodes = new List<RadixNode<T>>();
            foreach (var child in Children)
            {
                if (child.IsPartialMatch(prefix, out int matchLength) && matchLength == prefix.Length)
                {
                    nodes.AddRange(child.GetEndNodes());
                }
            }
            return nodes;
        }

        private string GetKey()
        {
            return IsRoot ? (_key) : (_parent.GetKey() + _key);
        }

        private string GetPrefix()
        {
            return IsRoot ? "" : _parent.GetKey();
        }

        public IEnumerable<string> GetKeys()
        {
            var currentPrefix = GetPrefix();
            return GetKeys(currentPrefix);
        }

        /// <summary>
        /// Returns a list of all endnodes in this tree. This includes the current node if it is an endnode.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RadixNode<T>> GetEndNodes()
        {
            if (IsEndNode)
            {
                yield return this;
                yield break;
            }
            foreach (var endNode in _children.SelectMany(c => c.GetEndNodes()))
            {
                yield return endNode;
            }
        }

        /// <summary>
        /// Counts the number of endnodes in this tree. This includes the current node if it is an endnode.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            if (IsEndNode)
            {
                return 1;
            }
            return _children.Sum(c => c.Count());
        }

        private IEnumerable<string> GetKeys(string prefix)
        {
            if (IsEndNode)
            {
                yield return prefix + _key;
                yield break;
            }
            foreach (var childKey in _children.SelectMany(c => c.GetKeys(prefix + _key)))
            {
                yield return childKey;
            }
        }

        protected void Split(int length, string key, T value)
        {
            var newParent = new RadixNode<T>(this, _key.Substring(length), Value);
            MoveChildren(newParent);
            _children.Add(newParent);
            _children.Add(new RadixNode<T>(this, key.Substring(length), value));
            _key = key.Substring(0, length);
        }

        protected void MoveChildren(RadixNode<T> newParent)
        {
            foreach (var child in Children)
            {
                newParent._children.Add(child);
                child._parent = newParent;
            }
            _children.Clear();
        }

        protected bool IsMatch(string key)
        {
            if (IsEmpty && !string.IsNullOrEmpty(key))
            {
                return false;
            }
            if (key.Length < _key.Length)
            {
                return false;
            }
            StringComparison comparison = CaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return string.Equals(_key, key.Substring(0, _key.Length), comparison);
        }

        protected bool IsPartialMatch(string key, out int matchLength)
        {
            matchLength = 0;
            if (string.IsNullOrEmpty(_key))
            {
                return false;
            }
            for (int i = 0; i < _key.Length && i < key.Length; i++)
            {
                var match = CaseInsensitive ? (char.ToUpperInvariant(key[i]) == char.ToUpperInvariant(_key[i])) : (key[i] == _key[i]);
                if (!match)
                {
                    break;
                }
                matchLength++;
            }
            return matchLength > 0;
        }

        public override string ToString()
        {
            if (IsRoot)
            {
                return "root";
            }
            return _key;
        }
    }
}
