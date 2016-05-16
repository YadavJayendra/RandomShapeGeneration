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

namespace Model
{
    public class Square:Shape
    {
        /// <summary>
        /// Holds image path
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// lent of each side in pixels
        /// </summary>
        public int SideLength { get; set; }
    }
}