using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UserVoice
{
    public class Collection : IList<JToken> {
        public const int PER_PAGE = 100;

        private Client client;
        private string path;
        private int perPage;
        private int limit = int.MaxValue;
        private Dictionary<int, JToken[]> pages = new Dictionary<int, JToken[]>();
        private JToken responseData = null;

        public Collection(Client client, string path, int? limit=null) {
            this.client = client;
            this.path = path;
            this.limit = limit ?? this.limit;
            this.perPage = Math.Min(this.limit, PER_PAGE);
        }
        public int Count {
            get {
                if (this.responseData == null) {
                    //Load the first page
                    LoadPage(1);
                }
                return Math.Min((int)this.responseData["total_records"], this.limit);
            }
        }

        public JToken this[int i] {
            get {
                //Console.WriteLine(string.Format("this[{0}]", i));
                if (i == 0 || (i > 0 && i < this.Count()) ) {
                    return LoadPage((int)(i/(float)(PER_PAGE)) + 1)[i % PER_PAGE];
                } else {
                    throw new IndexOutOfRangeException();
                }
            }
            set { throw new NotSupportedException("Read-only"); }
        }
        private JToken[] LoadPage(int i) {
            if (!pages.ContainsKey(i)) {
                string url;
                if (path.Contains("?")) {
                    url = path+"&";
                } else {
                    url = path+"?";
                }
                JToken result = client.Get(string.Format("{0}per_page={1}&page={2}", url, perPage, i));
                JToken[] elements = result.Values<JProperty>()
                                              .Where(k => k.Name != "response_data")
                                              .Select(k => k.Value).First().ToArray();
                if (null != result["response_data"]) {
                    responseData = result["response_data"];
                    pages[i] = elements;
                } else {
                    throw new UserVoice.NotFound("The resource you requested is not a collection.");
                }
            }
            return pages[i];
        }
        public IEnumerator<JToken> GetEnumerator() {
            for (int i = 0; i < this.Count(); i++) {
                yield return this[i];
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public int IndexOf(Newtonsoft.Json.Linq.JToken o) { return 0; }
        public void RemoveAt(int index) { throw new NotSupportedException("Read-only"); }
        public void Clear() { throw new NotSupportedException("Read-only"); }
        public bool IsReadOnly { get { return true; } }
        public void Add(Newtonsoft.Json.Linq.JToken o) { throw new NotSupportedException("Read-only"); }
        public bool Contains(Newtonsoft.Json.Linq.JToken o ) { throw new NotSupportedException("Contains not supported"); }
        public void CopyTo(Newtonsoft.Json.Linq.JToken[] a, int index) { throw new NotSupportedException("Read-only"); }
        public bool Remove(Newtonsoft.Json.Linq.JToken o) { throw new NotSupportedException("Read-only"); }
        public void Insert(int index, Newtonsoft.Json.Linq.JToken o) { throw new NotSupportedException("Read-only"); }
    }
}