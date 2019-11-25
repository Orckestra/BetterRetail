using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using FluentAssertions.Common;
using NUnit.Framework;

namespace Orckestra.Composer.Cart.Tests
{
    [TestFixture]
    public class CartConfigurationClassDef
    {
        private Type _inspectedType;
        private List<Type> _nestedTypes = new List<Type>();

        [SetUp]
        public void SetUp()
        {
            _inspectedType = typeof(CartConfiguration);

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
        public void All_Public_Fields_Should_Not_Be_Const_Unless_they_are_ignored()
        {
            _nestedTypes.SelectMany(t => t.GetFields())
                        .Where(f => f.IsLiteral)
                        .Where(f => !f.HasAttribute<IgnoreCheckAttribute>())
                        .Should()
                        .BeEmpty("This class is used for configuration, the custom implementation should be able to modify");
        }

        [Test]
        public void All_Public_Fields_Should_Not_Be_Readonly()
        {
            _nestedTypes.SelectMany(t => t.GetFields())
                        .Where(f => f.IsInitOnly)
                        .Where(f => !f.HasAttribute<IgnoreCheckAttribute>())
                        .Should()
                        .BeEmpty("This class is used for configuration, the custom implementation should be able to modify");
        }
    }
}
