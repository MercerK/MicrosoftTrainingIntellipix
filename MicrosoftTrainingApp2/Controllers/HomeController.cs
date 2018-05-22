using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using System.IO;

namespace MicrosoftTrainingApp2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var path = Directory.CreateDirectory(Server.MapPath("~/Thumbnails"));
            ViewBag.Thumbnails = Directory.EnumerateFiles(path.FullName).Select(Path.GetFileName);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                // Make sure the user selected an image file
                if (!file.ContentType.StartsWith("image"))
                {
                    TempData["Message"] = "Only image files may be uploaded";
                }
                else
                {
                    try
                    {
                        var filename = Path.GetFileName(file.FileName);

                        // Save the original image in the "Photos" folder
                        var path = Directory.CreateDirectory(Server.MapPath("~/Photos"));

                        using (var photo = System.IO.File.Create(Path.Combine(path.FullName, filename)))
                        {
                            file.InputStream.Seek(0L, SeekOrigin.Begin);
                            file.InputStream.CopyTo(photo);
                        }

                        // Generate a thumbnail and save it in the "Thumbnails" folder
                        path = Directory.CreateDirectory(Server.MapPath("~/Thumbnails"));

                        using (var thumbnail = System.IO.File.Create(Path.Combine(path.FullName, filename)))
                        {
                            using (var stream = new MemoryStream())
                            {
                                file.InputStream.Seek(0L, SeekOrigin.Begin);
                                var settings = new ResizeSettings { MaxWidth = 192 };
                                ImageBuilder.Current.Build(file.InputStream, stream, settings);
                                stream.Seek(0L, SeekOrigin.Begin);
                                stream.CopyTo(thumbnail);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // In case something goes wrong
                        TempData["Message"] = ex.Message;
                    }
                }
            }

            return RedirectToAction("Index");
        }
    }
}