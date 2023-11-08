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
            template.AppendLine("<p>Hi <strong>@Model.FullName</strong>!</p>");
            template.AppendLine("<p>A new blog post has been published under the headline of <strong>@Model.PostTopic</strong>!</p>");
            template.AppendLine("<p>Don't forget to read the article!</p>");
            template.AppendLine("<p>Thank you,<br>Mostafa Ashraf</p>");
            
            var model = new EmailModel(
                fullName,
                postTopic);

            await fluentEmail
                .To(emailAddress)
                .Subject("New Blog Post - Taa'mona")
                .UsingTemplate(template.ToString(), model)
                .SendAsync();
        }

        public async Task SendEmailWithLinkOfPasswordChangePage(string emailAddress)
        {
            StringBuilder template = new();
            template.AppendLine("To change your old password with a new one, click here: <a href='@Model.PasswordChangePageLink'>Password Change Link!</a>");

            var model = new { PasswordChangePageLink = "https://localhost:7058/Logger/AssignNewPasswordToSystemLogger" };

            await fluentEmail
                .To(emailAddress)
                .Subject("Password Change Email - Taa'mona")
                .UsingTemplate(template.ToString(), model)
                .SendAsync();
        }
    }
}
