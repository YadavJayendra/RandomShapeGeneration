using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Model;
using Android.Graphics.Drawables;
using ImageColorServices;
using App2.ViewModel;
using App2.Gesture;
using Android.Graphics;
using Android.Graphics.Drawables.Shapes;
using App2.Manager;

namespace App2.Views
{
    public class SquareView : LinearLayout
    {
        ShapeDrawable drawableShape = null;
        Context context = null;
        Square square = null;
        GestureDetector doubleTapDetector = null;
        GestureListener gestureListener = null;
        ImageColorService colorService = null;
        ImageCacheManager imgCacheManager = null; 
        ImageView imageView = null;
        /// <summary>
        /// The image.
        /// </summary>
        Bitmap imageVal;

        public UserAction DoubleTap { get; set; }
        

        public Bitmap Image
        {
            get
            {
                return imageVal;
            }
            set
            {
                imageVal = value;
            }
        }

        public SquareView(Context context, Model.Shape shape): 
        base (context)
        {
            this.context = context;
            this.square = shape as Square;
            colorService = ImageColorService.Instance;
            imgCacheManager = ImageCacheManager.Instance;
            DoubleTap = new UserAction(async () => HandleDoubleTap());
            Initialize();
        }

        async Task HandleDoubleTap()
        {
            await InitDrawShape();
            await ChangeImageSource();
            gestureListener.Shape = square;
        }

        async Task Initialize()
        {
            await InitDrawShape();
            InitImage();

            gestureListener = new GestureListener(square);
            doubleTapDetector = new GestureDetector(this.Context, gestureListener);

            gestureListener.DoubleTap = () => {
                DoubleTap.Invoke();
            };
            gestureListener.RequestLayout = (l, t, r, b) => {
                Layout(l, t, r, b);
                Invalidate();
            };
        }

        async Task InitDrawShape()
        {
            if (Image == null)
            {
                var paint = new Paint();
                paint.SetARGB(square.FillColor.A, square.FillColor.R, square.FillColor.G, square.FillColor.B);
                paint.SetStyle(Paint.Style.FillAndStroke);
                paint.StrokeWidth = 4;

                drawableShape = new ShapeDrawable(new RectShape());
                drawableShape.Paint.Set(paint);

                drawableShape.SetBounds(0, 0, square.SideLength * 2, square.SideLength * 2);
            }

            Image = await imgCacheManager.GetImageFromCache(square);
        }

        void InitImage()
        {
            imageView = new ImageView(this.Context);
            TextView textView = new TextView(this.Context);
            if (Image != null)
            {
                imageView.SetImageBitmap(Image);
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(square.SideLength * 2, square.SideLength * 2);
                AddView(imageView, layoutParams);
            }
            else
            {

                var paint = new Paint();
                paint.SetARGB(square.FillColor.A, square.FillColor.R, square.FillColor.G, square.FillColor.B);
                paint.SetStyle(Paint.Style.FillAndStroke);
                paint.StrokeWidth = 4;

                drawableShape = new ShapeDrawable(new RectShape());
                drawableShape.Paint.Set(paint);

                drawableShape.SetBounds(0, 0, square.SideLength * 2, square.SideLength * 2);

                textView.Text = string.Empty;
                textView.Background = drawableShape;
                LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(square.SideLength * 2, square.SideLength * 2);
                AddView(textView, layoutParams);

            }
        }

        async Task ChangeImageSource()
        {
            if (Image != null)
            {
                if (imageView == null)
                {
                    InitImage();
                    return;
                }
                imageView.SetImageBitmap(Image);
            }
        }

        public override bool OnTouchEvent(MotionEvent motion)
        {
            try
            {
                doubleTapDetector.OnTouchEvent(motion);
                gestureListener.HandleMotionEvent(motion);
            }
            catch
            {
                return true;
            }
            return true;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.Draw(canvas);
            if (drawableShape != null && Image == null)
                drawableShape.Draw(canvas);
        }
    }
}