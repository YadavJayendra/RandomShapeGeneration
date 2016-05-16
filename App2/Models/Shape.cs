using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Model
{
    public class Shape
    {
        /// <summary>
        /// Id of shape
        /// </summary>
        public int ShapeId { get; set; }
        
        /// <summary>
        /// X Cordinate
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Y cordinate
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// Holds filled color for shape
        /// </summary>
        public Color FillColor { get; set; }

        /// <summary>
        /// Generating filled color 
        /// </summary>
        public void GenerateFillColor()
        {
            Random random = new Random();
            this.FillColor = Color.FromArgb(255, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }
    }
}