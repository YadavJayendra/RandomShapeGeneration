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
using Model;

namespace App2.Gesture
{
    public class GestureListener: GestureDetector.SimpleOnGestureListener
    {
        public Action DoubleTap;

        public Action<int, int, int, int> RequestLayout;

        public Shape Shape { get; set; }

        float xTouch;
        float yTouch;
        float xVal = 0;
        float yVal = 0;
        int idPoint;

        public GestureListener(Shape shape)
        {
            Shape = shape;
        }

        /// <summary>
		/// Handles the drag event.
		/// </summary>
		/// <param name="ev">Ev.</param>
		public void HandleMotionEvent(MotionEvent ev)
        {
            MotionEventActions action = ev.Action & MotionEventActions.Mask;
            int pointerIndex;
            int value = 0;
            if (Shape is Circle)
            {
                Circle circle = Shape as Circle;
                value = circle.Radius;
            }
            else
            {
                Square square = Shape as Square;
                value = square.SideLength;
            }

            switch (action)
            {
                case MotionEventActions.Down:
                    xTouch = ev.RawX;
                    yTouch = ev.RawY;
                    idPoint = ev.GetPointerId(0);
                    
                    xVal = xVal == 0 ? xTouch - value : xVal;
                    yVal = yVal == 0 ? yTouch - value : yVal;
                    
                    break;

                case MotionEventActions.Move:
                    pointerIndex = ev.FindPointerIndex(idPoint);
                    float x = ev.RawX;
                    float y = ev.RawY;

                    float deltaX = x - xTouch;
                    float deltaY = y - yTouch;
                    xVal += deltaX;
                    yVal += deltaY;
                    if (RequestLayout != null)
                        RequestLayout((int)xVal, (int)yVal, (int)xVal + value * 2, (int)yVal + value * 2);

                    xTouch = x;
                    yTouch = y;
                    break;

                case MotionEventActions.PointerUp:
                    // check to make sure that the pointer that went up is for the gesture we're tracking.
                    pointerIndex = (int)(ev.Action & MotionEventActions.PointerIndexMask) >> (int)MotionEventActions.PointerIndexShift;
                    int pointerId = ev.GetPointerId(pointerIndex);
                    if (pointerId == idPoint)
                    {

                        xTouch = ev.RawX;
                        yTouch = ev.RawY;
                        Shape.X = (int)xVal + value;
                        Shape.Y = (int)yVal + value;

                    }
                    break;

                case MotionEventActions.Up:
                    xTouch = ev.RawX;
                    yTouch = ev.RawY;
                    Shape.X = (int)xVal + value;
                    Shape.Y = (int)yVal + value;
                    xVal = 0;
                    yVal = 0;
                    break;

                case MotionEventActions.Cancel:
                    idPoint = -1;
                    break;

            }
        }

        public override bool OnDoubleTap(MotionEvent e)
        {
            if (Shape is Circle)
            {
                Shape.GenerateFillColor();
            }
            else
            {
                Square square = Shape as Square;
            }
            return base.OnDoubleTap(e);
        }

    }
}