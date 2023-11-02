using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        // This is the TestContext property
        public TestContext TestContext { get; set; }
    }
}
