using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DG.Common.Collections
{
    /// <summary>
    /// Represents a collection of key/value pairs that is optimized for accessing through prefixes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class RadixDictionary<T> : IReadOnlyDictionary<string, T>
    {
        private readonly RadixNode _root;

        /// <summary>
        /// Returns a value indicating whether this instance of <see cref="RadixDictionary{T}"/> is case-sensitive when determining if keys match.
        /// </summary>
        public bool IsCaseSensitive => _root.IsCaseSensitive;

        /// <summary>
        /// Initializes a new instance of <see cref="RadixDictionary{T}"/>.
        /// </summary>
        /// <param name="caseSensitive"></param>
        public RadixDictionary(bool caseSensitive)
        {
            _root = new RadixNode(caseSensitive);
        }

        /// <summary>
        /// Adds the specified key and value to this dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
        public bool Add(string key, T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var dictionary = new Dictionary<string, T>();
            dictionary[key] = value;
            return _root.Add(key, value);
        }

        /// <summary>
        /// Returns a value indiciating whether this <see cref="RadixDictionary{T}"/> contains the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
        public bool ContainsKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }
            return _root.TryFindNode(key, out RadixNode _);
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="KeyNotFoundException"/>, and a set operation creates a new element with the specified key.</returns>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
        /// <exception cref="KeyNotFoundException">Key does not exist in the collection.</exception>
        public T this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                if (!TryGetValue(key, out T value))
                {
                    throw new KeyNotFoundException($"No item with the key {key} has been found.");
                }
                return value;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }
                _root.Add(key, value, true);
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs in this <see cref="RadixDictionary{T}"/>.
        /// </summary>
        /// <returns></returns>
        public int Count => _root.Count();

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the <see cref="RadixDictionary{T}"/>.
        /// </summary>
        public IEnumerable<string> Keys => _root.GetKeyValuePairs().Select(kv => kv.Key);

        /// <summary>
        /// Gets an enumerable collection that contains the values in the <see cref="RadixDictionary{T}"/>.
        /// </summary>
        public IEnumerable<T> Values => _root.GetKeyValuePairs().Select(kv => kv.Value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IEnumerable<T> FindByPrefix(string prefix)
        {
            if (prefix == null)
            {
                throw new ArgumentNullException(nameof(prefix));
            }
            var nodes = _root.GetNodesByPrefix(prefix);
            foreach (var node in nodes.Where(n => n.IsEndNode))
            {
                yield return node.Value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>True if the <see cref="RadixDictionary{T}"/> contains an element with the specified key, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Key is null.</exception>
        public bool TryGetValue(string key, out T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");
            }
            if (_root.TryFindNode(key, out RadixNode node))
            {
                value = node.Value;
                return true;
            }
            value = default(T);
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            foreach (var pair in _root.GetKeyValuePairs())
            {
                yield return pair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// This class represents a node in a radix tree of keys and values, to optimise searching for keys based on their prefix.
        /// </summary>
        private sealed class RadixNode
        {
            private readonly bool _isRoot;
            private readonly bool _caseSensitive;

            private string _key;
            private T _value;
            private readonly List<RadixNode> _children;

            public bool IsCaseSensitive => _caseSensitive;

            public bool IsEmpty => string.IsNullOrEmpty(_key);
            public T Value => _value;

            public bool IsRoot => _isRoot;
            public bool IsEndNode => !IsRoot && _children.Count == 0;

            /// <summary>
            /// Instantiates a new instance of the <see cref="RadixNode"/>. This is the root of a new radix tree.
            /// </summary>
            /// <param name="caseSensitive"></param>
            public RadixNode(bool caseSensitive)
            {
                _isRoot = true;
                _caseSensitive = caseSensitive;
                _children = new List<RadixNode>();
            }

            private RadixNode(RadixNode parent, string key, T value) : this(parent._caseSensitive)
            {
                _isRoot = false;
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
            public bool Add(string key, T value, bool overwrite = false)
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
                if (TryMatchChild(key, out RadixNode matchedChild))
                {
                    return matchedChild.Add(key.Substring(matchedChild._key.Length), value, overwrite);
                }
                foreach (var child in _children)
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
                    _children.Add(new RadixNode(this, "", Value));
                }
                _children.Add(new RadixNode(this, key, value));
                return true;
            }

            public bool TryFindNode(string key, out RadixNode node)
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key), "Key cannot be null.");
                }
                if (key.Length > 0)
                {
                    if (TryMatchChild(key, out RadixNode matchedChild))
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

            public IEnumerable<RadixNode> GetNodesByPrefix(string prefix)
            {
                if (prefix.Length == 0)
                {
                    return GetEndNodes();
                }
                if (TryMatchChild(prefix, out RadixNode matchedChild))
                {
                    return matchedChild.GetNodesByPrefix(prefix.Substring(matchedChild._key.Length));
                }
                List<RadixNode> nodes = new List<RadixNode>();
                foreach (var child in _children)
                {
                    if (child.IsPartialMatch(prefix, out int matchLength) && matchLength == prefix.Length)
                    {
                        nodes.AddRange(child.GetEndNodes());
                    }
                }
                return nodes;
            }

            /// <summary>
            /// Returns a list of all endnodes in this tree. This includes the current node if it is an endnode.
            /// </summary>
            /// <returns></returns>
            public IEnumerable<RadixNode> GetEndNodes()
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

            public IEnumerable<KeyValuePair<string, T>> GetKeyValuePairs(string prefix = "")
            {
                if (IsEndNode)
                {
                    yield return new KeyValuePair<string, T>(prefix + _key, Value);
                    yield break;
                }
                foreach (var childPair in _children.SelectMany(c => c.GetKeyValuePairs(prefix + _key)))
                {
                    yield return childPair;
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

            private void Split(int length, string key, T value)
            {
                var newParent = new RadixNode(this, _key.Substring(length), Value);
                EvictChildrenTo(newParent);
                _children.Add(newParent);
                _children.Add(new RadixNode(this, key.Substring(length), value));
                _key = key.Substring(0, length);
            }

            private void EvictChildrenTo(RadixNode newParent)
            {
                foreach (var child in _children)
                {
                    newParent._children.Add(child);
                }
                _children.Clear();
            }

            private bool IsMatch(string key)
            {
                if (IsEmpty && string.IsNullOrEmpty(key))
                {
                    return true;
                }
                if (!IsPartialMatch(key, out int matchLength))
                {
                    return false;
                }
                return matchLength == _key.Length;
            }

            private bool IsPartialMatch(string key, out int matchLength)
            {
                matchLength = 0;
                if (string.IsNullOrEmpty(_key))
                {
                    return false;
                }
                for (int i = 0; i < _key.Length && i < key.Length; i++)
                {
                    var match = _caseSensitive ? (key[i] == _key[i]) : (char.ToUpperInvariant(key[i]) == char.ToUpperInvariant(_key[i]));
                    if (!match)
                    {
                        break;
                    }
                    matchLength++;
                }
                return matchLength > 0;
            }

            private bool TryMatchChild(string key, out RadixNode child)
            {
                child = _children.FirstOrDefault(c => c.IsMatch(key));
                return child != null;
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
}
