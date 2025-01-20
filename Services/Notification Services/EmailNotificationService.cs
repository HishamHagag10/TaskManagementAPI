
using Microsoft.Extensions.Configuration;
using RestSharp.Authenticators;
using RestSharp;

namespace TaskManagement.API.Services.NotificationService
{
    public class EmailNotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;

        public EmailNotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Notify(string recipient, string subject, string body)
        {
            var option = new RestClientOptions(_configuration.GetSection("EmailConfig:Url").Value)
            {
                Authenticator = new HttpBasicAuthenticator("api",
                _configuration.GetSection("EmailConfig:API_KEY").Value)
            };
            var client = new RestClient(option);

            var request = new RestRequest("", Method.Post);
            request.AddParameter("domain", _configuration.GetSection("EmailConfig:Domain").Value,
                ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";

            request.AddParameter("from", _configuration.GetSection("EmailConfig:From").Value);
            request.AddParameter("to", recipient);

            request.AddParameter("subject", subject);
            request.AddParameter("html", body);
            request.Method = Method.Post;

            var response = client.Execute(request);
            return response.IsSuccessful;

        }
    }
}
