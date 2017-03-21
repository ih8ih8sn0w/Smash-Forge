﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using GraphicsMagick;
using System.IO;

namespace Smash_Forge
{
    class HGHT
    {
        public HGHT(string filename)
        {
            Read(new FileData(filename));
        }

        public HGHT(FileData f)
        {
            Read(f);
        }

        public HGHT(MagickImage image)
        {
            fromMagickImage(image);
        }

        public int width;
        public int height;
        public ushort[,] map;
        public Bitmap bitmap;
        public string name = "HeightMap";

        public void Read(FileData f)
        {
            width = (int)Math.Sqrt(f.eof() / 2);
            height = width;
            map = new ushort[width,height];
            for(int i = 0; i < width; i++)
                for(int j = 0; j < height; j++)
                    map[i, j] = (ushort)(f.readSignedShort() + 32767);
        }

        public void Write(string filename)
        {
            File.WriteAllBytes(filename, Rebuild());
        }

        public byte[] Rebuild()
        {
            FileOutput f = new FileOutput();
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    f.writeSignedShort((short)(map[i, j] - 32767));
            return f.getBytes();
        }

        public MagickImage toMagickImage()
        {
            MagickImage image = new MagickImage(new MagickColor(0,0,0), width, height);
            image.Format = MagickFormat.Png48;
            using (WritablePixelCollection pc = image.GetWritablePixels())
            {
                foreach(var pixel in pc)
                {
                    for (int channel = 0; channel < pc[pixel.X, pixel.Y].Channels; channel++)
                    {
                        pixel.SetChannel(channel, map[pixel.X, pixel.Y]);
                    }
                    pc.Set(pixel);
                }
                pc.Write();
            }
            return image;
        }

        public void fromMagickImage(MagickImage image)
        {
            MagickFormat format = image.Format;
            bool notFull16Bit = format != MagickFormat.Png48;
            height = image.Height;
            width = image.Width;
            map = new ushort[width, height];
            foreach(var pixel in image.GetWritablePixels())
            {
                map[pixel.X, pixel.Y] = pixel.GetChannel(0);
                if (notFull16Bit)//if 8 bit per channel remap to 16 bit space (hopefully this never has to be used... not high enough precision)
                    map[pixel.X, pixel.Y] = (ushort)(map[pixel.X, pixel.Y] * (32678f / 256));
            }
        }

        public void generateBitmap()
        {
            Bitmap b = new Bitmap(width, height);
            //Convert to bitmap while remapping 16 bit per channel to 8 bit space so it can be displayed
            for (int i = 0; i < width; i++)
                for(int j = 0; j < height; j++)
                    b.SetPixel(i, j, Color.FromArgb(255, (byte)((map[i, j] / 32678f) * 256), (byte)((map[i, j] / 32678f) * 256), (byte)((map[i, j] / 32678f) * 256)));
            bitmap = b;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
