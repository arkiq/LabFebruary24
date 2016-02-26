using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;




// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace myAccelerometerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
        }

        bool _isRunning;

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, 
                                        Windows.UI.Core.KeyEventArgs args)
        {
            if (_isRunning == false) return;

            if((args.VirtualKey == Windows.System.VirtualKey.Up ) ||
                (args.VirtualKey == VirtualKey.GamepadLeftThumbstickUp))
            {
                movePiece.SetValue(Canvas.TopProperty, 
                                    (double)movePiece.GetValue(Canvas.TopProperty) - 1);
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.Down )
            {
                movePiece.SetValue(Canvas.TopProperty,
                                    (double)movePiece.GetValue(Canvas.TopProperty) + 1);
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.Left )
            {
                movePiece.SetValue(Canvas.LeftProperty,
                                    (double)movePiece.GetValue(Canvas.LeftProperty) - 1);
            }
            else if (args.VirtualKey == Windows.System.VirtualKey.Right)
            {
                movePiece.SetValue(Canvas.LeftProperty,
                                    (double)movePiece.GetValue(Canvas.LeftProperty) + 1);
            }



        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if( _isRunning == false)
            {
                _isRunning = true;
                btnStartStop.Content = "Stop";
            }
            else
            {
                _isRunning = false;
                btnStartStop.Content = "Start";
            }

        }
    }
}
