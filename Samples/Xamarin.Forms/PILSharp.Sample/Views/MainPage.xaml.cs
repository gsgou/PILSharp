using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Xamarin.Forms;

namespace PILSharp.Sample
{
    public partial class MainPage : ContentPage
    {
        static Assembly assembly = typeof(MainPage).GetTypeInfo().Assembly;
        long elapsedTime;

        public MainPage()
        {
            InitializeComponent();
#if DEBUG
            // Debugging embedded resources
            foreach (var res in assembly.GetManifestResourceNames())
            {
                Debug.WriteLine("found resource: " + res);
            }
#endif
        }

        void OnTapSource1(object sender, EventArgs args)
        {
            Target1.Source = Equalize("vgl_5674_0098.bmp", out elapsedTime);
            Target1Label.Text = string.Format("{0}ms", elapsedTime);
        }

        void OnTapSource2(object sender, EventArgs args)
        {
            Target2.Source = Equalize("vgl_6434_0018a.bmp", out elapsedTime);
            Target2Label.Text = string.Format("{0}ms", elapsedTime);
        }

        void OnTapSource3(object sender, EventArgs args)
        {
            Target3.Source = Equalize("vgl_6548_0026a.bmp", out elapsedTime);
            Target3Label.Text = string.Format("{0}ms", elapsedTime);
        }

        void OnTapSource4(object sender, EventArgs args)
        {
            Target4.Source = Equalize("nightshot_iso_100.bmp", out elapsedTime);
            Target4Label.Text = string.Format("{0}ms", elapsedTime);
        }

        ImageSource Equalize(string resourceFileName, out long elapsedTime)
        {
            byte[] imageData = ResourceLoader.GetEmbeddedResourceBytes(assembly, resourceFileName);
            var bitmapData = PILSharp.ImageOps.GetBitmapData(imageData);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var result = PILSharp.ImageOps.Equalize(imageData, bitmapData);

            //var border = new PILSharp.PILThickness(20d, 10d, 40d, 20d);
            //var fill = new PILSharp.PILColor(0, 0, 0, 255);
            //var result = PILSharp.ImageOps.Expand(imageData, bitmapData, border, fill);

            stopwatch.Stop();
            elapsedTime = stopwatch.ElapsedMilliseconds;

            return ImageSource.FromStream(() => new MemoryStream(result));
        }
    }
}