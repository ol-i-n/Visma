using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp.Authenticators;
using System.Text.Json.Nodes;

namespace Visma
{
    // Sample Book model matching the API response structure
    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        // Add other properties as needed
    }

    [TestClass]
    public class APITests
    {

        [TestMethod]
        public void TestBooksAPI()
        {
            var client = new RestClient("https://demoqa.com/");
            var request = new RestRequest("BookStore/v1/Books", Method.Get);
            RestResponse response = client.Execute(request);

            // Pretty-printing the JSON response
            var parsedJson = JObject.Parse(response.Content);
            string formattedJson = parsedJson.ToString(Formatting.Indented);

            TestContext.WriteLine("Response Body:");
            TestContext.WriteLine(formattedJson);

            Assert.IsTrue(response.IsSuccessful, $"API Call failed with status code: {response.StatusCode}");
        }


        [TestMethod]
        public void TestCreateBooksAPI()
        {
            List<String> credential = TestCreateNewUser();
            string username = credential[0];
            string password = credential[1];
            string userId = credential[2];

            var options = new RestClientOptions("https://demoqa.com/")
            {
                Authenticator = new HttpBasicAuthenticator(username, password)
            };
            var client = new RestClient(options);
            var request = new RestRequest("BookStore/v1/Books", Method.Post);
           
            string payload = string.Format(@"{{""userId"": ""{0}"", ""collectionOfIsbns"": [{{""isbn"": ""test1""}}]}}", userId);


            request.AddStringBody(payload, ContentType.Json);

            RestResponse response = client.Execute(request);
            Assert.IsTrue(response.StatusCode.ToString().Equals("BadRequest"));
        }

        public List<String> TestCreateNewUser()
        {
            var client = new RestClient("https://demoqa.com/");
            var request = new RestRequest("Account/v1/User", Method.Post);
            string userName = "UserName " + Guid.NewGuid().ToString() + "Z@";
            string password = "Password " + Guid.NewGuid().ToString() + "Z@";
            string payload = string.Format(@"{{""userName"": ""{0}"",""password"": ""{1}""}}", userName, password);

            request.AddStringBody(payload, ContentType.Json);

            RestResponse response = client.Execute(request);
            Assert.IsTrue(response.IsSuccessful, $"API Call failed with status code: {response.StatusCode}");

            JObject parsedJson = JObject.Parse(response.Content);
            String userId = parsedJson["userID"].ToString();
            List<String> credential = new List<String>();
            credential.Add(userName);
            credential.Add(password);
            credential.Add(userId);
            return credential;
        }



        // This is the TestContext property
        public TestContext TestContext { get; set; }
    }
}
