// SPDX-FileCopyrightText: 2026 K. S. Ernest (iFire) Lee
// SPDX-License-Identifier: MIT
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace QuickLook.Plugin.ThorVGViewer
{
    internal static class ThorVG
    {
        private const string Lib = "libthorvg-1.dll";
        public const int ColorspaceArgb8888 = 1;
        public const int EngineDefault = 1;

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string path);

        public static void LoadNextTo(string assemblyPath)
        {
            var dll = Path.Combine(Path.GetDirectoryName(assemblyPath) ?? ".", Lib);
            if (LoadLibrary(dll) == IntPtr.Zero)
                throw new DllNotFoundException(dll);
        }

        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_engine_init(uint threads);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr tvg_swcanvas_create(int option);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_swcanvas_set_target(IntPtr canvas, IntPtr buffer, uint stride, uint w, uint h, int colorspace);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_canvas_destroy(IntPtr canvas);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_canvas_add(IntPtr canvas, IntPtr paint);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_canvas_update(IntPtr canvas);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_canvas_draw(IntPtr canvas, [MarshalAs(UnmanagedType.I1)] bool clear);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_canvas_sync(IntPtr canvas);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr tvg_animation_new();
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_animation_del(IntPtr animation);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern IntPtr tvg_animation_get_picture(IntPtr animation);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_animation_set_frame(IntPtr animation, float frame);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_animation_get_total_frame(IntPtr animation, out float count);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_animation_get_duration(IntPtr animation, out float seconds);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_picture_load_data(IntPtr picture, byte[] data, uint size, string mimetype, string resourcePath, [MarshalAs(UnmanagedType.I1)] bool copy);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_picture_get_size(IntPtr picture, out float w, out float h);
        [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)] public static extern int tvg_picture_set_size(IntPtr picture, float w, float h);
    }
}
