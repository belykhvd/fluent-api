using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class ObjectPrintingExtensions
    {
        public static string PrintToString<T>(this T serializableObject)
        {
            return ObjectPrinter.For<object>().PrintToString(serializableObject);
        }

        public static string PrintToString<T>(this T serializableObject, 
            Func<PrintingConfig<T>, PrintingConfig<T>> configurationFunction)
        {
            return configurationFunction(ObjectPrinter.For<T>()).PrintToString(serializableObject);
        }
    }
}