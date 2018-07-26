using System;

namespace PILSharp
{
    static class NumericExtensions
    {
        internal static double Clamp(this double self, double min, double max)
        {
            return Math.Min(max, Math.Max(self, min));
        }
    }
}