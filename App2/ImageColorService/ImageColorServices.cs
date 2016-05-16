using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model;

namespace ImageColorServices
{
    public class ImageColorService
    {
        
        private const string URI = "http://www.colourlovers.com/api/";
        private static ImageColorService instance;
        public static ImageColorService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImageColorService();
                }
                return instance;
            }
        }

        /// <summary>
        /// Gets the shape Circle/Square based on the input.
        /// </summary>
        /// <returns>The shape infor async.</returns>
        /// <param name="input">Input.</param>
        public async Task<Shape> GetShape(Shape input)
        {
            try
            {
                if (input is Square)
                    return await GetSquare(input as Square);
                if (input is Circle)
                    return await GetCircle(input as Circle);
            }
            catch
            {
                throw;
            }
            return input;
        }

        /// <summary>
        /// Gets the square shapes from the specified URI.
        /// </summary>
        /// <returns>The square.</returns>
        /// <param name="input">Input.</param>
        public async Task<Square> GetSquare(Square input)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(URI + "patterns/random"));
                request.ContentType = "application/json";
                request.Method = "GET";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (StreamReader strReader = new StreamReader(response.GetResponseStream()))
                    {

                        var data = strReader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(data))
                        {
                            return input;
                        }
                        else
                        {
                            var doc = XDocument.Parse(data);
                            return doc.Root.Descendants("pattern")
                                .Select(x => new Square()
                                {
                                    ShapeId = input.ShapeId,
                                    ImagePath = x.Element("imageUrl").Value,
                                    X = input.X,
                                    Y = input.Y
                                }).FirstOrDefault() ?? input;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string strPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
                strPath = Path.Combine(strPath, "Icon.png");
                input.ImagePath = strPath;
                input.GenerateFillColor();
            }

            return input;
        }

        /// <summary>
        /// Gets the circle.
        /// </summary>
        /// <returns>The circle.</returns>
        /// <param name="input">Input.</param>
        public async Task<Circle> GetCircle(Circle input)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(URI + "colors/random"));
                request.ContentType = "application/json";
                request.Method = "GET";

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (StreamReader strReader = new StreamReader(response.GetResponseStream()))
                    {

                        var data = strReader.ReadToEnd();
                        if (string.IsNullOrWhiteSpace(data))
                        {
                            return input;
                        }
                        else
                        {
                            var doc = XDocument.Parse(data);
                            return doc.Root.Descendants("rgb")
                                .Select(x =>
                                {
                                    int R = int.Parse(x.Element("red").Value);
                                    int G = int.Parse(x.Element("green").Value);
                                    int B = int.Parse(x.Element("blue").Value);
                                    return new Circle()
                                    {
                                        ShapeId = input.ShapeId,
                                        FillColor = System.Drawing.Color.FromArgb(255, R, G, B),
                                        X = input.X,
                                        Y = input.Y,
                                        Radius = input.Radius
                                    };
                                }).FirstOrDefault() ?? input;
                        }
                    }
                }
            }
            catch
            {
                input.GenerateFillColor();
            }

            return input;
        }
    }
}
