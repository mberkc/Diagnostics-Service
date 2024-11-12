# Diagnostics-Service
Handles runtime diagnostics such as memory warnings and log messages.


**Use Case:** In a mobile game, diagnosing runtime issues, such as memory warnings and critical log messages, is essential to maintain performance and stability across devices. Detecting low memory warnings and log messages in real-time enables proactive issue detection and resolution.

**Problem:** Without in-game monitoring, developers can’t capture device-specific runtime issues effectively. This makes it harder for developers to optimize performance and fix bugs that users encounter in real-time.

**Solution:** Implement a DiagnosticsService component that listens for Unity’s low memory and log message callbacks, packages the diagnostic data, and transmits it to a server for backend analysis. This setup provides insights into device performance, log frequencies, and error conditions encountered during gameplay.
