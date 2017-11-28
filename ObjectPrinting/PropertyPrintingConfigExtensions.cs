﻿using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtensions
    {
        public static PrintingConfig<TOwner> TrimmedToLength<TOwner>(this PropertyPrintingConfig<TOwner, string> propertyPrintingConfig, 
            int maxLength)
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, string>)propertyPrintingConfig).ParentConfig;
            return parentConfig.ChangeStringMaxLength(maxLength);            
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, double> propertyPrintingConfig,
            CultureInfo cultureInfo)
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, double>) propertyPrintingConfig).ParentConfig;
            return parentConfig.AddNumericTypeCultureInfo(typeof(double), cultureInfo);            
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, float> propertyPrintingConfig,
            CultureInfo cultureInfo)
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, float>)propertyPrintingConfig).ParentConfig;
            return parentConfig.AddNumericTypeCultureInfo(typeof(float), cultureInfo);
        }

        public static PrintingConfig<TOwner> Using<TOwner>(this PropertyPrintingConfig<TOwner, int> propertyPrintingConfig,
            CultureInfo cultureInfo)
        {
            var parentConfig = ((IPropertyPrintingConfig<TOwner, int>)propertyPrintingConfig).ParentConfig;
            return parentConfig.AddNumericTypeCultureInfo(typeof(int), cultureInfo);
        }
    }
}