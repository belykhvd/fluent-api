using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class ObjectPrintingExtensions
    {
        public static string PrintToString(this object serializableObject)
        {
            return ObjectPrinter.For<object>()
				//TODO RV(atolstov) опять же, почему ты определяешь конкретный список форматируемых типов? Используй IFormatable
				// Да и зачем их вообще конфигурировать по-умолчанию? Разве .ToString() не раьотает так же?
                .AddNumericTypeCultureInfo(typeof(int), CultureInfo.CurrentCulture)
                .AddNumericTypeCultureInfo(typeof(float), CultureInfo.CurrentCulture)
                .AddNumericTypeCultureInfo(typeof(double), CultureInfo.CurrentCulture)
                .PrintingProperty<int>("Age", p => $"{p} years old")
                .PrintingProperty<double>("Height", p => $"{p} cm")
                .PrintToString(serializableObject);
        }

        public static string PrintToString(this object serializableObject, 
            Func<PrintingConfig<object>, PrintingConfig<object>> configurationFunction)
        {
            return configurationFunction(ObjectPrinter.For<object>()).PrintToString(serializableObject);
        }
    }
}