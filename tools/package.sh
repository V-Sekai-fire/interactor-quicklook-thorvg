#!/bin/bash
# docs/design.md
set -euo pipefail
ROOT=$(cd "$(dirname "$0")/.." && pwd)
PLUGIN=${1:?path to the built plugin folder (bin/Release/net48)}
OUT="$ROOT/dist/QuickLook.Plugin.ThorVGViewer.qlplugin"
STAGE=$(mktemp -d)
cp "$PLUGIN/QuickLook.Plugin.ThorVGViewer.dll" "$PLUGIN/QuickLook.Plugin.Metadata.config" "$STAGE/"
cp "$ROOT/build/thorvg-win64/src/libthorvg-1.dll" "$STAGE/"
cp "$ROOT/LICENSE" "$STAGE/LICENSE.txt"
cp "$ROOT/docs/ThorVG-LICENSE.txt" "$STAGE/ThorVG-LICENSE.txt"
mkdir -p "$ROOT/dist"; rm -f "$OUT"
ELIXIR_ERL_OPTIONS=+fnu elixir -e '[out, stage] = System.argv(); files = stage |> File.ls!() |> Enum.sort() |> Enum.map(&String.to_charlist/1); {:ok, _} = :zip.create(String.to_charlist(out), files, cwd: String.to_charlist(stage))' "$OUT" "$STAGE"
rm -rf "$STAGE"
echo "$OUT"
