using System.Diagnostics;
using WuWaServerLauncher.Models;

namespace WuWaServerLauncher.Services;

public sealed class GameLauncher
{
    public Process Launch(ServerProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.ExecutablePath))
            throw new InvalidOperationException($"请先配置“{profile.Name}”的启动程序。");

        if (!File.Exists(profile.ExecutablePath))
            throw new FileNotFoundException($"找不到“{profile.Name}”的启动程序。", profile.ExecutablePath);

        var workingDirectory = Path.GetDirectoryName(profile.ExecutablePath);
        if (string.IsNullOrWhiteSpace(workingDirectory))
            throw new InvalidOperationException("无法确定启动程序所在目录。");

        var startInfo = new ProcessStartInfo
        {
            FileName = profile.ExecutablePath,
            WorkingDirectory = workingDirectory,
            UseShellExecute = true
        };

        return Process.Start(startInfo)
            ?? throw new InvalidOperationException("无法启动所选客户端。");
    }
}
