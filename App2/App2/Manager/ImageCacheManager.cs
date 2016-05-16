using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;

using Android.Graphics;
using Android.Util;
using ImageColorServices;
using Model;
using System.Xml.Linq;

namespace App2.Manager
{
    public class ImageCacheManager
    {
        LruCache imgCache;
        /// <summary>
        /// Gets the i image queue count.
        /// </summary>
        /// <value>The i image queue count.</value>
        public int imgQueueCount { get { return 3; } }
        private static ImageCacheManager instance;
        ImageColorService colorService;

        /// <summary>
        /// Initializes a new instance of the ImageCacheManager class.
        /// </summary>
        public ImageCacheManager()
        {
            imgCache = new LruCache(imgQueueCount);
            colorService = ImageColorService.Instance;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ImageCacheManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImageCacheManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Gets the image from cache.
        /// </summary>
        /// <returns>The image from cache.</returns>
        /// <param name="shape">Shape.</param>
        public async Task<Bitmap> GetImageFromCache(Square shape)
        {
            try
            {
                var image = imgCache.Get(shape.ShapeId.ToString()) as Bitmap;
                if (image == null && shape.ImagePath != "Icon.png")
                {
                    image = await GetImageFromUrl(shape.ImagePath);
                }
                else
                    image = (Bitmap)Resource.Drawable.Icon;
                return image;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }


        /// <summary>
        /// Puts the image to cache.
        /// </summary>
        /// <returns>The image to cache.</returns>
        /// <param name="squareShape">Square shape.</param>
        public async Task<Square> SetImageToCache(Square squareShape)
        {
            Square shape = null;
            try
            {
                if (string.IsNullOrEmpty(squareShape.ImagePath))
                {
                    shape = await colorService.GetSquare(squareShape) as Square;
                }
                if (shape != null)
                {

                    if (shape.ImagePath != "Icon.png")
                    {
                        var image = await GetImageFromUrl(shape.ImagePath);
                        if (image != null)
                            imgCache.Put(shape.ShapeId.ToString(), image);
                        return shape;
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Length > 0)
                {
                    var doc = XDocument.Parse(ex.Message);
                    squareShape = doc.Root.Descendants("pattern")
                        .Select(x => new Square()
                        {
                            ShapeId = squareShape.ShapeId,
                            ImagePath = x.Element("imageUrl").Value,
                            X = squareShape.X,
                            Y = squareShape.Y,
                        }) as Square;
                    
                }
                else
                {
                    string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
                    strPath = System.IO.Path.Combine(strPath, "Icon.png");
                    squareShape.ImagePath = strPath;
                    squareShape.GenerateFillColor();
                }
            }

            return squareShape;
        }


        /// <summary>
        /// Gets the image bitmap from URL.
        /// </summary>
        /// <returns>The image bitmap from URL.</returns>
        /// <param name="url">URL.</param>
        private async Task<Bitmap> GetImageFromUrl(string strUrl)
        {
            Bitmap image = null;
            using (var webClient = new WebClient())
            {
                try
                {
                    var imageByte = await webClient.DownloadDataTaskAsync(strUrl);
                    if (imageByte != null && imageByte.Length > 0)
                    {
                        image = BitmapFactory.DecodeByteArray(imageByte, 0, imageByte.Length);
                    }

                    else
                    {
                        image = (Bitmap)Resource.Drawable.Icon;
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("Exception in GetImageFromUrl() : {0}", ex.Message));
                }
            }

            return image;
        }
    }
}