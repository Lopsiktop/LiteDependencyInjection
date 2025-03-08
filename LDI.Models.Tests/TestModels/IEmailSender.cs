namespace LDI.Models.Tests
{
    public interface IEmailSender
    {
        void SendEmail(string email, string msg);
    }
}