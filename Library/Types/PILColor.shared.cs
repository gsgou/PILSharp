namespace PILSharp
{ 
    public struct PILColor
    {
        // Try to match Xamarin.Forms:
        // https://github.com/xamarin/Xamarin.Forms/blob/master/Xamarin.Forms.Core/Color.cs

        readonly float _a;

        public double A
        {
            get { return _a; }
        }

        readonly float _r;

        public double R
        {
            get { return _r; }
        }

        readonly float _g;

        public double G
        {
            get { return _g; }
        }

        readonly float _b;

        public double B
        {
            get { return _b; }
        }

        public PILColor(double r, double g, double b, double a)
        {
            _r = (float)r.Clamp(0, 1);
            _g = (float)g.Clamp(0, 1);
            _b = (float)b.Clamp(0, 1);
            _a = (float)a.Clamp(0, 1);
        }
    }
}