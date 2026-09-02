using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace WuWaServerLauncher.Models;

public sealed class ServerProfile : INotifyPropertyChanged
{
    private string _executablePath = string.Empty;
    private string _statusText = "未配置";
    private bool _isSelected;

    public string Id { get; set; } = string.Empty;
    public string AppId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string ExecutablePath
    {
        get => _executablePath;
        set
        {
            if (_executablePath == value) return;
            _executablePath = value;
            OnPropertyChanged();
            RefreshStatus();
        }
    }

    [JsonIgnore]
    public string StatusText
    {
        get => _statusText;
        private set
        {
            if (_statusText == value) return;
            _statusText = value;
            OnPropertyChanged();
        }
    }

    [JsonIgnore]
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public void RefreshStatus()
    {
        StatusText = string.IsNullOrWhiteSpace(ExecutablePath)
            ? "尚未配置启动程序"
            : File.Exists(ExecutablePath) ? "客户端已就绪" : "启动程序不存在";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
