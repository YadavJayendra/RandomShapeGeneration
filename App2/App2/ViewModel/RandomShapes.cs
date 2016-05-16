using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.ComponentModel;
using System.Threading.Tasks;
using Model;
using App2.Manager;
using ImageColorServices;
using Android.Graphics;
using App2.Views;

namespace App2.ViewModel
{
    public class RandomShapes
    {
        RelativeLayout layout = null;
        Context context = null;
        int maxHeight = 200;
        int maxWidth = 200;
        Random random = new Random();
        int shapeId = 1;
        Queue<Square> queueSquare = null;
        ImageCacheManager imgCacheService = null;
        ImageColorService imageColorService = null;
        
        public RandomShapes(RelativeLayout layout, Context context)
        {
            this.layout = layout;
            this.context = context;
            imgCacheService = ImageCacheManager.Instance;
            imageColorService = ImageColorService.Instance;
            
        }

        
        public async Task CreateNewShape(int x, int y, int height, int width)
        {
            RelativeLayout.LayoutParams layoutParams;
            Shape shape = await CreateRandonShape(height, width);

            if (shape is Circle)
            {
                Circle circle = shape as Circle;
                shape = await imageColorService.GetShape(shape);
                layoutParams = new RelativeLayout.LayoutParams(circle.Radius * 2, circle.Radius * 2);
                layoutParams.LeftMargin = x - circle.Radius;
                layoutParams.TopMargin = y - circle.Radius;

                var circleView = new CircleView(context, circle);
                layout.AddView(circleView, layoutParams);
            }
            else
            {
                Square square = shape as Square;
                layoutParams = new RelativeLayout.LayoutParams(width, height);
                layoutParams.LeftMargin = x > width ? (x - width) : (width - x);
                layoutParams.TopMargin = y > height ? (y - height) : (height - y);
                square.SideLength = square.SideLength == 0 ? random.Next(10, (width + height) / 8) : square.SideLength;

                var squareView = new SquareView(context, square);
                layout.AddView(squareView, layoutParams);
            }
        }

        /// <summary>
		/// Creates the random shape.
		/// </summary>
		/// <returns>The random shape.</returns>
		/// <param name="">.</param>
        private async Task<Shape> CreateRandonShape(int height, int width)
        {
            maxWidth = width;
            maxHeight = height;
            int randomShapeNum = random.Next(10);
            Shape shape = new Shape();
            if (randomShapeNum % 2 == 0)
            {
                Circle circle = new Circle();
                circle.Radius = circle.Radius == 0 ? random.Next(10, (width + height) / 8) : circle.Radius;

                shape = circle;
                shapeId++;
                shape.ShapeId = shapeId;
            }
            else
            {
                shape = await GetSquareFromQueue();
            }

            SetShape(ref shape, true, height, width);

            return shape;
        }

        /// <summary>
		/// Gets the square from queue.
		/// </summary>
		/// <returns>The square from queue.</returns>
		public async Task<Square> GetSquareFromQueue()
        {
            if (queueSquare == null || queueSquare.Count == 0)
                await LoadSquare();
            var shape = queueSquare.Dequeue();
            await Task.Run(() => LoadSquare());
            return shape;
        }

        /// <summary>
		/// Loads the square.
		/// </summary>
		/// <returns>The square.</returns>
		private async Task LoadSquare()
        {
            int imgCount = imgCacheService.imgQueueCount;
            if (queueSquare == null)
                queueSquare = new Queue<Square>(imgCount);
            while (queueSquare.Count < imgCount)
            {
                shapeId++;
                var shape = new Square()
                {
                    ShapeId = shapeId
                };
                shape.Y = shape.Y == 0 ? random.Next(10, maxHeight) : shape.Y;
                shape.X = shape.X == 0 ? random.Next(10, maxWidth) : shape.X;
                //shape.Radius = shape.Radius == 0 ? rand.Next(10, (iMaxWidth + iMaxHeight) / 8) : shape.Radius;
                //SetShape(ref shape,false,iMaxHeight,iMaxWidth);
                var shapeValue = await imgCacheService.SetImageToCache(shape);
                queueSquare.Enqueue(shapeValue);

            }
        }

        /// <summary>
        /// Sets the shape.
        /// </summary>
        /// <param name="shape">Shape.</param>
        /// <param name="bFlag">If set to <c>true</c> b flag.</param>
        /// <param name="iMaxHeight">I max height.</param>
        /// <param name="iMaxWidth">I max width.</param>
        private void SetShape(ref Shape shape, bool flag, int height, int width)
        {
            shape.Y = shape.Y == 0 ? random.Next(10, height) : shape.Y;
            shape.X = shape.Y == 0 ? random.Next(10, width) : shape.X;
            if (flag)
                shape.GenerateFillColor();

        }
    }
}