namespace Test
{
    using Newtonsoft.Json.Linq;

    public class SSOTest: Test {

        public void ShouldGenerateSSOToken() {
            JToken user = new JObject();
            user["email"] = "tester@example.com";
            user["trusted"] = true;
            user["guid"] = 12343;
            user["expires"] = "2012-12-12 12:12:12";

            string token = UserVoice.SSO.GenerateToken(Config["subdomain_name"], Config["sso_key"], user);
            //System.Console.WriteLine(string.Format("https://{0}.uservoice.com?sso={1}", Config["subdomain_name"], token));
            AssertTrue(token.Length > 10);
        }
    }
}
