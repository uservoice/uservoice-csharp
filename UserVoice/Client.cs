using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Contrib;

namespace UserVoice
{
    class APIError: Exception { public APIError(string msg): base(msg) {} }
    class Unauthorized: APIError { public Unauthorized(string msg): base(msg) {} }
    class NotFound: APIError { public NotFound(string msg): base(msg) {} }
    class ApplicationError: APIError { public ApplicationError(string msg): base(msg) {} }

    public class Client
    {
        private RestClient AccessToken;
        private string SubdomainName;
        private string ApiKey;
        private string ApiSecret;
        private string UservoiceDomain;
        private string Callback;
        private string Protocol;
        public string Token;
        public string Secret;

        public Client(string subdomainName, string apiKey, string apiSecret, string callback=null, string accessToken="", string accessTokenSecret="", string uservoiceDomain="uservoice.com", string protocol="https") {
            SubdomainName= subdomainName;
            ApiKey = apiKey;
            ApiSecret = apiSecret;
            UservoiceDomain = uservoiceDomain;
            Protocol = protocol;
            Callback = callback;
            Token = accessToken;
            Secret = accessTokenSecret;
            AccessToken = new RestClient(protocol + "://" + subdomainName + "." + uservoiceDomain);

            if (accessToken == "" && accessTokenSecret == "") {
                AccessToken.Authenticator = OAuth1Authenticator.ForRequestToken(apiKey, apiSecret);
            } else {
                AccessToken.Authenticator = OAuth1Authenticator.ForAccessToken(apiKey, apiSecret, accessToken, accessTokenSecret);
            }
        }

        public JToken Request(Method method, string path, JToken body=null) {
            var request = new RestRequest(path, method);
            if (body != null) {
                request.AddParameter("application/json", body, ParameterType.RequestBody);
            }
            request.AddHeader("Accept", "application/json");
            var response = AccessToken.Execute(request);

            JToken result = null;
            try {
                if (response.ContentType.StartsWith("application/json")) {
                    result = JObject.Parse(response.Content);
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
            return new Client(SubdomainName, ApiKey, ApiSecret, Callback, token, secret, UservoiceDomain, Protocol);
        }
        public Client RequestToken(string callback = null) {
            JObject parameters = new JObject();
            if (Callback != null) {
                parameters["oauth_callback"] = Callback;
            } else if (callback != null) {
                parameters["oauth_callback"] = callback;
            } else {
                parameters = null;
            }
            JToken result = Request(Method.POST, "/api/v1/oauth/request_token", parameters);
            if (null == result || null == result["token"] || null == result["token"]["oauth_token"]) {
                throw new Unauthorized("Failed to get request token");
            }
            return LoginWithAccessToken((string)result["token"]["oauth_token"], (string)result["token"]["oauth_token_secret"]);
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

        public JToken Get(string path) { return Request(Method.GET, path); }
        public JToken Delete(string path) { return Request(Method.DELETE, path); }
        public JToken Post(string path, JToken parameters) { return Request(Method.POST, path, parameters); }
        public JToken Put(string path, JToken parameters) { return Request(Method.PUT, path, parameters); }
    }
}
