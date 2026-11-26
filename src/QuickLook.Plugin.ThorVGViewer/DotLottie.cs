// SPDX-FileCopyrightText: 2026 K. S. Ernest (iFire) Lee
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuickLook.Plugin.ThorVGViewer
{
    // docs/design.md: a .lottie file is a zip with manifest.json and the animation JSON beside its images.
    internal sealed class DotLottie : IDisposable
    {
        public string Folder { get; }
        public byte[] Animation { get; }

        public DotLottie(string path)
        {
            Folder = Path.Combine(Path.GetTempPath(), "QuickLook.ThorVG." + Guid.NewGuid().ToString("N"));
            ZipFile.ExtractToDirectory(path, Folder);
            Animation = File.ReadAllBytes(FindAnimation());
        }

        private string FindAnimation()
        {
            var manifest = Path.Combine(Folder, "manifest.json");
            if (File.Exists(manifest))
            {
                var id = Regex.Match(File.ReadAllText(manifest), @"""animations""\s*:\s*\[\s*\{[^}]*?""id""\s*:\s*""([^""]+)""");
                if (id.Success)
                    foreach (var dir in new[] { "animations", "a" })
                    {
                        var f = Path.Combine(Folder, dir, id.Groups[1].Value + ".json");
                        if (File.Exists(f)) return f;
                    }
            }
            var any = Directory.GetFiles(Folder, "*.json", SearchOption.AllDirectories)
                .Where(f => !string.Equals(Path.GetFileName(f), "manifest.json", StringComparison.OrdinalIgnoreCase))
                .OrderBy(f => f, StringComparer.Ordinal).FirstOrDefault();
            return any ?? throw new InvalidDataException("This .lottie archive holds no animation.");
        }

        public void Dispose()
        {
            try { Directory.Delete(Folder, true); } catch (IOException) { } catch (UnauthorizedAccessException) { }
        }
    }
}
