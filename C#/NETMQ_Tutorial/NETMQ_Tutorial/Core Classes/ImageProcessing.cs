using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

public static class ImageProcessing
{
    /// <summary>
    /// Converts Bitmap Image to Byte array using given format
    /// </summary>
    /// <param name="img"></param>
    /// <returns>Byte array that contains image bytes</returns>
    public static byte[] ImageToByteArray(Bitmap img)
    {
        if (img == null)
            return null;
        try
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }
        catch
        {
            return null;
        }
    }
    /// <summary>
    /// Gets the screenshot of current window
    /// </summary>
    /// <returns></returns>
    public static Bitmap GetScreenShot()
    {
        System.Drawing.Rectangle bounds = Screen.AllScreens[0].Bounds;
        Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size, CopyPixelOperation.SourceCopy);
        }
        return bitmap;
    }
    /// <summary>
    /// Converts Given byte array that contains image bytes, to an image
    /// </summary>
    /// <param name="imageBytes"></param>
    /// <returns></returns>
    public static Bitmap ImageFromByteArray(byte[] imageBytes)
    {
        try
        {
            Bitmap bmp;
            using (var ms = new MemoryStream(imageBytes))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }
        catch( Exception e)
        {
            System.Diagnostics.Debug.WriteLine("Failed to creat image from given byte array: " + e.ToString());
            return null;
        }
    }

    /// <summary>
    /// Delete a GDI object
    /// </summary>
    /// <param name="o">The poniter to the GDI object to be deleted</param>
    /// <returns></returns>
    [DllImport("gdi32")]
    private static extern int DeleteObject(IntPtr o);

    /// <summary>
    /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
    /// </summary>
    /// <param name="image">The Emgu CV Image</param>
    /// <returns>The equivalent BitmapSource</returns>
    public static BitmapSource ToBitmapSource(Bitmap image)
    {
        IntPtr ptr = image.GetHbitmap(); //obtain the Hbitmap

        BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            ptr,
            IntPtr.Zero,
            System.Windows.Int32Rect.Empty,
            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

        DeleteObject(ptr); //release the HBitmap
        return bs;
    }
}