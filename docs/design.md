# Design

## What it is

A viewer plugin for QuickLook for Windows (`QL-Win/QuickLook`). It previews Lottie animations and renders them with ThorVG's CPU engine through ThorVG's C API. The plugin's code is MIT, and so is ThorVG; no GPL or AGPL code is linked into the plugin. QuickLook itself is GPL-3.0 and loads the plugin as a separate assembly.

## Which files it opens

- `*.lot` and `*.lottie.json`: Lottie JSON, always (ThorVG's own loader treats `lot` as Lottie).
- `*.lottie`: dotLottie, a zip. ThorVG 1.1.4 has no loader for the archive, so the plugin unpacks it into a temporary folder, takes the first animation named in `manifest.json` from `animations/<id>.json` (dotLottie 1) or `a/<id>.json` (dotLottie 2), or else the first JSON file, and loads it with that folder as the resource path, so its images resolve. The folder is deleted when the preview closes.
- `*.json`: only when the first 8 KB hold the four keys a Bodymovin document starts with (`"v"`, `"fr"`, `"op"`, `"layers"`), so other JSON files keep QuickLook's text viewer.

## How a preview is drawn

The plugin reads the file's bytes and gives them to `tvg_picture_load_data` with the mimetype `lottie+json` and the file's folder as the resource path, so image assets next to the file load and non-ASCII paths never pass through the C runtime's narrow file API. The picture is scaled so its longer side is at most 2048 pixels and drawn into a WPF `WriteableBitmap`. ThorVG's `ARGB8888` colorspace is premultiplied ARGB in a 32-bit word, which in little-endian memory is WPF's `Pbgra32`, so the canvas renders straight into the bitmap's back buffer with no conversion. A `DispatcherTimer` steps the frames at the animation's own rate: the duration divided by the number of frames.

## Building ThorVG

`native/build.sh` downloads ThorVG 1.1.4, checks its SHA-256, and cross-compiles `libthorvg-1.dll` on Linux with llvm-mingw (`LLVM_MINGW` names the toolchain folder) and meson: CPU engine, the `capi` binding, loaders for Lottie, SVG, PNG, JPEG and TrueType, Lottie expressions on. The C++ runtime is linked statically, so the DLL imports only `KERNEL32.dll` and the Windows UCRT (`api-ms-win-crt-*`).

OpenMP is off (`-Dextra=lottie_exp` drops ThorVG's default `openmp`). llvm-mingw ships OpenMP only as `libomp.dll` with its import library, not as a static `libomp.a`, so the static link cannot take it. A build with OpenMP would link libomp dynamically and ship `libomp.dll` (Apache-2.0 with the LLVM exception) next to the plugin. ThorVG still renders on its own task threads without it.

## Building and packaging the plugin

The plugin targets .NET Framework 4.8, as QuickLook's plugins do, and builds with the .NET SDK alone (no Visual Studio) through the `Microsoft.NETFramework.ReferenceAssemblies` package. It compiles against the installed `QuickLook.Common.dll` (`QuickLookDir` defaults to `%LOCALAPPDATA%\Programs\QuickLook`), which is not copied or committed.

`tools/package.sh <bin/Release/net48>` zips the plugin, its metadata, `libthorvg-1.dll` and both licence texts into `dist/QuickLook.Plugin.ThorVGViewer.qlplugin`. Open that file with QuickLook to install it, or unpack it into `%APPDATA%\pooi.moe\QuickLook\QuickLook.Plugin\QuickLook.Plugin.ThorVGViewer\` and restart QuickLook.

## Checked

On 2026-11-26 with QuickLook 4.5.0: a 256 x 256, 60-frame Lottie previewed with no exception logged, and two captures of the preview window 0.4 s apart showed the animation at different frames. The same animation as `.lot`, and as a dotLottie 1 archive whose animation draws an image from the archive's `images/` folder, previewed with the image in place.
