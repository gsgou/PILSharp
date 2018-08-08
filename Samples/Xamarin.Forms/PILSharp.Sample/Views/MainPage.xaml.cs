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
            BindingContext = new MainPageViewModel();
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
            Target1.Source = Process("vgl_5674_0098.bmp", out elapsedTime);
            Target1Label.Text = string.Format("{0}ms", elapsedTime);
        }

        void OnTapSource2(object sender, EventArgs args)
        {
            Target2.Source = Process("vgl_6434_0018a.bmp", out elapsedTime);
            Target2Label.Text = string.Format("{0}ms", elapsedTime);
        }

        void OnTapSource3(object sender, EventArgs args)
        {
            Target3.Source = Process("vgl_6548_0026a.bmp", out elapsedTime);
            Target3Label.Text = string.Format("{0}ms", elapsedTime);
        }

        void OnTapSource4(object sender, EventArgs args)
        {
            Target4.Source = Process("nightshot_iso_100.bmp", out elapsedTime);
            Target4Label.Text = string.Format("{0}ms", elapsedTime);
        }

        ImageSource Process(string resourceFileName, out long elapsedTime)
        {
            byte[] imageData = ResourceLoader.GetEmbeddedResourceBytes(assembly, resourceFileName);
            PILSharp.PILThickness border;
            PILSharp.PILColor fill;
            byte[] result = Array.Empty<byte>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            switch ((BindingContext as MainPageViewModel).SelectedImageOp)
            {
                case ImageOpsEnum.Crop:
                    border = new PILSharp.PILThickness(20d, 10d, 20d, 10d);
                    result = PILSharp.ImageOps.Crop(imageData, border);
                    break;
                case ImageOpsEnum.Equalize:
                    result = PILSharp.ImageOps.Equalize(imageData);
                    break;
                case ImageOpsEnum.Expand:
                    border = new PILSharp.PILThickness(20d, 10d, 40d, 20d);
                    fill = new PILSharp.PILColor(0, 0, 0, 255);
                    result = PILSharp.ImageOps.Expand(imageData, border, fill);
                    break;
                case ImageOpsEnum.Fit:
                    result = PILSharp.ImageOps.Fit(imageData, 100, false);
                    break;
                default: 
                    break;
            }

            stopwatch.Stop();
            elapsedTime = stopwatch.ElapsedMilliseconds;

            return ImageSource.FromStream(() => new MemoryStream(result));
        }
    }
}