
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

        protected string CALLBACK = "http://localhost:3000/"; // your site
        protected string PROTOCOL = Config["protocol"];
        protected string USERVOICE_DOMAIN = Config["uservoice_domain"];
        protected string USERVOICE_SUBDOMAIN = Config["subdomain_name"];
        protected string SSO_KEY = Config["sso_key"];
        protected string API_KEY = Config["api_key"];
        protected string API_SECRET = Config["api_secret"];

        public Test() {
            foreach (MethodInfo method in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)) {
                if (method.Name.StartsWith("Should") || method.Name.StartsWith("Test") ) {
                    try {
                        var failed = assertionsFailed;
                        this.GetType().InvokeMember(method.Name, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, this, null);
                        if (failed > assertionsFailed) {
                            Console.WriteLine(method.Name + " FAILED due to " + (failed - assertionsFailed) + " assertions.");
                        } else {
                            Console.WriteLine(method.Name + " passed.");
                        }
                    } catch (System.Reflection.TargetInvocationException e) {
                        Console.WriteLine(method.Name + " FAILED due to: " + e.InnerException);
                        errors++;
                    }
                }
            }
        }
        private static int assertionsFailed = 0;
        private static int assertionsTotal = 0;
        private static int errors = 0;
        private static NameValueCollection config;
        protected static NameValueCollection Config {
            get {
                if (config == null)  {
                    string line;
                    config = new NameValueCollection();
                    System.IO.StreamReader reader = new System.IO.StreamReader("config.yml");
                    while ((line = reader.ReadLine()) != null) {
                        string [] substrings = Regex.Split(line.Split('#')[0], ": +");
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
        public static void Main() {
            new ClientTest();
            new SSOTest();
            System.Console.WriteLine("Assertions failed: " + assertionsFailed + " / " + assertionsTotal + ", Errors: " + errors);
        }
    }
}
