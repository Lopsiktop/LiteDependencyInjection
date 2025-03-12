namespace LDI.Models.Tests.TestModels
{
    public class TxtLogger : ILogger
    {
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Send(string msg)
        {
            //log in to file
        }
    }
}