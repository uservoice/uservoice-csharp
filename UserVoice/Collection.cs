using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace UserVoice
{
    public class Unsupported: Exception {}
    public class Collection : IList<JToken> {
        private Client client;

        public Collection(Client client) {
            this.client = client;
        }
        public JToken this[int index]{ get { throw new Unsupported(); } set {throw new Unsupported(); } }
        public IEnumerator<JToken> GetEnumerator() { throw new Unsupported(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new Unsupported(); }
        public int Count { get { throw new Unsupported(); } }

        public int IndexOf(Newtonsoft.Json.Linq.JToken o) { return 0; }
        public void RemoveAt(int index) { throw new Unsupported(); }
        public void Clear() { throw new Unsupported(); }
        public bool IsReadOnly { get { return true; } }
        public void Add(Newtonsoft.Json.Linq.JToken o) { throw new Unsupported(); }
        public bool Contains(Newtonsoft.Json.Linq.JToken o ) { throw new Unsupported(); }
        public void CopyTo(Newtonsoft.Json.Linq.JToken[] a, int index) { throw new Unsupported(); }
        public bool Remove(Newtonsoft.Json.Linq.JToken o) { throw new Unsupported(); }
        public void Insert(int index, Newtonsoft.Json.Linq.JToken o) { throw new Unsupported(); }
    }
}