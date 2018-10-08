using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;

namespace Orckestra.Composer.Tests
{
    [TestFixture]
    public class ComposerConfigurationClassDef
    {
        private Type _inspectedType;
        private List<Type> _nestedTypes = new List<Type>();
        private Type _counterExampleType;

        [SetUp]
        public void SetUp()
        {
            _inspectedType = typeof (ComposerConfiguration);

            Stack<Type>  toVisit = new Stack<Type>();
            toVisit.Push(_inspectedType);

            while (toVisit.Any())
            {
                Type head = toVisit.Pop();

                foreach (Type nested in head.GetNestedTypes())
                {
                    toVisit.Push(nested);
                }

                _nestedTypes.Add(head);
            }

            _counterExampleType = typeof(CounterExampleConfig);
        }

        
        [Test]
        public void Class_Name_Should_End_with_Configuration()
        {
            _inspectedType.FullName
                          .Should()
                          .EndWith("Configuration", "By convension, all Configuration Classes must end with the word \"Configuration\"");
        }

        [Test]
        public void Class_Must_Be_Public()
        {
            _inspectedType.IsPublic
                          .Should()
                          .BeTrue("Consumer must be able to read/write the configurations");
        }

        [Test]
        public void Class_Must_Be_Static()
        {
            //Actually "public class" compiles into "public sealed abstract" so that's what we are checking.
            bool isStatic = (_inspectedType.IsAbstract && _inspectedType.IsSealed);

            isStatic.Should()
                    .BeTrue("Consumers are assumed to access these configuration through static accessers only");
        }

        [Test]
        public void All_Properties_Should_Be_Static()
        {
            _nestedTypes.SelectMany(t => t.GetProperties())
                        .Select(p => p.GetMethod ?? p.SetMethod)
                        .Where(m => m != null)
                        .Where(m => !m.IsStatic)
                        .Should()
                        .BeEmpty("Consumers are assumed to access these configuration through static accessers only");
        }

        [Test]
        public void All_Properties_Should_Have_A_Public_Getter()
        {
            _nestedTypes.SelectMany(t => t.GetProperties())
                        .Select(p => p.GetGetMethod(true))
                        .Where(m => m == null || !m.IsPublic)
                        .Should()
                        .BeEmpty("Consumers should be allowed to access all configurations to match there business rules");
        }

        [Test]
        public void All_Properties_Should_Have_A_Public_Setter()
        {
            _nestedTypes.SelectMany(t => t.GetProperties())
                        .Select(p => p.GetSetMethod(true))
                        .Where(m => m == null || !m.IsPublic)
                        .Should()
                        .BeEmpty("Consumers should be allowed to modify all configurations to match there business rules");
        }

        [Test]
        public void Properties_Should_Not_Have_Index_Accesser()
        {
            _nestedTypes.SelectMany(t => t.GetProperties())
                        .Select(p => p.GetMethod)
                        .Where(m => m != null)
                        .Where(m => m.GetParameters().Any())
                        .Should()
                        .BeEmpty("By design properties with index accesser [] are prohibed to avoid confusion and preserve maintainability");
        }

        [Test]
        public void All_Properties_Must_Have_A_NonNull_Default_Value()
        {
            object[] noIndexes = new object[0];

            _nestedTypes.SelectMany(t => t.GetProperties())
                        .Select(p => p.GetMethod)
                        .Where(m => m != null)
                        .Where(m => m.IsStatic)
                        .Where(m => !m.GetParameters().Any())
                        .Where(m => m.Invoke(null, noIndexes) == null)
                        .Should()
                        .BeEmpty("Valid default values must be provided when consumer don't have any specific business rules");
        }

        [Test]
        public void All_Properties_Getters_Should_Not_Throw_Exceptions()
        {
            //Arrange
            List<MethodInfo> getters = _nestedTypes.SelectMany(t => t.GetProperties())
                                                   .Select(p => p.GetMethod)
                                                   .Where(m => m != null)
                                                   .Where(m => m.IsStatic)
                                                   .Where(m => !m.GetParameters().Any())
                                                   .ToList();
            //Act & Assert
            foreach (MethodInfo getter in getters)
            {
                object[] args = new object[0];
                MethodInfo method = getter;

                Action action = () => method.Invoke(null, args);
                action.ShouldNotThrow("Absolutly no exceptions should be thrown");
            }
        }

        [Test]
        public void All_Properties_Setters_Should_Not_Throw_Exceptions()
        {
            //Arrange
            List<MethodInfo> setters = _nestedTypes.SelectMany(t => t.GetProperties())
                                                   .Select(p => p.SetMethod)
                                                   .Where(m => m != null)
                                                   .Where(m => m.IsStatic)
                                                   .Where(m => !m.GetParameters().Any())
                                                   .ToList();

            //Act & Assert
            foreach (MethodInfo setter in setters)
            {
                object[] args = new object[0];
                MethodInfo method = setter;

                Action action = () => method.Invoke(null, args);
                action.ShouldNotThrow("Absolutly no exceptions should be thrown");
            }
        }

        [Test]
        public void All_Public_Fields_Should_Not_Be_Const()
        {
            _nestedTypes.SelectMany(t => t.GetFields())
                        .Where(f => f.IsLiteral)
                        .Should()
                        .BeEmpty("This class is used for configuration, the custom implementation should be able to modify");
        }

        [Test]
        public void All_Public_Fields_Should_Not_Be_Readonly()
        {
            _nestedTypes.SelectMany(t => t.GetFields())
                        .Where(f => f.IsInitOnly)
                        .Should()
                        .BeEmpty("This class is used for configuration, the custom implementation should be able to modify");
        }

        #region CounterExample
        [Test]
        public void Poorly_Named_Class_Should_Be_Detected()
        {
            _counterExampleType.FullName
                               .Should()
                               .NotEndWith("Configuration", "This is the opposite unit test used to detect poorly named Configuration classes");
        }

        [Test]
        public void NonPublic_Class_Should_Be_Detected()
        {
            _counterExampleType.IsPublic
                               .Should()
                               .BeFalse("This is the opposite unit test used to detect non public class");
        }

        [Test]
        public void NonStatic_Class_Should_be_Detected()
        {
            //Actually "public class" compiles into "public sealed abstract" so that's what we are checking.
            bool isStatic = (_counterExampleType.IsAbstract && _counterExampleType.IsSealed);

            isStatic.Should()
                    .BeFalse("This is the opposite unit test used to detect non public class");
        }

        [Test]
        public void NonStatic_Properties_Should_Be_Detected()
        {
            _counterExampleType.GetProperties()
                               .Select(p => p.GetMethod ?? p.SetMethod)
                               .Where(m => m != null)
                               .Where(m => !m.IsStatic)
                               .Should()
                               .NotBeNullOrEmpty("This is the opposite unit test used to detect non static properties");
        }

        [Test]
        public void NonPublic_Getters_Should_Be_Detected()
        {
            _counterExampleType.GetProperties()
                               .Select(p => p.GetGetMethod(true))
                               .Where(m => m == null || !m.IsPublic)
                               .Should()
                               .NotBeNullOrEmpty("This is the opposite unit test used to detect non public getters");
        }

        [Test]
        public void NonPublic_Setters_Should_Be_Detected()
        {
            _counterExampleType.GetProperties()
                               .Select(p => p.GetSetMethod(true))
                               .Where(m => m == null || !m.IsPublic)
                               .Should()
                               .NotBeNullOrEmpty("This is the opposite unit test used to detect non public setters");
        }

        [Test]
        public void Index_Accesser_Should_Be_Detected()
        {
            _counterExampleType.GetProperties()
                               .Select(p => p.GetMethod)
                               .Where(m => m != null)
                               .Where(m => m.GetParameters().Any())
                               .Should()
                               .NotBeNullOrEmpty("This is the opposite unit test used to detect index accesser [] which are prohibed by design");
        }

        [Test]
        public void Null_Default_Values_Should_Be_Detected()
        {
            object[] noIndexes = new object[0];

            _counterExampleType.GetProperties()
                               .Select(p => p.GetMethod)
                               .Where(m => m != null)
                               .Where(m => m.IsStatic)
                               .Where(m => !m.GetParameters().Any())
                               .Where(m => m.Invoke(null, noIndexes) == null)
                               .Should()
                               .NotBeNullOrEmpty("This is the opposite unit test used to detect properties returning null values which are prohibed by design");
        }

        [Test]
        public void Exceptions_In_Getters_Should_Be_Detected()
        {
            //Arrange
            object[] args = new object[0];
            MethodInfo getter = _counterExampleType.GetProperties()
                                                   .Where(p => p.Name == "ThrowsException")
                                                   .Select(p => p.GetMethod)
                                                   .First();
            //Act
            Action action = () => getter.Invoke(null, args);

            //Assert
            action.ShouldThrow<Exception>("This is the opposite unit test used to detect properties throwing exceptions");
        }

        [Test]
        public void Exceptions_In_Setters_Should_Be_Detected()
        {
            //Arrange
            object[] args = new object[0];
            MethodInfo setter = _counterExampleType.GetProperties()
                                                   .Where(p => p.Name == "ThrowsException")
                                                   .Select(p => p.SetMethod)
                                                   .First();
            //Act
            Action action = () => setter.Invoke(null, args);

            //Assert
            action.ShouldThrow<Exception>("This is the opposite unit test used to detect properties throwing exceptions");
        }

        [Test]
        public void Const_Fields_Should_Be_Detected()
        {
            //Arrange
            FieldInfo field = _counterExampleType.GetFields()
                                                 .First(m => m.Name == "ThisIsConst");

            //Act

            //Assert
            field.IsLiteral.Should().BeTrue("This is the opposite unit test used to detect const fields");
        }

        [Test]
        public void Readonly_Fields_Should_Be_Detected()
        {
            //Arrange
            FieldInfo field = _counterExampleType.GetFields()
                                                 .First(m => m.Name == "ThisIsReadonly");

            //Act

            //Assert
            field.IsInitOnly.Should().BeTrue("This is the opposite unit test used to detect readonly fields");
        }

        /// <summary>
        /// CounterExample, list of everything that is not expected in a Configuration class.
        /// </summary>
        internal class CounterExampleConfig
        {
            public const string ThisIsConst = "asdf";
            public static readonly string ThisIsReadonly = string.Empty;

            private static string _thisIsNotAProperty = string.Empty;

            public static string GetterOnly { get { return _thisIsNotAProperty; } }
            public static string SetterOnly { set { _thisIsNotAProperty = value; } }

            internal static string Internal { get; set; }

            public static string InternalSetter { get { return _thisIsNotAProperty; } internal set { _thisIsNotAProperty = value; } }
            public static string InternalGetter { internal get { return _thisIsNotAProperty; } set { _thisIsNotAProperty = value; } }

            public static string PrivateSetter { get { return _thisIsNotAProperty; } private set { _thisIsNotAProperty = value; } }
            public static string PrivateGetter { private get { return _thisIsNotAProperty; } set { _thisIsNotAProperty = value; } }

            public string NotStatic { get { return _thisIsNotAProperty; } set { _thisIsNotAProperty = value; } }

            public static string ReturnsNull { get { return null; } set { _thisIsNotAProperty = value; } }

            public string this[int index] { get { return _thisIsNotAProperty; } set { _thisIsNotAProperty = value; } }

            public static string ThrowsException
            {
                get { throw new Exception("This is a very bad practice"); }
                set { throw new Exception("This is a very bad practice"); }
            }
        }
        #endregion
    }
}
