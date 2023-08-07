using FluentEmail.Core;
using System.Text;

namespace HealthyFoodWebsite.EmailTemplate
{
    public class EmailFactory
    {
        // Object Fields Zone
        private readonly IFluentEmail fluentEmail;

        
        // Dependency Injection Zone
        public EmailFactory(IFluentEmail fluentEmail)
            => this.fluentEmail = fluentEmail;


        // Object Methods Zone
        public async Task NotifyBlogSubscriberAsync(string postTopic, string fullName, string emailAddress)
        {
            StringBuilder template = new();
            template.AppendLine("Hi @Model.FullName! This is an email, sent to your address @Model.EmailAddress");
            template.AppendLine("<h1>Thanks, Mostafa Ashraf</h1>");
            
            var model = new EmailModel(
                fullName,
                emailAddress);

            await fluentEmail
                .To(emailAddress)
                .Subject(postTopic)
                .UsingTemplate(template.ToString(), model)
                .SendAsync();
        }
    }
}
