namespace Test
{
    using System;
    using System.Linq;

    public class ClientTest: Test {

        private UserVoice.Client getSignedClient() {
            return new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET, CALLBACK, null, null, USERVOICE_DOMAIN, PROTOCOL);
        }
        private UserVoice.Client getUnsignedClient() {
            return new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, null, null, null, null, USERVOICE_DOMAIN, PROTOCOL);
        }

        public void ShouldGetCurrentUserEmail() {
            UserVoice.Client client = getSignedClient();
            client = client.LoginAs("user@example.com");
            AssertEqual("user@example.com", (string)client.Get("/api/v1/users/current")["user"]["email"]);
        }

        public void ShouldGetPublicForumData() {
            UserVoice.Client publicClient = getUnsignedClient();
            AssertEqual(false, (bool)publicClient.GetCollection("/api/v1/forums", 1)[0]["private"]);
        }

        public void ShouldLoginAsOwner() {
            UserVoice.Client client = getSignedClient();
            client = client.LoginAsOwner();
            AssertTrue((bool)client.Get("/api/v1/users/current")["user"]["roles"]["owner"]);
        }

        public void ShouldGet10Users() {
            UserVoice.Client client = getSignedClient();
            AssertEqual(client.Get("/api/v1/users")["users"].Count(), 10);
        }

        public void ShouldReuseAccessToken() {
            const string email = "user@example.com";
            UserVoice.Client client = getSignedClient();
            UserVoice.Client userToken = client.LoginAs(email);

            UserVoice.Client copiedToken = getSignedClient()
                .LoginWithAccessToken(userToken.Token, userToken.Secret);
            AssertEqual((string)copiedToken.Get("/api/v1/users/current")["user"]["email"], email);
        }
        public void ShouldGetOnly3Users() {
            UserVoice.Client client = getSignedClient();
            UserVoice.Collection users = client.LoginAsOwner().GetCollection("/api/v1/users", 3);
            AssertEqual(3, users.Count());
        }
        public void ShouldThrowExceptionOnOutOfRangeReference() {
            var k = false;
            try {
                UserVoice.Client client = getSignedClient();
                UserVoice.Collection users = client.LoginAsOwner().GetCollection("/api/v1/users", 3);
                Console.WriteLine("Getting user out of range FAILED: " + users[3]);
            } catch (IndexOutOfRangeException e) {
                e = e;
                k = true;
            }
            AssertTrue(k);
        }
        public void ShouldGetMoreThan10Users() {
            UserVoice.Client client = getSignedClient();
            UserVoice.Collection users = client.LoginAsOwner().GetCollection("/api/v1/users");
            int times = 3;
            foreach (var k in users) {
                if (--times <= 0) { break; }
                //Console.WriteLine("User: " + k);
            }
            AssertTrue(users.Count() > 10);
        }
        public void ShouldForLoopMoreThan10Users() {
            UserVoice.Client client = getSignedClient();
            UserVoice.Collection users = client.LoginAsOwner().GetCollection("/api/v1/users");
            int times = 3;
            for (int i = 0; i < users.Count(); i++) {
                if (--times <= 0) { break; }
                //Console.WriteLine("User: " + users[i]);
            }
            AssertTrue(users.Count() > 10);
        }
    }
}
