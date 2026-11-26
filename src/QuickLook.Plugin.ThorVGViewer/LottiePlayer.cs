// SPDX-FileCopyrightText: 2026 K. S. Ernest (iFire) Lee
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace QuickLook.Plugin.ThorVGViewer
{
    internal sealed class LottiePlayer : Image, IDisposable
    {
        private IntPtr _animation, _canvas;
        private WriteableBitmap _bitmap;
        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Render);
        private float _frames, _frame;
        public Size NativeSize { get; private set; }

        public LottiePlayer(string path, int maxSide)
        {
            var data = File.ReadAllBytes(path);
            _animation = ThorVG.tvg_animation_new();
            var picture = ThorVG.tvg_animation_get_picture(_animation);
            var dir = Path.GetDirectoryName(Path.GetFullPath(path)) + Path.DirectorySeparatorChar;
            if (ThorVG.tvg_picture_load_data(picture, data, (uint)data.Length, "lottie+json", dir, true) != 0)
                throw new InvalidDataException("ThorVG could not read this file as a Lottie animation.");
            ThorVG.tvg_picture_get_size(picture, out var w, out var h);
            NativeSize = new Size(w, h);
            var scale = Math.Min(1.0, maxSide / Math.Max(w, h));
            int pw = Math.Max(1, (int)Math.Round(w * scale)), ph = Math.Max(1, (int)Math.Round(h * scale));
            ThorVG.tvg_picture_set_size(picture, pw, ph);
            _bitmap = new WriteableBitmap(pw, ph, 96, 96, PixelFormats.Pbgra32, null);
            _canvas = ThorVG.tvg_swcanvas_create(ThorVG.EngineDefault);
            ThorVG.tvg_swcanvas_set_target(_canvas, _bitmap.BackBuffer, (uint)(_bitmap.BackBufferStride / 4), (uint)pw, (uint)ph, ThorVG.ColorspaceArgb8888);
            ThorVG.tvg_canvas_add(_canvas, picture);
            ThorVG.tvg_animation_get_total_frame(_animation, out _frames);
            ThorVG.tvg_animation_get_duration(_animation, out var seconds);
            Source = _bitmap;
            Stretch = Stretch.Uniform;
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
            Draw();
            if (_frames > 1 && seconds > 0)
            {
                _timer.Interval = TimeSpan.FromSeconds(seconds / _frames);
                _timer.Tick += (s, e) => { _frame = (_frame + 1) % _frames; Draw(); };
                _timer.Start();
            }
        }

        private void Draw()
        {
            ThorVG.tvg_animation_set_frame(_animation, _frame);
            _bitmap.Lock();
            ThorVG.tvg_canvas_update(_canvas);
            ThorVG.tvg_canvas_draw(_canvas, true);
            ThorVG.tvg_canvas_sync(_canvas);
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
            _bitmap.Unlock();
        }

        public void Dispose()
        {
            _timer.Stop();
            if (_canvas != IntPtr.Zero) { ThorVG.tvg_canvas_destroy(_canvas); _canvas = IntPtr.Zero; }
            if (_animation != IntPtr.Zero) { ThorVG.tvg_animation_del(_animation); _animation = IntPtr.Zero; }
        }
    }
}
