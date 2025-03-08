namespace LDI.Models.Tests
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger logger;

        public EmailSender(ILogger logger)
        {
            this.logger = logger;
        }

        public void SendEmail(string email, string msg)
        {
            // logic of send email
            logger.Send($"Send message: {msg} to email: {email}");
        }
    }
}