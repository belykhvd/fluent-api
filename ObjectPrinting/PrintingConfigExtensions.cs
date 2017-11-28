using System.Globalization;

namespace ObjectPrinting
{
    public static class PrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> Default<TOwner>(this PrintingConfig<TOwner> printingConfig)
        {
            return printingConfig
                .AddNumericTypeCultureInfo(typeof(int), CultureInfo.CurrentCulture)
                .AddNumericTypeCultureInfo(typeof(float), CultureInfo.CurrentCulture)
                .AddNumericTypeCultureInfo(typeof(double), CultureInfo.CurrentCulture)
                .PrintingProperty<int>("Age", p => $"{p} years old")
                .PrintingProperty<double>("Height", p => $"{p} cm");
        }
    }
}