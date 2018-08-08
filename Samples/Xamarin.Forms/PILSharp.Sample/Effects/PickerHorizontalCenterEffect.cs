using Xamarin.Forms;

namespace PILSharp.Sample
{
    // This class derives from RoutingEffect, is named using Effect, and passes a string to the base constructor.
    // This string is used to route the effect to the relevant platform specific implementation.
    public class PickerHorizontalCenterEffect : RoutingEffect
    {
        public PickerHorizontalCenterEffect() : base("PILSharp.Sample.PickerHorizontalCenterEffect")
        {

        }
    }
}