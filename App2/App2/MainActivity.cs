using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using App2.ViewModel;
using Android.Hardware;

namespace App2
{
    [Activity(Label = "RandonShapes", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, ISensorEventListener
    {
        int count = 1;
        RelativeLayout layout = null;
        RandomShapes randomShapeModel = null;
        float last_x = 0.0f;
        float last_y = 0.0f;
        float last_z = 0.0f;
        bool bUpdated = false;
        DateTime dtLastUpdate;
        const int ShakeDetectionTimeLapse = 100;
        const int ShakeThreshold = 800;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            /// Getting ralative layout for drawing
            RelativeLayout mainLayout = FindViewById<RelativeLayout>(Resource.Id.layoutId);
            randomShapeModel = new RandomShapes(mainLayout, this);
            mainLayout.Touch += async (sender, e) => {
                if ((e.Event.Action & MotionEventActions.Mask) == MotionEventActions.Down)
                {
                    await randomShapeModel.CreateNewShape((int)e.Event.GetX(),
                        (int)e.Event.GetY(),
                        (int)Resources.DisplayMetrics.HeightPixels,
                        (int)Resources.DisplayMetrics.WidthPixels);
                }
            };

            var sensorManager = GetSystemService(SensorService) as SensorManager;
            var sensor = sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            sensorManager.RegisterListener(this, sensor, SensorDelay.Ui);
        }


        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                float x = e.Values[0];
                float y = e.Values[1];
                float z = e.Values[2];

                DateTime curDateTime = DateTime.Now;
                if (bUpdated == false)
                {
                    bUpdated = true;
                    dtLastUpdate = curDateTime;
                    last_x = x;
                    last_y = y;
                    last_z = z;

                }
                else
                {
                    if ((curDateTime - dtLastUpdate).TotalMilliseconds > ShakeDetectionTimeLapse)
                    {
                        float diffTime = (float)(curDateTime - dtLastUpdate).TotalMilliseconds;
                        dtLastUpdate = curDateTime;
                        float total = x + y + z - last_x - last_y - last_z;
                        float speed = Math.Abs(total) / diffTime * 10000;

                        if (speed > ShakeThreshold)
                        {

                            RelativeLayout mainLayout = FindViewById<RelativeLayout>(Resource.Id.layoutId);
                            mainLayout.RemoveAllViews();
                        }

                        last_x = x;
                        last_y = y;
                        last_z = z;
                    }
                }
            }
        }
    }
}

