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
using App2.ViewModel;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Graphics;
using ImageColorServices;
using App2.Gesture;

namespace App2.Views
{
    public class CircleView : View
    {
        ShapeDrawable drawableShape = null;
        Context context = null;
        Circle circle = null;
        GestureDetector doubleTapDetector = null;
        GestureListener gestureListener = null;
        ImageColorService colorService = null;

        public UserAction DoubleTap { get; set; }

        public CircleView(Context context, Model.Shape shape):
        base(context)
        {
            this.context = context;
            this.circle = shape as Circle;
            colorService = ImageColorService.Instance;
            DoubleTap = new UserAction(async () => HandleDoubleTap());
            Initialize();
        }

        /// <summary>
		/// Handles the double tap.
		/// </summary>
		/// <returns>The double tap.</returns>
		async Task HandleDoubleTap()
        {
            circle.GenerateFillColor();
            circle = await colorService.GetShape(circle) as Circle;
            InitDrawShape();
        }

        /// <summary>
		/// Initialize this instance.
		/// </summary>
		void Initialize()
        {
            InitDrawShape();

            gestureListener = new GestureListener(circle);
            doubleTapDetector = new GestureDetector(this.Context, gestureListener);

            gestureListener.DoubleTap = () => {
                DoubleTap.Invoke();
            };

            gestureListener.RequestLayout = (l, t, r, b) => {
                Layout(l, t, r, b);
                Invalidate();
            };
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            doubleTapDetector.OnTouchEvent(ev);
            gestureListener.HandleMotionEvent(ev);
            return true;
        }

        /// <summary>
        /// Inits the draw shape.
        /// </summary>
        void InitDrawShape()
        {
            drawableShape = null;
            var paint = new Paint();
            paint.SetARGB(circle.FillColor.A, circle.FillColor.R, circle.FillColor.G, circle.FillColor.B);
            paint.SetStyle(Paint.Style.FillAndStroke);
            paint.StrokeWidth = 2;

            drawableShape = new ShapeDrawable(new OvalShape());
            drawableShape.Paint.Set(paint);

            drawableShape.SetBounds(0, 0, circle.Radius * 2, circle.Radius * 2);
        }

        protected override void OnDraw(Canvas canvas)
        {
            drawableShape.Draw(canvas);
        }
        
    }
}