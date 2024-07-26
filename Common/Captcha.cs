using System.Drawing.Text;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using G_APIs.BussinesLogic;
using G_APIs.BussinesLogic.Interface;

namespace G_APIs.Common;

public class Captcha
{
    public Captcha()
    {
    }
    public string Create(out string urlCaptcha)
    {

        var strCaptch = new Random().Next().ToString().Substring(0, 4);
        urlCaptcha = @"\Captcha\" + strCaptch + ".jpg";


        Bitmap bmpImage = new Bitmap(1, 1);

        int iWidth = 0;
        int iHeight = 0;

        Font MyFont = new Font("Arial", 36, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
        Graphics MyGraphics = Graphics.FromImage(bmpImage);
        iWidth = Convert.ToInt32(MyGraphics.MeasureString(strCaptch, MyFont).Width) + 20;
        iHeight = Convert.ToInt32(MyGraphics.MeasureString(strCaptch, MyFont).Height) + 4;
        bmpImage = new Bitmap(bmpImage, new Size(iWidth, iHeight));
        MyGraphics = Graphics.FromImage(bmpImage);
        MyGraphics.Clear(Color.Beige);
        MyGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
        MyGraphics.DrawString(strCaptch, MyFont, new SolidBrush(Color.Brown), 10, 4);
        MyGraphics.Flush();

        bmpImage.Save(Directory.GetCurrentDirectory() + @"\wwwroot\" + urlCaptcha, System.Drawing.Imaging.ImageFormat.Gif);
        bmpImage.Dispose();

        return strCaptch;

    }

}
