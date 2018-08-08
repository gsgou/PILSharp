using Android.Widget;
using Android.Views;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("PILSharp.Sample")]
[assembly: ExportEffect(typeof(PILSharp.Sample.Droid.PickerHorizontalCenterEffect), nameof(PILSharp.Sample.PickerHorizontalCenterEffect))]
namespace PILSharp.Sample.Droid
{
    public class PickerHorizontalCenterEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control is EditText editText)
            {
                editText.Gravity = GravityFlags.CenterHorizontal;
            }
        }

        // In base Effect OnDetached is declared as an abstract method. We have to implement the method in inherited class.
        // No implementation is provided by the OnDetached method because no cleanup is necessary.
        protected override void OnDetached()
        {

        }
    }
}