using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(this PropertyPrintingConfig<TOwner, string> propertyPrintingConfig,
            int maxLength)
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, string>) propertyPrintingConfig).ParentConfig;
            return parentConfig.ChangeStringMaxLength(maxLength);
        }

        public static PrintingConfig<TOwner> Using<TOwner, T>(this PropertyPrintingConfig<TOwner, T> propertyPrintingConfig,
            CultureInfo cultureInfo) where T : IFormattable
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, T>) propertyPrintingConfig).ParentConfig;
            return parentConfig.AddNumericTypeCultureInfo(typeof(T), cultureInfo);
        }
    }
}