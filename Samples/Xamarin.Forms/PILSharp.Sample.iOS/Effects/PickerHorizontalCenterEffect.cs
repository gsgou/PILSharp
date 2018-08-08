using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("PILSharp.Sample")]
[assembly: ExportEffect(typeof(PILSharp.Sample.iOS.PickerHorizontalCenterEffect), nameof(PILSharp.Sample.PickerHorizontalCenterEffect))]
namespace PILSharp.Sample.iOS
{
    public class PickerHorizontalCenterEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control is UITextField uiTextField)
            {
                uiTextField.TextAlignment = UITextAlignment.Center;
            }
        }

        // In base Effect OnDetached is declared as an abstract method. We have to implement the method in inherited class.
        // No implementation is provided by the OnDetached method because no cleanup is necessary.
        protected override void OnDetached()
        {

        }
    }
}