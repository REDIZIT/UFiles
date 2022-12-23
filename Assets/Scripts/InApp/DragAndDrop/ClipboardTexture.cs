using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;

namespace Stampcrawler
{
    public class ClipboardTexture
    {
        private Texture2D _texture;
        public Texture2D texture => _texture;

        public ClipboardTexture()
        {
        }

        public Texture2D GetClipboardTexture()
        {
            Texture2D tex = null;
            if (Clipboard.GetDataObject() != null)
            {
                Debug.Log("Not null");
                IDataObject data = Clipboard.GetDataObject();

                if (data.GetDataPresent(DataFormats.Bitmap))
                {
                    var image = data.GetData(DataFormats.Dib, false) as Bitmap;
                    Int32 stride;
                    var bm32bData = GetImageByteData(image, out stride);
                    var signature = 0;
                    for (int j = stride - image.Width; j < stride; j++)
                    {
                        signature += bm32bData[j];
                    }

                    if (signature == 0)
                    {
                        tex = GetFromPhotoEditor(image);
                    }
                    else
                    {
                        tex = ShiftTexture(RetrieveFromClipboard(), 3);
                    }
                }
            }
            Debug.Log("null");
            return tex;
        }

        private Texture2D GetFromPhotoEditor(Bitmap image)
        {
            image.RotateFlip(RotateFlipType.Rotate180FlipX);
            Int32 stride;
            var data = GetImageByteData(image, out stride);
            Byte[] newImageData = new Byte[data.Length];

            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    var pixelColumn = i * stride + (j * 4);
                    int dataPixelColumn = i * stride + (j * 3);

                    newImageData[pixelColumn] = data[dataPixelColumn + 2];
                    newImageData[pixelColumn + 1] = data[dataPixelColumn + 1];
                    newImageData[pixelColumn + 2] = data[dataPixelColumn];
                    newImageData[pixelColumn + 3] = 255;
                }
            }

            Texture2D tex = new Texture2D(image.Width, image.Height, TextureFormat.RGBA32, false);
            tex.LoadRawTextureData(newImageData);
            tex.Apply();
            _texture = tex;
            return tex;
        }

        private Texture2D ShiftTexture(Texture2D baseTexture, int shiftPixels)
        {
            Texture2D tex = new Texture2D(baseTexture.width, baseTexture.height);
            for (int i = 0; i < baseTexture.height; i++)
            {
                for (int j = shiftPixels; j < baseTexture.width; j++)
                {
                    tex.SetPixel(j - shiftPixels, i, baseTexture.GetPixel(j, i));
                }

                for (int k = 0; k < shiftPixels; k++)
                {
                    tex.SetPixel(k + baseTexture.width - shiftPixels, i, baseTexture.GetPixel(k, i));
                }
            }

            tex.Apply();
            _texture = tex;
            return tex;
        }

        private Texture2D RetrieveFromClipboard()
        {
            //System.Drawing.Image image = System.Windows.Forms.Clipboard.GetImage();
            //System.IO.MemoryStream s = new System.IO.MemoryStream(image.Width * image.Height);
            //image.Save(s, System.Drawing.Imaging.ImageFormat.Jpeg);

            //Texture2D tex = new Texture2D(image.Width, image.Height);
            //tex.LoadImage(s.ToArray());
            //tex.Apply();

            //s.Close();
            //s.Dispose();

            //return tex;
            return null;
        }

        private Byte[] GetImageByteData(Bitmap sourceImage, out Int32 stride)
        {
            BitmapData sourceData = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, sourceImage.PixelFormat);
            stride = sourceData.Stride;
            Byte[] data = new Byte[stride * sourceImage.Height];
            Marshal.Copy(sourceData.Scan0, data, 0, data.Length);
            sourceImage.UnlockBits(sourceData);

            return data;
        }
    }
}