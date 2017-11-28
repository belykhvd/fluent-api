using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ObjectPrinting
{
    //TODO RV(atolstov) попробуй сделать данный класс неизменяемым
    public class PrintingConfig<TOwner>
    {
        private readonly HashSet<Type> excludingTypes = new HashSet<Type>();
        private readonly HashSet<string> excludingProperties = new HashSet<string>();
        private readonly Dictionary<string, Delegate> propertiesSerializationFunctions = new Dictionary<string, Delegate>();

        //TODO RV(atolstov) эти свойства должны быть private
        public int StringMaxLength { get; set; } = -1;    
        public Dictionary<Type, Delegate> TypesSerializationFunctions { get; } = new Dictionary<Type, Delegate>();
        public Dictionary<Type, CultureInfo> NumericTypesCultureInfos { get; } = new Dictionary<Type, CultureInfo>();        

        public PrintingConfig<TOwner> ExcludingType<TPropType>()
        {            
            excludingTypes.Add(typeof(TPropType));
            return this;
        }

        public PrintingConfig<TOwner> ExcludingProperty(string name)
        {
            excludingProperties.Add(name);
            return this;
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> selector)
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PrintingConfig<TOwner> PrintingProperty<TPropType>(string name, Func<TPropType, string> serializationFunction)
        {
            propertiesSerializationFunctions[name] = serializationFunction;
            return this;
        }        

        public string PrintToString(TOwner obj) => PrintToString(obj, 0);

        private string PrintToString(object obj, int nestingLevel)
        {            
            if (obj == null)
                return "null" + Environment.NewLine;

            var objectType = obj.GetType();
            var serializeWithFunction =
                TypesSerializationFunctions.TryGetValue(objectType, out var serializationFunction);

            var isNumericWithCulture = NumericTypesCultureInfos.TryGetValue(objectType, out var numericCulture);

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };

            if (finalTypes.Contains(objectType))
            {                
                var representation = serializeWithFunction
                    ? serializationFunction.DynamicInvoke(obj).ToString()
                    : isNumericWithCulture
                        ? ((IFormattable) obj).ToString(null, numericCulture)
                        : obj.ToString();

                if (StringMaxLength >= 0
                    && objectType == typeof(string)
                    && representation.Length > StringMaxLength)
                {
                    representation = representation.Substring(0, StringMaxLength);
                }

                return representation + Environment.NewLine;
            }

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder()
                .AppendLine(objectType.Name);

            if (serializeWithFunction)
            {
                return sb
                    .AppendLine(serializationFunction.DynamicInvoke(obj).ToString())
                    .ToString();
            }

            foreach (var propertyInfo in objectType.GetProperties())
            {
                var propertyType = propertyInfo.PropertyType;
                if (excludingTypes.Contains(propertyType))
                    continue;

                var propertyName = propertyInfo.Name;
                if (nestingLevel == 0 && excludingProperties.Contains(propertyName))
                    continue;

                if (nestingLevel == 0 
                    && propertiesSerializationFunctions.TryGetValue(propertyName, out serializationFunction))
                {
                    sb.AppendLine($"{identation}{propertyInfo.Name} = {serializationFunction.DynamicInvoke(propertyInfo.GetValue(obj))}");
                    continue;                    
                }

                sb.Append($"{identation}{propertyInfo.Name} = {PrintToString(propertyInfo.GetValue(obj), nestingLevel + 1)}");
            }

            return sb.ToString();
        }
    }
}