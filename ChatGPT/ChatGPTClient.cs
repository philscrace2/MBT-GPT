using ChatGPT;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ChatGPT
{
    public class ChatGPTClient
    {
        private readonly string _apiKey;
        private readonly RestClient _client;

        // Constructor that takes the API key as a parameter
        public ChatGPTClient(string apiKey)
        {
            _apiKey = apiKey;
            // Initialize the RestClient with the ChatGPT API endpoint
            _client = new RestClient("https://api.openai.com/v1/chat/completions");
        }

        public ChatGPTClient() : this(LoadApiKey())
        {

        }

        public static string LoadApiKey()
        {
            var key = Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.Machine);
            if (key is null)
            {
                Console.WriteLine("Please enter your OpenAI API key " +
                                    "(you can get it from https://platform.openai.com/account/api-keys): ");
                key = Console.ReadLine();
                if (key is null)
                {
                    throw new Exception("API key is not provided");
                }
            }

            return key;
        }

        // Method to send a message to the ChatGPT API and return the response
        public string SendMessage(string message)
        {
            // Check for empty input
            if (string.IsNullOrWhiteSpace(message))
            {
                return "Sorry, I didn't receive any input. Please try again!";
            }

            try
            {
                // Create a new POST request
                var request = new RestRequest("", Method.Post);
                // Set the Content-Type header
                request.AddHeader("Content-Type", "application/json");
                // Set the Authorization header with the API key
                request.AddHeader("Authorization", $"Bearer {_apiKey}");

                var chatRequest = new ChatGptRequest
                {
                    Model = "gpt-3.5-turbo",
                    Messages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    Role = "system",
                    Content = "You are a helpful assistant."
                },
                new ChatMessage
                {
                    Role = "user",
                    Content = message
                }
            }
                };

                //Add the JSON body to the request
                request.AddJsonBody(JsonConvert.SerializeObject(chatRequest));

                // Execute the request and receive the response
                var response = _client.Execute(request);

                // Deserialize the response JSON content
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);

                JObject parsedJson = JObject.Parse(jsonResponse.ToString());

                string content = parsedJson["choices"]?[0]?["message"]?["content"]?.ToString();

                return content;

                //return jsonResponse?.choices[0]?.text?.ToString()?.Trim();
            }

            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the API request
                Console.WriteLine($"Error: {ex.Message}");
                return "Sorry, there was an error processing your request. Please try again later.";
            }
        }
    }
}