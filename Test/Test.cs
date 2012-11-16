
namespace Test
{
    using UserVoice;
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;
    public class AssertionError: Exception { public AssertionError(string msg): base(msg) {}}



    public abstract class Test {
        public Test() {
            foreach (MethodInfo method in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)) {
                if (method.Name.StartsWith("Should") || method.Name.StartsWith("Test") ) {
                    this.GetType().InvokeMember(method.Name, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, this, null);
                }
            }
        }
        private NameValueCollection config;
        private static int assertionsFailed = 0;
        private static int assertionsTotal = 0;
        public static int AssertionsFailed() {
            return assertionsFailed;
        }
        public static int AssertionsTotal() {
            return assertionsTotal;
        }
        protected NameValueCollection Config {
            get {
                if (config == null)  {
                    string line;
                    config = new NameValueCollection();
                    System.IO.StreamReader reader = new System.IO.StreamReader("Test/config.yml");
                    while ((line = reader.ReadLine()) != null) {
                        string [] substrings = Regex.Split(line.Split('#')[0], ": *");
                        if (substrings.Length == 2) {
                            config[substrings[0]] = substrings[1];
                        }
                    }
                }
                return config;
            }
        }
        protected void AssertEqual(string one, string two) {
            assertionsTotal++;
            if (!(one == two)) {
                assertionsFailed++;
                Console.WriteLine(one + "==" + two + " failed in " + new StackFrame(1, true).GetMethod().ToString());
            }
        }
        protected void AssertEqual(bool one, bool two) {
            assertionsTotal++;
            if (!(one == two)) {
                assertionsFailed++;
                Console.WriteLine(one + "==" + two + " failed in " + new StackFrame(1, true).GetMethod().ToString());
            }
        }
        protected void AssertTrue(bool one) {
            assertionsTotal++;
            if (!one) {
                assertionsFailed++;
                Console.WriteLine("AssertTrue failed in " + new StackFrame(1, true).GetMethod().ToString());
            }
        }
        protected void AssertFalse(bool one) {
            assertionsTotal++;
            if (one) {
                assertionsFailed++;
                Console.WriteLine("AssertFalse failed in " + new StackFrame(1, true).GetMethod().ToString());
            }
        }
        protected void AssertEqual(int one, int two) {
            assertionsTotal++;
            if (!(one == two)) {
                assertionsFailed++;
                Console.WriteLine(one + "==" + two + " failed in " + new StackFrame(1, true).GetMethod().ToString());
            }
        }
    }
}
