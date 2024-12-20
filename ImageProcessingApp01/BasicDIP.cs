﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing.Drawing2D;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Diagnostics;



public class BasicDIP
{
    // TEMPLATE FUNCTION
    public void Run(Bitmap origanalImage, Bitmap newImage, Func<Bitmap, int, int, Color> pixelFunc, int originX, int originY, int endX, int endY)
    {
        for (int x = originX; x < endX; x++)
        {
            for (int y = originY; y < endY; y++)
            {
                newImage.SetPixel(x, y, pixelFunc(origanalImage, x, y));
            }
        }
    }
    public void Run(Bitmap origanalImage, Bitmap newImage, Func<Bitmap, int, int, Color> pixelFunc)
    {
        Run(origanalImage, newImage, pixelFunc, 0, 0, origanalImage.Width, origanalImage.Height);
    }


    // BASIC COPY
    public void basicCopy(Bitmap image, Bitmap newImage, int originX, int originY, int endX, int endY)
    {
        Run(
            image, newImage,
            (bitmap, x, y) => image.GetPixel(x, y),
            originX, originY, endX, endY
            );
    }
    public void basicCopy(Bitmap image, Bitmap newImage)
    {
        basicCopy(image, newImage, 0, 0, image.Width, image.Height);
    }
    public Bitmap basicCopy(Bitmap image)
    {
        Bitmap newImage = new Bitmap(image.Width, image.Height);
        basicCopy(image, newImage, 0, 0, image.Width, image.Height);
        return newImage;
    }


    // GRAYSCALE
    public void grayscale(Bitmap image, Bitmap newImage, int originX, int originY, int endX, int endY)
    {
        Run(
            image, newImage,
            (bitmap, x, y) => {
                Color pixel = image.GetPixel(x, y);
                int ave = (int)(pixel.R + pixel.G + pixel.B) / 3;
                return Color.FromArgb(ave, ave, ave);
            },
            originX, originY, endX, endY
            );
    }
    public void grayscale(Bitmap image, Bitmap newImage)
    {
        grayscale(image, newImage, 0, 0, image.Width, image.Height);
    }
    public Bitmap grayscale(Bitmap image)
    {
        Bitmap newImage = new Bitmap(image.Width, image.Height);
        grayscale(image, newImage, 0, 0, image.Width, image.Height);
        return newImage;
    }


    // INVERSION
    public void invert(Bitmap image, Bitmap newImage, int originX, int originY, int endX, int endY)
    {
        Run(
            image, newImage,
            (bitmap, x, y) => {
                Color pixel = image.GetPixel(x, y);
                return Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
            },
            originX, originY, endX, endY
            );
    }
    public void invert(Bitmap image, Bitmap newImage)
    {
        invert(image, newImage, 0, 0, image.Width, image.Height);
    }
    public Bitmap invert(Bitmap image)
    {
        Bitmap newImage = new Bitmap(image.Width, image.Height);
        invert(image, newImage, 0, 0, image.Width, image.Height);
        return newImage;
    }


    // MIRROR X
    public void mirrorX(Bitmap image, Bitmap newImage, int originX, int originY, int endX, int endY)
    {
        Run(
            image, newImage,
            (bitmap, x, y) => image.GetPixel(image.Width - x - 1 + originX, y),
            originX, originY, endX, endY
            );
    }
    public void mirrorX(Bitmap image, Bitmap newImage)
    {
        mirrorX(image, newImage, 0, 0, image.Width, image.Height);
    }
    public Bitmap mirrorX(Bitmap image)
    {
        Bitmap newImage = new Bitmap(image.Width, image.Height);
        mirrorX(image, newImage, 0, 0, image.Width, image.Height);
        return newImage;
    }


    // MIRROR Y
    public void mirrorY(Bitmap image, Bitmap newImage, int originX, int originY, int endX, int endY)
    {
        Run(
            image, newImage,
            (bitmap, x, y) => image.GetPixel(x, image.Height - y - 1 + originY),
            originX, originY, endX, endY
            );
    }
    public void mirrorY(Bitmap image, Bitmap newImage)
    {
        mirrorY(image, newImage, 0, 0, image.Width, image.Height);
    }
    public Bitmap mirrorY(Bitmap image)
    {
        Bitmap newImage = new Bitmap(image.Width, image.Height);
        mirrorY(image, newImage, 0, 0, image.Width, image.Height);
        return newImage;
    }


    // SEPIA
    public void sepia(Bitmap image, Bitmap newImage, int originX, int originY, int endX, int endY)
    {
        Run(
            image, newImage,
            (bitmap, x, y) => {
                Color pixel = image.GetPixel(x, y);

                // apply the sepia formula
                int tr = (int)(0.393 * pixel.R + 0.769 * pixel.G + 0.189 * pixel.B);
                int tg = (int)(0.349 * pixel.R + 0.686 * pixel.G + 0.168 * pixel.B);
                int tb = (int)(0.272 * pixel.R + 0.534 * pixel.G + 0.131 * pixel.B);

                // ensure RGB values are within the valid range (0-255)
                tr = Math.Min(255, tr);
                tg = Math.Min(255, tg);
                tb = Math.Min(255, tb);

                return Color.FromArgb(tr, tg, tb);
            },
            originX, originY, endX, endY
            );
    }
    public void sepia(Bitmap image, Bitmap newImage)
    {
        sepia(image, newImage, 0, 0, image.Width, image.Height);
    }
    public Bitmap sepia(Bitmap image)
    {
        Bitmap newImage = new Bitmap(image.Width, image.Height);
        sepia(image, newImage, 0, 0, image.Width, image.Height);
        return newImage;
    }



    // HISTOGRAM
    public Bitmap histogram(Bitmap bitmap)
    {
        Bitmap processed = new Bitmap(256, 800);

        int[] hist = getPixelFrequency(bitmap);
        int max = hist.Max() / processed.Height;

        for (int i = 0; i < hist.Length; i++)
        {
            for (int j = 0; j < Math.Min(processed.Height, hist[i] / max); j++)
            {
                processed.SetPixel(i, processed.Height - j - 1, Color.Black);
            }
        }

        return processed;
    }
    private int[] getPixelFrequency(Bitmap bitmap)
    {
        int[] hist = new int[256];

        for (int i = 0; i < bitmap.Width; i++)
        {
            for (int j = 0; j < bitmap.Height; j++)
            {
                Color pixel = bitmap.GetPixel(i, j);
                int avg = (pixel.R + pixel.G + pixel.B) / 3;
                hist[avg]++;
            }
        }

        return hist;
    }


    // SUBTRACT
    public Bitmap subtract(Bitmap foreground, Bitmap background, Color foregroundColor)
    {
        // check if any image is null
        if (foreground == null || background == null) 
            return null;

        Bitmap mergedImage = ResizeImage(foreground, background);

        int aveForeColor = (foregroundColor.R + foregroundColor.G + foregroundColor.B) / 3;
        int threshold = 10;

        Bitmap subtracted = new Bitmap(mergedImage.Width, mergedImage.Height);

        for (int x = 0; x < mergedImage.Width; x++)
        {
            for (int y = 0; y < mergedImage.Height; y++)
            {
                Color front = mergedImage.GetPixel(x, y);
                Color back = background.GetPixel(x, y);
                int aveFront = (front.R + front.G + front.B) / 3;

                if (Math.Abs(aveFront - aveForeColor) <= threshold)
                    subtracted.SetPixel(x, y, back);
                else
                    subtracted.SetPixel(x, y, front);
            }
        }

        return subtracted;
    }

    private Bitmap ResizeImage(Bitmap a, Bitmap b)
    {
        Bitmap resizedImage = new Bitmap(b.Width, b.Height);
        using (Graphics g = Graphics.FromImage(resizedImage))
        {
            g.DrawImage(a, 0, 0, b.Width, b.Height);
        }
        return resizedImage;
    }
}