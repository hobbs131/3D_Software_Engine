using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SoftEngine
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Device device;
        Mesh[] meshes;
        Camera mera = new Camera();
        DateTime previousDate;
        private Collection<double> lastFPSValues = new Collection<double>();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Choose the back buffer resolution here
            WriteableBitmap bmp = new WriteableBitmap((int)ActualWidth, (int)ActualHeight);

            // Our Image XAML control
            frontBuffer.Source = bmp;
            
            device = new Device(bmp);
            meshes = await device.LoadJSONFileAsync("monkey.babylon");
            mera.Position = new Vector3(0, 0, 10.0f);
            mera.Target = Vector3.Zero;

            // Registering to the XAML rendering loop
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        // Rendering loop handler
        void CompositionTarget_Rendering(object sender, object e)
        {
            // Fps
            var now = DateTime.Now;
            var currentFPS = 1000.0 / (now - previousDate).TotalMilliseconds;
            previousDate = now;

            fps.Text = string.Format("instant {0:0.00} fps", currentFPS);

            if (lastFPSValues.Count < 60)
            {
                lastFPSValues.Add(currentFPS);
            }
            else
            {
                lastFPSValues.RemoveAt(0);
                lastFPSValues.Add(currentFPS);
                var totalValues = 0d;
                for (var i = 0; i < lastFPSValues.Count; i++)
                {
                    totalValues += lastFPSValues[i];
                }

                var averageFPS = totalValues / lastFPSValues.Count;
                averageFps.Text = string.Format("average {0:0.00} fps", averageFPS);
            }

            device.Clear(0, 0, 0, 255);

            foreach (var mesh in meshes) {
                // rotating slightly the meshes during each frame rendered
                //mesh.Rotation = new Vector3(mesh.Rotation.X + 0.01f, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z);
                mesh.Rotation = new Vector3(mesh.Rotation.X, mesh.Rotation.Y + 0.01f, mesh.Rotation.Z);
                //mesh.Position = new Vector3(0, 0, (float)(5 * Math.Cos(alpha)));
            }

            // Doing the various matrix operations
            device.Render(mera, meshes);
            // Flushing the back buffer into the front buffer
            device.Present();
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
