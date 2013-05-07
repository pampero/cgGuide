using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using Framework.Helpers;

namespace WebUI.Controllers
{
    // http://www.webforefront.com/performance/webservers_statictier.html
    // http://dotnetslackers.com/articles/aspnet/ASP-NET-MVC-3-Controller-for-Serving-Images.aspx#1938
    // http://imageresizing.net/docs/best-practices
    public class ImagesController : Controller
    {
       
        public ActionResult Render(string file)
        {
            var fullFilePath = this.getFullFilePath(file);

            if (!ImageFileAvailable(fullFilePath))
            {
                return this.instantiate404ErrorResult(file);
            }

            return new ImageFileResult(fullFilePath);
        }

        public ActionResult RenderWithResize(int width, int height, string file)
        {
            var fullFilePath = this.getFullFilePath(file);

            if (!ImageFileAvailable(fullFilePath))
            {
                return this.instantiate404ErrorResult(file);
            }

            var resizeSettings = this.instantiateResizeSettings(width, height);
            var resizedImage = ImageBuilder.Current.Build(fullFilePath, resizeSettings);
            return new DynamicImageResult(file, resizedImage.ToByteArray());
        }

        public ActionResult RenderWithResizeAndWatermark(int width, int height, string file)
        {
            var fullFilePath = this.getFullFilePath(file);

            if (!ImageFileAvailable(fullFilePath))
            {
                return this.instantiate404ErrorResult(file);
            }

            var resizeSettings = this.instantiateResizeSettings(width, height);
            var resizedImage = ImageBuilder.Current.Build(fullFilePath, resizeSettings);
            var watermarkFullFilePath = this.getFullFilePath("Watermark.png");

            resizedImage = this.addWatermark(resizedImage,
                watermarkFullFilePath, new Point(0, 0));

            return new DynamicImageResult(file, resizedImage.ToByteArray());
        }

        private ResizeSettings instantiateResizeSettings(int width, int height)
        {
            var queryString = string.Format(
                "maxwidth={0}&maxheight={1}&quality=90",
                width, height);
            return new ResizeSettings(queryString);
        }

        private Bitmap addWatermark(Bitmap image, string watermarkFullFilePath, Point watermarkLocation)
        {
            using (var watermark = Image.FromFile(watermarkFullFilePath))
            {
                var watermarkToUse = watermark;
                if (watermark.Width > image.Width || watermark.Height > image.Height)
                {
                    var resizeSettings = this.instantiateResizeSettings(image.Width,
                        image.Height);
                    watermarkToUse = ImageBuilder.Current.Build(watermarkFullFilePath,
                        resizeSettings);
                }
                using (var graphics = Graphics.FromImage(image))
                {
                    graphics.DrawImage(watermarkToUse, watermarkLocation);
                }
            }
            return image;
        }

        private string getFullFilePath(string file)
        {
            return string.Format(@"{0}\{1}", Server.MapPath("~/Content/Images"), file);
        }


        private bool ImageFileAvailable(string fullFilePath)
        {
            return System.IO.File.Exists(fullFilePath);
        }


        private HttpNotFoundResult instantiate404ErrorResult(string file)
        {
            return new HttpNotFoundResult(
                string.Format("The file {0} does not exist.", file));
        }
    }
}

