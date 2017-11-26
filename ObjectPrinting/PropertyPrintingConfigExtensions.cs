using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(this PropertyPrintingConfig<TOwner, string> propertyPrintingConfig, 
            int maxLength)
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, string>)propertyPrintingConfig).ParentConfig;
            parentConfig.StringMaxLength = maxLength;
            return parentConfig;
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, double> propertyPrintingConfig,
            CultureInfo cultureInfo)
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, double>) propertyPrintingConfig).ParentConfig;
            parentConfig.NumericTypesCultureInfos[typeof(double)] = cultureInfo;
            return parentConfig;
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, float> propertyPrintingConfig,
            CultureInfo cultureInfo)
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, float>)propertyPrintingConfig).ParentConfig;
            parentConfig.NumericTypesCultureInfos[typeof(float)] = cultureInfo;
            return parentConfig;
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, int> propertyPrintingConfig,
            CultureInfo cultureInfo)
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, int>)propertyPrintingConfig).ParentConfig;
            parentConfig.NumericTypesCultureInfos[typeof(int)] = cultureInfo;
            return parentConfig;
        }
    }
}