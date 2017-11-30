using System;
using System.Globalization;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ObjectPrinting.Tests
{
	[TestFixture]
	public class ObjectPrinterAcceptanceTests
	{
		[Test]
		public void Demo()
		{
			var person = new Person { Name = "Alexander", Age = 19, Height = 175.5 };

		    var printer = ObjectPrinter.For<Person>()
		        .ExcludingType<Guid>()                                    //1. Исключить из сериализации свойства определенного типа
		        .Printing<int>().Using(i => (i + 1).ToString())           //2. Указать альтернативный способ сериализации для определенного типа		        
		        .Printing<double>().Using(CultureInfo.InstalledUICulture) //3. Для числовых типов указать культуру
				//TODO RV(atolstov) плохо просить ввести имя свойства. Реализуй вот такой синтаксис (почитай про Expression):
			    //.PrintingProperty<string>(o => o.Name, p => p + " I")
				.PrintingProperty<string>("Name", p => p + " I")          //4. Настроить сериализацию конкретного свойства
		        .Printing<string>().TrimmedToLength(4)                    //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
		        .ExcludingProperty("Age");                                //6. Исключить из сериализации конкретного свойства

            var s1 = printer.PrintToString(person);
            TestContext.WriteLine(s1);

		    //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
            var s2 = person.PrintToString();                
            TestContext.WriteLine($"{Environment.NewLine}{s2}");

		    //8. ...с конфигурированием
		    var s3 = person.PrintToString(s => s.ExcludingProperty("Age"));
		    TestContext.WriteLine($"{Environment.NewLine}{s3}");
        }
	}
}