namespace Test
{
    using Newtonsoft.Json.Linq;

    public class SSOTest: Test {

        public void ShouldGenerateSSOToken() {


            string token = UserVoice.SSO.GenerateToken(Config["subdomain_name"], Config["sso_key"], new {
                email = "tester@example.com",
                trusted = true,
                guid = 12343 //, expires = "2012-12-12 12:12:12"
            }, 5);
            //System.Console.WriteLine(string.Format("https://{0}.uservoice.com?sso={1}", Config["subdomain_name"], token));
            AssertTrue(token.Length > 10);
        }
    }
}
