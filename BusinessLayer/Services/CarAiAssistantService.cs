using BusinessLayer.DTOs;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BusinessLayer.Services
{
    public class CarAiAssistantService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _http;

        public CarAiAssistantService(IConfiguration configuration)
        {
            _configuration = configuration;
            _http = new HttpClient();
        }

        public async Task<string> AskAsync(string userInput, List<VehicleDto> vehicles)
        {
            string apiKey = _configuration["Groq:ApiKey"];

            var vehicleJson = JsonSerializer.Serialize(
                vehicles.Select(v => new
                {
                    v.Id,
                    v.Name,
                    v.Brand,
                    v.VehicleType,
                    Seats = v.seartCapacity,
                    MaxDistance = v.MaxDistance,
                    Status = v.Status.ToString()
                })
            );

            var body = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
        new { role = "system", content =
@"You are a smart EV car rent assistant. 
Your task is to recommend cars for rental based on user request. 

Rules:
1. Always return a single JSON object:
{
  ""Summary"": ""<summary text>"",
  ""Cars"": [
    { ""Id"": number, ""Name"": string, ""Reason"": ""<reason using actual car info (seats, max distance, battery, price, etc.)>"" }
  ]
}

2. If the user request specifies a number of people, trip distance, or preferences:
   - Compute whether each car can make the trip based on its MaxDistance.
   - If MaxDistance < trip distance, mention that the car is suitable but the user will need to recharge along the way.
   - Include this reasoning in the ""Reason"" field.
   - Summarize the reasoning in the ""Summary"" field.

3. If the user request is general or a greeting:
   - Provide a friendly greeting in ""Summary"".
   - Provide the JSON array of available cars under ""Cars"".

4. Each ""Reason"" must use the actual car info (seats, max distance, battery, price, etc.) and explain why it is suitable.

5. Do NOT include any text outside the JSON object."},
        new { role = "user", content =
$@"User request: ""{userInput}""
Available cars: {vehicleJson}" }
    },
                temperature = 0.2
            };



            string jsonBody = JsonSerializer.Serialize(body);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.groq.com/openai/v1/chat/completions"
            );

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _http.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseText);
            string answer = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return answer;
        }
    }
}
