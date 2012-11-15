namespace Test
{
    using UserVoice;
    using System;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;

    public class TestSuite {
        private NameValueCollection config;
        private NameValueCollection Config { get {
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
        }}
        public TestSuite() {
            UserVoice.Client client = new UserVoice.Client(Config["subdomain_name"], Config["api_key"], Config["api_secret"]);
            System.Console.WriteLine(client.LoginAs("tester@example.com").Get("/api/v1/users/current"));
        }

        public static void Main() {
            new TestSuite();
        }
    }
}