// SPDX-FileCopyrightText: 2026 K. S. Ernest (iFire) Lee
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Text;
using System.Windows;
using QuickLook.Common.Plugin;

namespace QuickLook.Plugin.ThorVGViewer
{
    public class Plugin : IViewer
    {
        private LottiePlayer _player;
        private DotLottie _archive;
        public int Priority => 10;

        public void Init()
        {
            ThorVG.LoadNextTo(typeof(Plugin).Assembly.Location);
            ThorVG.tvg_engine_init((uint)Math.Max(0, Environment.ProcessorCount - 1));
        }

        public bool CanHandle(string path)
        {
            if (Directory.Exists(path)) return false;
            if (path.EndsWith(".lottie.json", StringComparison.OrdinalIgnoreCase)) return true;
            if (path.EndsWith(".lot", StringComparison.OrdinalIgnoreCase)) return true;
            if (path.EndsWith(".lottie", StringComparison.OrdinalIgnoreCase)) return true;
            return path.EndsWith(".json", StringComparison.OrdinalIgnoreCase) && LooksLikeLottie(path);
        }

        // docs/design.md: a Bodymovin document starts with its version, frame rate, frame range and layers.
        private static bool LooksLikeLottie(string path)
        {
            try
            {
                var buffer = new byte[8192];
                int n;
                using (var f = File.OpenRead(path)) n = f.Read(buffer, 0, buffer.Length);
                var head = Encoding.UTF8.GetString(buffer, 0, n);
                return head.Contains("\"v\"") && head.Contains("\"fr\"") && head.Contains("\"op\"") && head.Contains("\"layers\"");
            }
            catch (IOException) { return false; }
        }

        public void Prepare(string path, ContextObject context)
        {
            if (path.EndsWith(".lottie", StringComparison.OrdinalIgnoreCase))
            {
                _archive = new DotLottie(path);
                _player = new LottiePlayer(_archive.Animation, _archive.Folder, 2048);
            }
            else
            {
                _player = new LottiePlayer(File.ReadAllBytes(path), Path.GetDirectoryName(Path.GetFullPath(path)), 2048);
            }
            context.SetPreferredSizeFit(_player.NativeSize, 0.8);
        }

        public void View(string path, ContextObject context)
        {
            context.Title = Path.GetFileName(path);
            context.ViewerContent = _player;
            context.IsBusy = false;
        }

        public void Cleanup()
        {
            _player?.Dispose();
            _player = null;
            _archive?.Dispose();
            _archive = null;
        }
    }
}
