using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ObjectPrinting
{
    public class PrintingConfig<TOwner>
    {        
        private ImmutableHashSet<Type> excludingTypes = ImmutableHashSet<Type>.Empty;
        private ImmutableHashSet<string> excludingProperties = ImmutableHashSet<string>.Empty;        
        private ImmutableDictionary<Type, Delegate> typesSerializationFunctions = ImmutableDictionary<Type, Delegate>.Empty;
        private ImmutableDictionary<Type, CultureInfo> numericTypesCultureInfos = ImmutableDictionary<Type, CultureInfo>.Empty;
        private ImmutableDictionary<string, Delegate> propertiesSerializationFunctions = ImmutableDictionary<string, Delegate>.Empty;
        private int stringMaxLength = -1;

        public PrintingConfig()
        {           
        }

        public PrintingConfig(PrintingConfig<TOwner> printingConfig)
        {
            excludingTypes = printingConfig.excludingTypes;
            excludingProperties = printingConfig.excludingProperties;
            typesSerializationFunctions = printingConfig.typesSerializationFunctions;
            numericTypesCultureInfos = printingConfig.numericTypesCultureInfos;
            propertiesSerializationFunctions = printingConfig.propertiesSerializationFunctions;
            stringMaxLength = printingConfig.stringMaxLength;
        }

        public PrintingConfig<TOwner> ExcludingType<TPropType>()
        {
            return new PrintingConfig<TOwner>(this)
            {
                excludingTypes = excludingTypes.Add(typeof(TPropType))
            };
        }

        public PrintingConfig<TOwner> ExcludingProperty(string name)
        {            
            return new PrintingConfig<TOwner>(this)
            {
                excludingProperties = excludingProperties.Add(name)
            };
        }

        public PrintingConfig<TOwner> PrintingProperty<TPropType>(string name, Func<TPropType, string> serializationFunction)
        {
            return new PrintingConfig<TOwner>(this)
            {
                propertiesSerializationFunctions = propertiesSerializationFunctions.Add(name, serializationFunction)
            };
        }

        public PrintingConfig<TOwner> AddTypeSerializationFunction(Type type, Delegate function)
        {
            return new PrintingConfig<TOwner>(this)
            {
                typesSerializationFunctions = typesSerializationFunctions.Add(type, function)
            };            
        }

        public PrintingConfig<TOwner> AddNumericTypeCultureInfo(Type type, CultureInfo cultureInfo)
        {
            return new PrintingConfig<TOwner>(this)
            {
                numericTypesCultureInfos = numericTypesCultureInfos.Add(type, cultureInfo)
            };
        }

        public PrintingConfig<TOwner> AddPropertySerializationFunction(string name, Delegate function)
        {
            return new PrintingConfig<TOwner>(this)
            {
                propertiesSerializationFunctions = propertiesSerializationFunctions.Add(name, function)
            };
        }

        public PrintingConfig<TOwner> ChangeStringMaxLength(int maxLength)
        {
            return new PrintingConfig<TOwner>(this)
            {
                stringMaxLength = maxLength
            };
        }        

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }

        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> selector)
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this);
        }
          
        public string PrintToString(TOwner obj) => PrintToString(obj, 0);

        private string PrintToString(object obj, int nestingLevel)
        {            
            if (obj == null)
                return "null" + Environment.NewLine;

            var objectType = obj.GetType();
            var serializeWithFunction = 
                typesSerializationFunctions.TryGetValue(objectType, out var serializationFunction);

            var isNumericWithCulture = numericTypesCultureInfos.TryGetValue(objectType, out var numericCulture);

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

                if (stringMaxLength >= 0
                    && objectType == typeof(string)
                    && representation.Length > stringMaxLength)
                {
                    representation = representation.Substring(0, stringMaxLength);
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
                switch (nestingLevel)
                {
                    case 0 when excludingProperties.Contains(propertyName):
                        continue;
                    case 0 when propertiesSerializationFunctions.TryGetValue(propertyName, out serializationFunction):
                        sb.AppendLine($"{identation}{propertyInfo.Name} = {serializationFunction.DynamicInvoke(propertyInfo.GetValue(obj))}");
                        continue;
                }

                sb.Append($"{identation}{propertyInfo.Name} = {PrintToString(propertyInfo.GetValue(obj), nestingLevel + 1)}");
            }

            return sb.ToString();
        }
    }
}