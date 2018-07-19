using System;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace PILSharp.Sample
{
    // You exclude the 'Extension' suffix when using in Xaml markup
    [Preserve(AllMembers = true)]
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            return ImageSource.FromResource(Source);
        }
    }
}