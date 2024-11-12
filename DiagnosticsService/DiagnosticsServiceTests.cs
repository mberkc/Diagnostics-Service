using Moq;
using UnityEngine;
using NUnit.Framework;

public class DiagnosticsServiceTests
{
    private Mock<IServerCommunicator> _serverCommunicatorMock;
    private DiagnosticsService _service;
    
    [SetUp]
    public void SetUp()
    {
        _serverCommunicatorMock = new Mock<IServerCommunicator>();
        _service = new GameObject().AddComponent<DiagnosticsService>();
        _service.Initialize(_serverCommunicatorMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(_service.gameObject);
    }

    [Test]
    public void LowMemoryCallback_ShouldSendDiagnosticsData()
    {
        // Simulating low memory callback
        _service.LowMemoryCallback();

        // Verify if SendDiagnosticsToServer was called with the correct endpoint
        _serverCommunicatorMock.Verify(mock => mock.SendDiagnosticsToServer(
            "/memory", 
            It.IsAny<object>()),
            Times.Once
        );
    }
    
    [Test]
    public void LogCallback_ShouldNotSendDiagnosticsData_WhenLogTypeIsLog()
    {
        // Simulating with LogType.Log
        _service.LogCallback("Test condition", "Test stack trace", LogType.Log);

        // Verify that SendDiagnosticsToServer was not called
        _serverCommunicatorMock.Verify(mock => mock.SendDiagnosticsToServer(
                It.IsAny<string>(), 
                It.IsAny<object>()),
            Times.Never
        );
    }

    [Test]
    public void LogCallback_ShouldSendDiagnosticsData_WhenLogTypeIsWarning()
    {
        // Simulating log callback
        _service.LogCallback("Test condition", "Test stack trace", LogType.Warning);

        // Verify if SendDiagnosticsToServer was called with the correct endpoint
        _serverCommunicatorMock.Verify(mock => mock.SendDiagnosticsToServer(
            "/log", 
            It.IsAny<object>()),
            Times.Once
        );
    }
}