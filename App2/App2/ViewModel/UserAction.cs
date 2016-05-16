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

namespace App2.ViewModel
{
    public class UserAction
    {
        Action executeCommand = null;

        public UserAction(Action action)
        {
            executeCommand = action;
        }

        public void Invoke()
        {
            executeCommand();
        }
    }
}