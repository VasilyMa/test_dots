using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.Rendering;

public class MobileLogCollector : MonoBehaviour
{
    private StringBuilder _logBuilder;
    private string _logFilePath;

    void Awake()
    {
        _logBuilder = new StringBuilder();
        _logFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");

        Application.logMessageReceived += OnLogReceived;

        LogSystemInfo();
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= OnLogReceived;
    }

    private void OnLogReceived(string condition, string stackTrace, LogType type)
    {
        _logBuilder.AppendLine($"[{type}] {condition}");

        if (type == LogType.Error || type == LogType.Exception)
        {
            _logBuilder.AppendLine(stackTrace);
        }

        _logBuilder.AppendLine("--------------------------------------------------");
    }

    private void LogSystemInfo()
    {
        _logBuilder.AppendLine("=== SYSTEM INFO ===");
        _logBuilder.AppendLine($"Device: {SystemInfo.deviceModel}");
        _logBuilder.AppendLine($"OS: {SystemInfo.operatingSystem}");
        _logBuilder.AppendLine($"GPU: {SystemInfo.graphicsDeviceName}");
        _logBuilder.AppendLine($"GPU API: {SystemInfo.graphicsDeviceType}");
        _logBuilder.AppendLine($"Shader Level: {SystemInfo.graphicsShaderLevel}");
        _logBuilder.AppendLine($"RAM: {SystemInfo.systemMemorySize} MB");
        _logBuilder.AppendLine($"VRAM: {SystemInfo.graphicsMemorySize} MB");
        _logBuilder.AppendLine($"CPU: {SystemInfo.processorType}");
        _logBuilder.AppendLine($"Cores: {SystemInfo.processorCount}");
        _logBuilder.AppendLine($"URP: {GraphicsSettings.currentRenderPipeline?.name}");
        _logBuilder.AppendLine("===================\n");
    }
     
    public void SaveLogAndCopyToClipboard()
    {
        File.WriteAllText(_logFilePath, _logBuilder.ToString());

        GUIUtility.systemCopyBuffer = _logBuilder.ToString();

        Debug.Log($"LOG SAVED: {_logFilePath}");
    }
}
