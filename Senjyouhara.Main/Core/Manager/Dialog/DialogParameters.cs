using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    
    class MyEnumerator : IEnumerator
    {
        private Dictionary<string, object> data;
        private List<string> keys = new();
        private int keyIndex = -1;

        public MyEnumerator(Dictionary<string, object> data)
        {
            this.data = data;
            keys = data.Keys.ToList();
        }

        public object Current => data[keys[keyIndex]];

        public bool MoveNext()
        {
            keyIndex++;
            return keyIndex < keys.Count;
        }

        public void Reset()
        {
            keyIndex = -1;
        }
    }
    
    public class DialogParameters : IDialogParameters,System.Collections.IEnumerable
    {
        private readonly Dictionary<string, object> _entries = new();

        public int Count => _entries.Count;

        public IEnumerable<string> Keys => _entries.Keys;

        public void Add(string key, object value)
        {
            _entries[key] = value;
        }

        public bool ContainsKey(string key)
        {
           return _entries.ContainsKey(key);
        }

        public T GetValue<T>(string key)
        {
            return (T) _entries[key];
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            var k = _entries.ContainsKey(key);
            if (k)
            {
               value = (T)_entries[key];
               return true;
            }

            value = default;
            return k;
        }

        public IEnumerator GetEnumerator()
        {
            return new MyEnumerator(_entries);
        }
    }
}
