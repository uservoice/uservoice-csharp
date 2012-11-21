namespace Test
{
    using System;
    using System.Linq;

    public class ClientTest: Test {

string callbackURL = "http://localhost:3000/"; // your site
string USERVOICE_SUBDOMAIN = Config["subdomain_name"];
string SSO_KEY = Config["sso_key"];
string API_KEY = Config["api_key"];
string API_SECRET = Config["api_secret"];

        public void ShouldGetCurrentUserEmail() {
            UserVoice.Client client = new UserVoice.Client(Config["subdomain_name"], Config["api_key"], Config["api_secret"]);
            client = client.LoginAs("user@example.com");
            AssertEqual("user@example.com", (string)client.Get("/api/v1/users/current")["user"]["email"]);
        }

        public void ShouldLoginAsOwner() {
            UserVoice.Client client = new UserVoice.Client(Config["subdomain_name"], Config["api_key"], Config["api_secret"]);
            client = client.LoginAsOwner();
            AssertTrue((bool)client.Get("/api/v1/users/current")["user"]["roles"]["owner"]);
        }

        public void ShouldGet10Users() {
            UserVoice.Client client = new UserVoice.Client(Config["subdomain_name"], Config["api_key"], Config["api_secret"]);
            AssertEqual(client.Get("/api/v1/users")["users"].Count(), 10);
        }

        public void ShouldReuseAccessToken() {
            const string email = "user@example.com";
            UserVoice.Client client = new UserVoice.Client(Config["subdomain_name"], Config["api_key"], Config["api_secret"]);
            UserVoice.Client userToken = client.LoginAs(email);

            UserVoice.Client copiedToken = new UserVoice.Client(Config["subdomain_name"], Config["api_key"], Config["api_secret"])
                .LoginWithAccessToken(userToken.Token, userToken.Secret);
            AssertEqual((string)copiedToken.Get("/api/v1/users/current")["user"]["email"], email);
        }
        public void ShouldDo () {
            UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET);
            UserVoice.Collection users = client.GetCollection("/api/v1/users");
            Console.WriteLine(string.Format("Subdomain \"{0}\" has {1} users.", USERVOICE_SUBDOMAIN, users.Count));
        }
    }
}
