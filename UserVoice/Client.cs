using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System.Web;

namespace UserVoice
{
    public class APIError: Exception { public APIError(string msg): base(msg) {} }
    public class Unauthorized: APIError { public Unauthorized(string msg): base(msg) {} }
    public class NotFound: APIError { public NotFound(string msg): base(msg) {} }
    public class ApplicationError: APIError { public ApplicationError(string msg): base(msg) {} }

    public class Client
    {
        public const string CLIENT_VERSION = "0.0.4";
        private RestClient consumer;
        private RestClient accessToken;
        private string apiKey;
        private string apiSecret;
        private string uservoiceDomain;
        private string protocol;
        private string subdomainName;
        private string callback;
        public string Token;
        public string Secret;
        private Client requestToken = null;

        public Client(string subdomainName, string apiKey, string apiSecret=null, string callback=null, string token=null, string secret=null, string uservoiceDomain=null, string protocol=null) {
            this.protocol = protocol ?? "https";
            this.uservoiceDomain = uservoiceDomain ?? "uservoice.com";
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            this.subdomainName = subdomainName;
            this.callback = callback;
            this.Token = token;
            this.Secret = secret;
            consumer = new RestClient(this.protocol + "://" + this.subdomainName + "." + this.uservoiceDomain);
            if (apiSecret != null) {
                consumer.Authenticator = OAuth1Authenticator.ForRequestToken(apiKey, apiSecret, callback);
            }
            if (token != null && secret != null) {
                accessToken = new RestClient(this.protocol + "://" + this.subdomainName + "." + this.uservoiceDomain);
                accessToken.Authenticator = OAuth1Authenticator.ForAccessToken(apiKey, apiSecret, token, secret);
            }
        }
        private RestClient getToken() {
            if (accessToken != null) {
                return accessToken;
            }
            return consumer;
        }

        public JToken Request(Method method, string path, Object body=null) {
            //Console.WriteLine(method + " " + path + "\n" + body);
            var request = new RestRequest(path.Split('?').First(), method);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("API-Client", string.Format("uservoice-csharp-{0}", CLIENT_VERSION));
            if (body != null) {
                //Console.WriteLine("BODY PARAMETER " + body.ToString());
                request.AddParameter("application/json", JsonConvert.SerializeObject(body), ParameterType.RequestBody);
            }
            if (body == null) {
                var queryString = string.Join(string.Empty, path.Split('?').Skip(1));
                var getParams = System.Web.HttpUtility.ParseQueryString(queryString);
                if (null != getParams) {
                    foreach (string k in getParams.AllKeys) {
                        //Console.WriteLine("GET PARAMTER " + k + "=" + getParams[k]);
                        request.AddParameter(k, getParams[k], ParameterType.GetOrPost);
                    }
                }
            }
            if (apiSecret == null) {
                request.AddParameter("client", apiKey, ParameterType.GetOrPost);
            }
            var response = getToken().Execute(request);

            JToken result = null;
            try {
                if (response.ContentType.StartsWith("application/json")) {
                    result = JObject.Parse(response.Content);
                } else {
                    result = new JObject();
                    var values = HttpUtility.ParseQueryString(response.Content);
                    if (null != values.AllKeys) {
                        foreach (String k in values.AllKeys) {
                            if (null != k) {
                                result[k] = values[k];
                            }
                        }
                    }
                }
            } catch (Newtonsoft.Json.JsonReaderException) {
                throw new ApplicationError("Invalid JSON received: " + response.Content);
            }

            if (!HttpStatusCode.OK.Equals(response.StatusCode)) {
                string msg = response.Content;
                
                if (result != null && null != result["errors"] && null != result["errors"]["message"]) {
                    msg = result["errors"].Value<string>("message");
                }
                switch((int)response.StatusCode) {
                    case 401: throw new Unauthorized(msg);
                    case 404: throw new NotFound(msg);
                    case 500: throw new ApplicationError(msg);
                    default: throw new APIError(msg);
                }
            }
            return result;
        }
        public Client LoginWithAccessToken(string token, string secret) {
            return new Client(subdomainName, apiKey, apiSecret, callback, token, secret, uservoiceDomain, protocol);
        }
        public Client RequestToken(string callback = null) {
            var request = new RestRequest("/oauth/request_token", Method.POST);
            var response = consumer.Execute(request);
            var result = HttpUtility.ParseQueryString(response.Content);

            if (null == result || null == result["oauth_token"] || null == result["oauth_token_secret"]) {
                throw new Unauthorized("Failed to get request token");
            }
            return LoginWithAccessToken((string)result["oauth_token"], (string)result["oauth_token_secret"]);
        }
        public Client LoginAs(string email) {
            JObject parameters = new JObject();
            parameters["user"] = new JObject();
            parameters["user"]["email"] = email;
            parameters["request_token"] = RequestToken().Token;
            JToken result = Request(Method.POST, "/api/v1/users/login_as", parameters);
            return LoginWithAccessToken((string)result["token"]["oauth_token"], (string)result["token"]["oauth_token_secret"]);
        }
        public Client LoginAsOwner() {
            JObject parameters = new JObject();
            parameters["request_token"] = RequestToken().Token;
            JToken result = Request(Method.POST, "/api/v1/users/login_as_owner", parameters);
            return LoginWithAccessToken((string)result["token"]["oauth_token"], (string)result["token"]["oauth_token_secret"]);
        }
        public string AuthorizeURL() {
            requestToken = RequestToken();
            return requestToken.getToken().BaseUrl + "/oauth/authorize?oauth_token=" + requestToken.Token;
        }
        public Client LoginWithVerifier(string verifier) {
            var result = requestToken.Post("/oauth/access_token", new { oauth_verifier=verifier });
            if (null == result || null == result["oauth_token"] || null == result["oauth_token_secret"]) {
                throw new Unauthorized("Failed to get request token");
            }
            return LoginWithAccessToken((string)result["oauth_token"], (string)result["oauth_token_secret"]);
        }
        public UserVoice.Collection GetCollection(string path, int? limit=null) { return new UserVoice.Collection(this, path, limit); }

        public JToken Get(string path) { return Request(Method.GET, path); }
        public JToken Delete(string path) { return Request(Method.DELETE, path); }
        public JToken Post(string path, Object parameters) { return Request(Method.POST, path, parameters); }
        public JToken Put(string path, Object parameters) { return Request(Method.PUT, path, parameters); }
    }
}
