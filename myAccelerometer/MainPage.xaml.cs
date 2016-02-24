using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace myAccelerometer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Accelerometer myAccel;
        Gyrometer gyrometer; 
        private uint _desiredReportInterval;
        DispatcherTimer dispatcherTimer; //move the block
        double maxWidth, maxHeight;

        public MainPage()
        {
            this.InitializeComponent();
            setupTimers();
            checkForAccelerometer();
            checkForGyro();
        }

        private async void checkForGyro()
        {
            MessageDialog msgDialog = new MessageDialog("");
            gyrometer = Gyrometer.GetDefault();
            if (gyrometer == null)
            {
                msgDialog.Content = "No gyro present";
                await msgDialog.ShowAsync();
            }
            else
            {
                gyrometer.ReadingChanged += Gyrometer_ReadingChanged;

            }
        }

        private async void Gyrometer_ReadingChanged(Gyrometer sender, GyrometerReadingChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                updateUIGyro(args.Reading);  // update the UI with that reading
            });

        }

        private void updateUIGyro(GyrometerReading reading)
        {
            statusTextBlock.Text = "AngVel Y: " + reading.AngularVelocityY.ToString();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            maxWidth = Window.Current.Bounds.Width;
            maxHeight = Window.Current.Bounds.Height;
        }

        private void setupTimers()
        {
            if( dispatcherTimer == null)
            {
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                dispatcherTimer.Tick += DispatcherTimer_Tick;
            }
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            double numX = Convert.ToDouble(xTextBlock.Text.Substring(2));
            double numY = Convert.ToDouble(yTextBlock.Text.Substring(2));

            #region move X direction
            if (numX <= 0)
            {
                if ((double)myRectangle.GetValue(Canvas.LeftProperty) <= 0)// Canvas.Left == 0)
                { } // do nothing }
                else if ((double)myRectangle.GetValue(Canvas.LeftProperty) > 0)
                {
                    // move left Canvas.Left --;
                    myRectangle.SetValue(Canvas.LeftProperty,
                        (double)myRectangle.GetValue(Canvas.LeftProperty) - 1);
                }
            }
            else
            {
                if (((double)myRectangle.GetValue(Canvas.LeftProperty) >=
                                (myCanvas.Width - myRectangle.Width)))
                { }
                else
                {
                    myRectangle.SetValue(Canvas.LeftProperty,
                        (double)myRectangle.GetValue(Canvas.LeftProperty) + 1);
                }
            }
            #endregion

            #region move Y direction
            // when the phone is flat, +ve Y means move up and -ve Y means move down (increase Canvas.Top)
            if (numY <= 0)
            {
                if ((double)myRectangle.GetValue(Canvas.TopProperty) <= 0)
                { } // do nothing }
                else if ((double)myRectangle.GetValue(Canvas.TopProperty) > 0)
                {
                    // move left Canvas.Top ++ to move down;
                    myRectangle.SetValue(Canvas.TopProperty,
                        (double)myRectangle.GetValue(Canvas.TopProperty) + 1);
                }
            }
            else // move up by decreasing Canvas.Top
            {
                if (((double)myRectangle.GetValue(Canvas.TopProperty) >=
                                (myCanvas.Height - myRectangle.Height)))
                { }
                else
                {
                    myRectangle.SetValue(Canvas.TopProperty,
                        (double)myRectangle.GetValue(Canvas.TopProperty) - 1);
                }
            }
            #endregion

        }

        private async void checkForAccelerometer()
        {
            MessageDialog msgDialog = new MessageDialog(""); 
            myAccel = Accelerometer.GetDefault();
            if (myAccel == null)
            {
                msgDialog.Content = "No accelerometer present";
                await msgDialog.ShowAsync();
            }
            else
            {
                // Establish the report interval
                uint minReportInterval = myAccel.MinimumReportInterval;
                _desiredReportInterval = minReportInterval > 100 ? minReportInterval : 100;
                myAccel.ReportInterval = _desiredReportInterval;
                myAccel.ReadingChanged += MyAccel_ReadingChanged;
                myAccel.Shaken += MyAccel_Shaken; // maybe not

                dispatcherTimer.Start();

            }

        }

        private async void MyAccel_Shaken(Accelerometer sender, AccelerometerShakenEventArgs args)
        {
            MessageDialog msgDialog = new MessageDialog("Don't do that - not good.");
            await msgDialog.ShowAsync();
        }

        private async void MyAccel_ReadingChanged(Accelerometer sender, 
                                            AccelerometerReadingChangedEventArgs args)
        {
            
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                AccelerometerReading reading = args.Reading;    // gets the reading 
                updateUI(reading);  // update the UI with that reading
            }
                                    );
        }

        private void updateUI(AccelerometerReading reading)
        {
            // Show the numeric values.
            // AccelerationX is the force along the x axiscx
            xTextBlock.Text = "X: " + reading.AccelerationX.ToString("0.000");
            yTextBlock.Text = "Y: " + reading.AccelerationY.ToString("0.000");
            zTextBlock.Text = "Z: " + reading.AccelerationZ.ToString("0.000");

            // Show the values graphically.
            xLine.X2 = xLine.X1 + reading.AccelerationX * 200;
            yLine.Y2 = yLine.Y1 - reading.AccelerationY * 200;
            zLine.X2 = zLine.X1 - reading.AccelerationZ * 100;
            zLine.Y2 = zLine.Y1 + reading.AccelerationZ * 100;

            // if acceleration is along the x axis, then move the rectangle
            // to the left if minus, to the right if positive.


        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
