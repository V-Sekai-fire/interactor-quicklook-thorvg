# interactor-quicklook-thorvg

Preview Lottie animations in [QuickLook for Windows](https://github.com/QL-Win/QuickLook), rendered by [ThorVG](https://github.com/thorvg/thorvg). Select a `.lot`, `.lottie` (dotLottie), `.lottie.json` file, or a `.json` file that holds a Lottie animation, and press Space.

## Install

Build the package (below), then open `QuickLook.Plugin.ThorVGViewer.qlplugin` with QuickLook, or unpack it into `%APPDATA%\pooi.moe\QuickLook\QuickLook.Plugin\QuickLook.Plugin.ThorVGViewer\` and restart QuickLook.

## Build

    LLVM_MINGW=/path/to/llvm-mingw native/build.sh          # libthorvg-1.dll, on Linux
    dotnet build -c Release src/QuickLook.Plugin.ThorVGViewer   # on Windows, with QuickLook installed
    tools/package.sh src/QuickLook.Plugin.ThorVGViewer/bin/Release/net48

How it works, which files it opens and why OpenMP is off: [docs/design.md](docs/design.md).

## Licence

MIT, as is ThorVG (`LICENSES/ThorVG-MIT.txt`).
