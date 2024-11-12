public class ServerCommunicatorWrapper : IServerCommunicator
{
    public void SendDiagnosticsToServer(string path, object payload)
    {
        ServerCommunicator.SendDiagnosticsToServer(path, payload);
    }
}