UserVoice C# library for API connections
========================================

This library allows you to easily:
* Do 3-legged and 2-legged UserVoice API calls safely without having to worry about the cryptographic details.

Installation
============

Run the following command to install uservoice from NuGet. In Visual Studio, you need the NuGet add-on.
```sh
nuget install uservoice
```
Now you should be good to go!

Examples
========

Prerequisites:

* The mcrypt and oauth need to be installed. Check installation instructions above.
* Place the following configuration parameters somewhere in your application:

```csharp
// Suppose your UserVoice site is at http://uservoice-subdomain.uservoice.com/
const string USERVOICE_SUBDOMAIN = "uservoice-subdomain";
const string SSO_KEY = "982c88f2df72572859e8e23423eg87ed"; // Admin Console -> Settings -> General -> User Authentication

// Define an API client at: Admin Console -> Settings -> Channels -> API
const string API_KEY = "oQt2BaunWNuainc8BvZpAm";
const string API_SECRET = "3yQMSoXBpAwuK3nYHR0wpY6opE341inL9a2HynGF2";

```

SSO-token generation using uservoice library
--------------------------------------------

SSO-token can be used to create sessions for SSO users. They are capable of synchronizing the user information from one system to another.
Generating the SSO token from SSO key and given uservoice subdomain can be done by calling UserVoice.SSO.GenerateToken method like this:

```csharp
    string ssoToken = UserVoice.SSO.GenerateToken(USERVOICE_SUBDOMAIN, SSO_KEY, new {
        display_name = "John Doe",
        email = "john.doe@example.com"
    }, 5*60); // the token will be valid for 5 minutes (5*60 seconds) by default

    System.Console.WriteLine(string.Format("https://{0}.uservoice.com/?sso={1}", USERVOICE_SUBDOMAIN, ssoToken));
```

Making API calls
----------------

You need to create an instance of UserVoice\\Client. Get API_KEY and API_SECRET for an API client which you can create
from Admin Console. Go to Settings -> Channels -> API.

```csharp

using System;
try {
    UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET);

    // Get users of a subdomain (requires trusted client, but no user)
    UserVoice.Collection users = client.GetCollection("/api/v1/users");

    Console.WriteLine(string.Format("Subdomain \"{0}\" has {1} users.", USERVOICE_SUBDOMAIN, users.Length));

    foreach (var user in users) {
	Console.WriteLine(string.Format("User: \"{0}\", Profile URL: {1}", (string)user["name"], (string)user["url"]));
    }

    // Now, let's login as mailaddress@example.com, a regular user
    UserVoice.Client regularAccessToken = client.LoginAs("mailaddress@example.com");

    // Example request #1: Get current user.
    var regularUser = regularAccessToken.Get("/api/v1/users/current")["user"];

    Console.WriteLine(string.Format("User: \"{0}\", Profile URL: {1}", (string)regularUser["name"], (string)regularUser["url"]));

    // Login as account owner
    UserVoice.Client ownerAccessToken = client.LoginAsOwner();

    // Example request #2: Create a new private forum limited to only example.com email domain.
    var forum = ownerAccessToken.Post("/api/v1/forums", new {
        forum = new { 
            name = "C# Client Private Feedback",
            @private = true,
            allow_by_email_domain = true,
            allowed_email_domains = new [] { 
                new { domain = "example.com" } 
            }
        }
    })["forum"];

    Console.WriteLine(string.Format("Forum \"{0}\" created! URL: {1}", (string)forum["name"], (string)forum["url"]));
} catch (UserVoice.Unauthorized e) {
    /* Thrown usually due to faulty tokens, untrusted client or if attempting
     * operations without Admin Privileges
     */
    System.Console.WriteLine(e);
} catch (UserVoice.NotFound e) {
    // Thrown when attempting an operation to a resource that does not exist
    System.Console.WriteLine(e);
}

```

Verifying a UserVoice user
--------------------------

If you want to make calls on behalf of a user, but want to make sure he or she
actually owns certain email address in UserVoice, you need to use 3-Legged API
calls. Just pass your user an authorize link to click, so that user may grant
your site permission to access his or her data in UserVoice.

```csharp

const string callbackURL = "http://localhost:3000/"; // your site

UserVoice.Client client = new UserVoice.Client(USERVOICE_SUBDOMAIN, API_KEY, API_SECRET, callbackURL);

// At this point you want to print/redirect to client.AuthorizeURL in your application.
// Here we just output them as this is a command-line example.
System.Console.WriteLine(string.Format("1. Go to {0} and click \"Allow access\".\n", client.AuthorizeURL()));
System.Console.WriteLine("2. Then type the oauth_verifier which is passed as a GET parameter to the callback URL:\n");

// In a web app we would get the oauth_verifier via a redirection to CALLBACK_URL.
// In this command-line example we just read it from stdin:
UserVoice.Client accessToken = client.LoginWithVerifier(readline());

// All done. Now we can read the current user's email address:
JToken user = accessToken.Get("/api/v1/users/current")["user"];


System.Console.WriteLine(string.Format("User logged in, Name: {0}, email: {1}", user["name"], user["email"]));

```