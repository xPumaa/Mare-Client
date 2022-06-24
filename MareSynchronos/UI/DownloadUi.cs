﻿using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using MareSynchronos.Utils;
using MareSynchronos.WebAPI;

namespace MareSynchronos.UI;

public class DownloadUi : Window, IDisposable
{
    private readonly WindowSystem _windowSystem;
    private readonly Configuration _pluginConfiguration;
    private readonly ApiController _apiController;

    public void Dispose()
    {
        Logger.Debug("Disposing " + nameof(DownloadUi));
        _windowSystem.RemoveWindow(this);
    }

    public DownloadUi(WindowSystem windowSystem, Configuration pluginConfiguration, ApiController apiController) : base("Mare Synchronos Downloads")
    {
        Logger.Debug("Creating " + nameof(DownloadUi));
        _windowSystem = windowSystem;
        _pluginConfiguration = pluginConfiguration;
        _apiController = apiController;

        SizeConstraints = new WindowSizeConstraints()
        {
            MaximumSize = new(300, 90),
            MinimumSize = new(300, 90)
        };

        Flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground;

        windowSystem.AddWindow(this);
        IsOpen = true;
    }

    public override void Draw()
    {
        if (!_pluginConfiguration.ShowTransferWindow) return;
        if (!_apiController.IsDownloading && !_apiController.IsUploading) return;

        var drawList = ImGui.GetWindowDrawList();
        var yDistance = 20;
        var xDistance = 20;

        var basePosition = ImGui.GetWindowPos() + ImGui.GetWindowContentRegionMin();

        if (_apiController.IsUploading)
        {
            var doneUploads = _apiController.CurrentUploads.Count(c => c.Value.Item1 == c.Value.Item2);
            var totalUploads = _apiController.CurrentUploads.Keys.Count;
            var totalUploaded = _apiController.CurrentUploads.Sum(c => c.Value.Item1);
            var totalToUpload = _apiController.CurrentUploads.Sum(c => c.Value.Item2);
            UiShared.DrawOutlinedFont(drawList, "▲",
                new Vector2(basePosition.X + 0, basePosition.Y + (int)(yDistance * 0.5)),
                UiShared.Color(255, 255, 255, 255), UiShared.Color(0, 0, 0, 255), 2);
            UiShared.DrawOutlinedFont(drawList, $"Uploading {doneUploads}/{totalUploads}",
                new Vector2(basePosition.X + xDistance, basePosition.Y + yDistance * 0),
                UiShared.Color(255, 255, 255, 255), UiShared.Color(0, 0, 0, 255), 2);
            UiShared.DrawOutlinedFont(drawList, $"{UiShared.ByteToString(totalUploaded)}/{UiShared.ByteToString(totalToUpload)}",
                new Vector2(basePosition.X + xDistance, basePosition.Y + yDistance * 1),
                UiShared.Color(255, 255, 255, 255), UiShared.Color(0, 0, 0, 255), 2);
        }

        if (_apiController.IsDownloading)
        {
            var multBase = _apiController.IsDownloading ? 0 : 2;
            var doneDownloads = _apiController.CurrentDownloads.Count(c => c.Value.Item1 == c.Value.Item2);
            var totalDownloads = _apiController.CurrentDownloads.Keys.Count;
            var totalDownloaded = _apiController.CurrentDownloads.Sum(c => c.Value.Item1);
            var totalToDownload = _apiController.CurrentDownloads.Sum(c => c.Value.Item2);
            UiShared.DrawOutlinedFont(drawList, "▼",
                new Vector2(basePosition.X + 0, basePosition.Y + (int)(yDistance * multBase + (yDistance * 0.5))),
                UiShared.Color(255, 255, 255, 255), UiShared.Color(0, 0, 0, 255), 2);
            UiShared.DrawOutlinedFont(drawList, $"Downloading {doneDownloads}/{totalDownloads}",
                new Vector2(basePosition.X + xDistance, basePosition.Y + yDistance * multBase),
                UiShared.Color(255, 255, 255, 255), UiShared.Color(0, 0, 0, 255), 2);
            UiShared.DrawOutlinedFont(drawList, $"{UiShared.ByteToString(totalDownloaded)}/{UiShared.ByteToString(totalToDownload)}",
                new Vector2(basePosition.X + xDistance, basePosition.Y + yDistance * (1 + multBase)),
                UiShared.Color(255, 255, 255, 255), UiShared.Color(0, 0, 0, 255), 2);
        }
    }
}