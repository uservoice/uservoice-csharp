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
            UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET);
            client = client.LoginAs("user@example.com");
            AssertEqual("user@example.com", (string)client.Get("/api/v1/users/current")["user"]["email"]);
        }

        public void ShouldLoginAsOwner() {
            UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET);
            client = client.LoginAsOwner();
            AssertTrue((bool)client.Get("/api/v1/users/current")["user"]["roles"]["owner"]);
        }

        public void ShouldGet10Users() {
            UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET);
            AssertEqual(client.Get("/api/v1/users")["users"].Count(), 10);
        }

        public void ShouldReuseAccessToken() {
            const string email = "user@example.com";
            UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET);
            UserVoice.Client userToken = client.LoginAs(email);

            UserVoice.Client copiedToken = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET)
                .LoginWithAccessToken(userToken.Token, userToken.Secret);
            AssertEqual((string)copiedToken.Get("/api/v1/users/current")["user"]["email"], email);
        }
        public void ShouldGetOnly3Users() {
            UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET);
            UserVoice.Collection users = client.LoginAsOwner().GetCollection("/api/v1/users", 3);
            AssertEqual(3, users.Count());
        }
        public void ShouldThrowExceptionOnOutOfRangeReference() {
            var k = false;
            try {
                UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET);
                UserVoice.Collection users = client.LoginAsOwner().GetCollection("/api/v1/users", 3);
                Console.WriteLine("Getting user out of range FAILED: " + users[3]);
            } catch (IndexOutOfRangeException e) {
                k = true;
            }
            AssertTrue(k);
        }
        public void ShouldGetMoreThan10Users() {
            UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET);
            UserVoice.Collection users = client.LoginAsOwner().GetCollection("/api/v1/users");
            foreach (var k in users) {
                //Console.WriteLine("User: " + k);
            }
            AssertTrue(users.Count() > 10);
        }
        public void ShouldForLoopMoreThan10Users() {
            UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET);
            UserVoice.Collection users = client.LoginAsOwner().GetCollection("/api/v1/users");
            for (int i = 0; i < users.Count(); i++) {
                //Console.WriteLine("User: " + users[i]);
            }
        }
    }
}
