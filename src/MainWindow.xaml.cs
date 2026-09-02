using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Win32;
using WuWaServerLauncher.Models;
using WuWaServerLauncher.Services;

namespace WuWaServerLauncher;

public partial class MainWindow : Window
{
    private readonly SettingsService _settingsService = new();
    private readonly GameLauncher _gameLauncher = new();
    private readonly LauncherSettings _settings;
    private ServerProfile _selectedServer = null!;

    public ObservableCollection<ServerProfile> Servers { get; } = new();

    public MainWindow()
    {
        InitializeComponent();

        _settings = _settingsService.Load();

        foreach (var profile in _settings.Servers.Values.OrderBy(GetSortOrder))
        {
            profile.RefreshStatus();
            Servers.Add(profile);
        }

        DataContext = this;

        var initial = Servers.FirstOrDefault(x => x.Id == _settings.LastServer) ?? Servers.First();
        SelectServer(initial);
    }

    protected override void OnClosed(EventArgs e)
    {
        SaveSettings();
        base.OnClosed(e);
    }

    private static int GetSortOrder(ServerProfile profile)
        => profile.Id switch
        {
            "cn" => 0,
            "bilibili" => 1,
            "global" => 2,
            _ => 99
        };

    private void ServerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { Tag: ServerProfile profile })
            SelectServer(profile);
    }

    private void SelectServer(ServerProfile profile)
    {
        foreach (var server in Servers)
            server.IsSelected = ReferenceEquals(server, profile);

        _selectedServer = profile;
        _settings.LastServer = profile.Id;
        SelectedServerText.Text = profile.Name;
        ExecutablePathTextBox.Text = string.IsNullOrWhiteSpace(profile.ExecutablePath)
            ? "尚未选择启动程序"
            : profile.ExecutablePath;
        LaunchButton.Content = $"启动 {_selectedServer.Name}";
        LaunchButton.IsEnabled = File.Exists(_selectedServer.ExecutablePath);
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedServer is null)
            return;

        var dialog = new OpenFileDialog
        {
            Title = $"选择 {_selectedServer.Name} 的启动程序",
            Filter = "可执行文件 (*.exe)|*.exe",
            CheckFileExists = true,
            Multiselect = false
        };

        if (File.Exists(_selectedServer.ExecutablePath))
            dialog.InitialDirectory = Path.GetDirectoryName(_selectedServer.ExecutablePath);

        if (dialog.ShowDialog(this) != true)
            return;

        _selectedServer.ExecutablePath = dialog.FileName;
        SaveSettings();
        SelectServer(_selectedServer);
    }

    private void LaunchButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedServer is null)
            return;

        try
        {
            SaveSettings();
            _gameLauncher.Launch(_selectedServer);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                $"启动 {_selectedServer.Name} 失败",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void SaveSettings()
    {
        _settings.Servers = Servers.ToDictionary(x => x.Id, x => x);
        _settingsService.Save(_settings);
    }
}
