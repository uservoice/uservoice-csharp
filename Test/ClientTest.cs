namespace Test
{
    using System.Linq;

    public class ClientTest: Test {

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
    }
}
