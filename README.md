# WuWa Server Launcher

一个专注于《鸣潮》服务器选择的 Windows 启动器。

## 现在能做什么

打开 Launcher 后，可以直接选择：

- 🇨🇳 官服
- 🇨🇳 B服
- 🌐 国际服

每个服务器分别绑定一个你本机已经安装好的启动程序，然后点击对应的“启动”即可。

界面只保留服务器选择、启动程序选择和启动三个核心动作。

## 使用方式

1. 启动 `WuWaServerLauncher.exe`。
2. 选择 **官服 / B服 / 国际服**。
3. 第一次使用某个服务器时，点击 **选择 EXE**。
4. 选择该服务器实际使用的 `launcher.exe` 或 `Wuthering Waves.exe`。
5. 点击 **启动**。

程序会记住三个服务器各自的启动程序和上一次选择的服务器。

配置文件保存在：

```text
%APPDATA%\WuWaServerLauncher\settings.json
```

## 设计参考

本项目参考了 [WutheringWavesTool](https://github.com/leck995/WutheringWavesTool) 对鸣潮客户端来源的划分。

WutheringWavesTool 当前代码中将鸣潮客户端区分为：

| 服务器 | App ID |
| --- | ---: |
| 官服 | 10003 |
| B服 | 10004 |
| 国际服 | 50004 |

本项目只采用这一层“服务器来源”的概念，不复制 WutheringWavesTool 的资源下载、校验、缓存和文件切换实现。

## 当前版本的范围

这个版本**只负责选择并启动已经安装好的客户端**。

它不会：

- 下载游戏；
- 更新或修复游戏；
- 修改游戏资源文件；
- 在国服/B服之间自动交换渠道文件；
- 修改渠道 ID；
- 注入 DLL；
- 绕过反作弊；
- 保存账号、密码或登录 Token。

因此，如果电脑上分别安装了三个客户端，可以通过一个 Launcher 统一选择进入哪个服务器。

## Build

Requirements:

- Windows 10/11
- .NET 8 SDK

```powershell
dotnet restore src/WuWaServerLauncher.csproj
dotnet build src/WuWaServerLauncher.csproj -c Release
```

发布 Windows x64：

```powershell
dotnet publish src/WuWaServerLauncher.csproj `
  -c Release `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -o publish
```

GitHub Actions 会自动构建 Windows x64 self-contained 版本。

## Third-party notice

This is an unofficial community project and is not affiliated with or endorsed by Kuro Games, Wuthering Waves, or Bilibili.

## License

MIT
