using System;
using System.Collections.Generic;
using System.Linq;

namespace PILSharp.Sample
{
    public class MainPageViewModel : ViewModelBase
    {
        ImageOpsEnum selectedImageOp;
        public ImageOpsEnum SelectedImageOp
        {
            get => selectedImageOp;
            set
            {
                if (selectedImageOp != value)
                {
                    selectedImageOp = value;
                    OnPropertyChanged(nameof(SelectedImageOp));
                }
            }
        }

        public IList<string> ImageOpsNames
        {
            get
            {
                return Enum.GetNames(typeof(ImageOpsEnum)).Select(op => op.SplitCamelCase()).ToList();
            }
        }
    }
}