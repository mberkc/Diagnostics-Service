public interface IServerCommunicator
{
    void SendDiagnosticsToServer(string path, object payload);
}