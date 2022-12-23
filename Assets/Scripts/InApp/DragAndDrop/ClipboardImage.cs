using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class ClipboardImage
{
    [DllImport("UnityClipboard")] private static extern bool Open();
    [DllImport("UnityClipboard")] private static extern int Width();
    [DllImport("UnityClipboard")] private static extern int Height();
    [DllImport("UnityClipboard")] private static extern int BitsPerPixel();
    [DllImport("UnityClipboard")] private static extern IntPtr Read();

    public static Texture2D Copy()
    {
        try
        {
            // no image in clipboard
            if (Open() == false)
            {
                Debug.Log("Clipboard null");
                return null;
            }

            var width = Width();
            var height = Height();
            var bitsPerPixel = BitsPerPixel();
            var bytesPerPixel = bitsPerPixel / 8;
            var ptr = Read();
            var tex = new Texture2D(width, height, GraphicsFormat.R8G8B8A8_SRGB, 0, TextureCreationFlags.None);
            tex.wrapMode = TextureWrapMode.Clamp;

            var bytes = new byte[width * height * bytesPerPixel];

            if (bytesPerPixel == 3)
            {
                // When we have 3 bytes per pixel, it indicates an image formatted differently.
                // The format is BGR, and it additionally skips some bytes at the end of each row.
                // The number of skipped bytes is a function of the width.
                int skippedBytesPerRow = (width % 4);
                if (skippedBytesPerRow == 0)
                    skippedBytesPerRow = 4;

                int p = 0;
                bool skipBytes = false;
                int bytesPerRow = width * bytesPerPixel;
                for (int b = 0; b < bytes.Length; b++)
                {
                    if (skipBytes)
                    {
                        p += skippedBytesPerRow;
                    }
                    bytes[b] = Marshal.ReadByte(ptr, p++);
                    skipBytes = (b + 1) % bytesPerRow == 0;
                }
            }
            else
            {
                Marshal.Copy(ptr, bytes, 0, width * height * bytesPerPixel);
            }

            Color32[] colors = new Color32[width * height];

            int c = 0;
            for (int y = height - 1; y >= 0; y--)
            {
                for (var x = 0; x < width; x++)
                {
                    var pos = (y * width + x) * bytesPerPixel;

                    // read in bgra format
                    var b = bytes[pos];
                    var g = bytes[pos + 1];
                    var r = bytes[pos + 2];
                    var a = bytesPerPixel == 4 ? bytes[pos + 3] : (byte)255;

                    colors[c] = new Color32(r, g, b, a);
                    c++;
                }
            }

            tex.SetPixels32(colors);
            tex.Apply();
            return tex;
        }
        catch (Exception e)
        {
            Debug.LogError($"Can't past image from clipboard {e}");
        }

        return null;
    }
}