namespace LDI.Models.Tests
{
    public interface ILogger
    {
        string Name { get; set; }
        void Send(string msg);
    }
}