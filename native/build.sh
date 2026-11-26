#!/bin/bash
# docs/design.md
set -euo pipefail
ROOT=$(cd "$(dirname "$0")/.." && pwd)
LLVM_MINGW=${LLVM_MINGW:?set LLVM_MINGW to an llvm-mingw toolchain folder}
VERSION=1.1.4
SHA256=b448d88a005173e56d668d7baa08925b65569589226f73f2146775b0f796ce6c
mkdir -p "$ROOT/third_party" "$ROOT/build"
cd "$ROOT/third_party"
[ -f thorvg-$VERSION.tar.xz ] || curl -fsSL -o thorvg-$VERSION.tar.xz https://github.com/thorvg/thorvg/releases/download/v$VERSION/thorvg-$VERSION.tar.xz
echo "$SHA256  thorvg-$VERSION.tar.xz" | sha256sum -c -
rm -rf thorvg-$VERSION && tar xf thorvg-$VERSION.tar.xz
sed "s|@LLVM_MINGW@|$LLVM_MINGW|g" "$ROOT/native/llvm-mingw-x86_64.ini.in" > "$ROOT/build/llvm-mingw-x86_64.ini"
cd thorvg-$VERSION
rm -rf "$ROOT/build/thorvg-win64"
meson setup "$ROOT/build/thorvg-win64" --cross-file "$ROOT/build/llvm-mingw-x86_64.ini" --buildtype=release \
  -Ddefault_library=shared -Dengines=cpu -Dloaders=svg,png,jpg,lottie,ttf -Dbindings=capi \
  -Dthreads=true -Dsimd=true -Dlog=false -Dextra=lottie_exp
ninja -C "$ROOT/build/thorvg-win64"
