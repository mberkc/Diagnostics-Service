using System;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Handles runtime diagnostics such as memory warnings and log messages.
/// </summary>  
public sealed class DiagnosticsService : MonoBehaviour
{
    private static DiagnosticsService _instance; // could be public if needed
    private IServerCommunicator _serverCommunicator;
    
    private class MemoryDiagnosticsPayload
    {
        internal int memory_size;
        internal int graphics_memory_size;
    }
    
    private class LogDiagnosticsPayload
    {
        internal string function_involved;
        internal string condition, stackTrace;
        internal LogType type;
    }

    private const float MemoryLogInterval = 30f; // Time interval to limit memory log spam
    private float lastMemoryLogTime = -MemoryLogInterval;
    
    // For testing purposes
    public void Initialize(IServerCommunicator serverCommunicator)
    {
        _serverCommunicator = serverCommunicator;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            _serverCommunicator ??= new ServerCommunicatorWrapper();
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Subscribes to Unity's low memory and log message callbacks on enabling the object.
    /// </summary>
    private void OnEnable()
    {
        Application.lowMemory += LowMemoryCallback;
        Application.logMessageReceivedThreaded += LogCallback;
    }
    
    /// <summary>
    /// Unsubscribes from Unity's low memory and log message callbacks when disabling the object.
    /// </summary>
    private void OnDisable()
    {
        Application.lowMemory -= LowMemoryCallback;
        Application.logMessageReceivedThreaded -= LogCallback;
    }
    
    /// <summary>
    /// Triggered on low memory warning from the system. Captures memory data and sends it to the server.
    /// </summary>
    internal void LowMemoryCallback()
    {
        if(Time.time < lastMemoryLogTime + MemoryLogInterval) return; // Prevents spamming
        lastMemoryLogTime = Time.time;
        
        var payload = new MemoryDiagnosticsPayload
        {
            memory_size = SystemInfo.systemMemorySize,
            graphics_memory_size = SystemInfo.graphicsMemorySize
        };

        // Send log diagnostics data to server
        _serverCommunicator.SendDiagnosticsToServer("/memory", payload);
    }

    /// <summary>
    /// Triggered on log message received. Captures relevant log data and sends it to the server if it’s not LogType.log.
    /// </summary>
    /// <param name="condition">The condition that triggered the log message.</param>
    /// <param name="stackTrace">The stack trace of the log.</param>
    /// <param name="type">Type of log (error, warning, etc.).</param>
    internal void LogCallback(string condition, string stackTrace, LogType type)
    {
        if(type == LogType.Log) return; // Skip non-critical logs
        // If requests are frequent they can be sent batched periodically
        
        // Retrieve stack trace if empty
        if(string.IsNullOrEmpty(stackTrace))
            stackTrace = new StackTrace(true).ToString();
        
        // Attempt to parse function name from the stack trace
        var emptySpaceIndex = stackTrace.IndexOf(" ", StringComparison.Ordinal);
        var functionInvolved = emptySpaceIndex > 0 ? stackTrace[..emptySpaceIndex] : condition;

        var payload = new LogDiagnosticsPayload
        {
            function_involved = functionInvolved,
            condition = condition,
            stackTrace = stackTrace,
            type = type
        };

        // Send log diagnostics data to server
        _serverCommunicator.SendDiagnosticsToServer("/log", payload);
    }
}
