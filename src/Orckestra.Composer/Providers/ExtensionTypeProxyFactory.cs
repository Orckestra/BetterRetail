using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Providers
{
    public class ExtensionTypeProxyFactory
    {
        private static ExtensionTypeProxyFactory _instance;
        private static readonly object SingletonLocker = new object();
        private readonly object _typeCreatorLocker = new object();

        /// <summary>
        ///     Dictionary of proxy types.
        ///     Key is the type of an extended view model (extends <see cref="IExtensionOf{T}"/>).
        ///     Value is the proxy type  (extending <see cref="ExtensionOf{TModel}"/> where T is the type of the extended view model) used to instanciate the extended view model.
        /// </summary>
        private readonly ConcurrentDictionary<Type, Type> _proxyTypes = new ConcurrentDictionary<Type, Type>();

        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;

        /// <summary>
        ///     <see cref="ExtensionTypeProxyFactory"/> is a singleton and should not be instanciated
        /// </summary>
        private ExtensionTypeProxyFactory() { }

        /// <summary>
        /// This method should be called only in unit tests. elsewhere use <see cref="Default"/> to get the singleton instead.
        /// </summary>
        /// <returns>A new instance of <see cref="ExtensionTypeProxyFactory"/></returns>
        public static ExtensionTypeProxyFactory Create()
        {
            return new ExtensionTypeProxyFactory();
        }

        /// <summary>
        ///     Gets the current instance of <see cref="ExtensionTypeProxyFactory" />
        /// </summary>
        public static ExtensionTypeProxyFactory Default
        {
            get
            {
                if (_instance == null)
                {
                    lock (SingletonLocker)
                    {
                        if (_instance == null)
                        {
                            _instance = new ExtensionTypeProxyFactory();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        ///     Creates a proxy type <see cref="ExtensionOf{T}" />, where T is the type <paramref name="extensionType" /> is
        ///     extending.
        /// </summary>
        /// <param name="extensionType">An extension type of a view model.</param>
        private Type BuildProxyType(Type extensionType)
        {
            Type proxyType;

            lock (_typeCreatorLocker)
            {
                if (_proxyTypes.ContainsKey(extensionType))
                {
                    proxyType = _proxyTypes[extensionType];
                }
                else
                {
                    ValidateType(extensionType);
                    EnsureAssembly();

                    var proxyTypeBuilder = CreateProxyTypeBuilder(extensionType);
                    DefineProxyTypeConstructor(proxyTypeBuilder);
                    DefineProxyTypeProperties(proxyTypeBuilder, extensionType);
                    proxyType = proxyTypeBuilder.CreateType();
                }
            }

            return proxyType;
        }

        /// <summary>
        ///     Creates a new instance of a class implementing the interface <typeparamref name="T" /> which should extend the type
        ///     of the <paramref name="viewModel" />.
        ///     No validation is done to make sure <typeparamref name="T" /> actually extends <paramref name="viewModel" />; i.e.,
        ///     that T really extends from <see cref="IExtensionOf{TViewModel}" />, where TViewModel is the type of
        ///     <paramref name="viewModel" />.
        /// </summary>
        /// <typeparam name="T">
        ///     Type extending <see cref="IExtensionOf{TViewModel}" /> which the new instance must
        ///     implement, where TViewModel should be the type of <paramref name="viewModel" />.
        /// </typeparam>
        /// <param name="viewModel">View model which the new instance should extend.</param>
        public T Create<T>(IBaseViewModel viewModel)
        {
            if (viewModel == null) { throw new ArgumentNullException(nameof(viewModel)); }

            var extensionType = typeof(T);
            Type proxyType = _proxyTypes.GetOrAdd(extensionType, BuildProxyType);

            var proxy = (T)Activator.CreateInstance(proxyType);
            ((IExtension)proxy).SetBaseViewModel(viewModel);
            return proxy;
        }

        /// <summary>
        ///     Creates a <see cref="TypeBuilder" /> to create a type of <see cref="ExtensionOf{T}" />, where T is
        ///     <paramref name="extensionType" />.
        /// </summary>
        /// <param name="extensionType">Type extending <see cref="IExtensionOf{T}" /></param>
        private TypeBuilder CreateProxyTypeBuilder(Type extensionType)
        {
            var extendedType = GetExtendedType(extensionType);

            var proxyTypeBuilder = _moduleBuilder.DefineType(
                GetProxyTypeName(extensionType),
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                typeof(ExtensionOf<>).MakeGenericType(extendedType),
                new[] { extensionType });

            return proxyTypeBuilder;
        }

        /// <summary>
        ///     Defines a default constructor to the <paramref name="typeBuilder" />.
        /// </summary>
        /// <param name="typeBuilder">
        ///     A <see cref="TypeBuilder" /> to create a type of <see cref="ExtensionOf{T}" />, where T is
        ///     the type of the extended view model.
        /// </param>
        private void DefineProxyTypeConstructor(TypeBuilder typeBuilder)
        {
            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                Type.EmptyTypes);

            //Define the reflection ConstructorInfor for System.Object
            var conObj = typeBuilder.BaseType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, new ParameterModifier[0]);

            //call constructor of base object
            var il = constructor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, conObj);
            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        ///     Defines the same properties as in the extended type <paramref name="extensionType" /> to the
        ///     <paramref name="proxyTypeBuilder" />.
        /// </summary>
        /// <param name="proxyTypeBuilder">
        ///     A <see cref="TypeBuilder" /> to create a type of <see cref="ExtensionOf{T}" />, where T
        ///     is the type of view model extended by <paramref name="extensionType" />.
        /// </param>
        /// <param name="extensionType">Type extending <see cref="IExtensionOf{T}" /></param>
        private void DefineProxyTypeProperties(TypeBuilder proxyTypeBuilder, Type extensionType)
        {
            var extendedType = GetExtendedType(extensionType);

            var propertyMethodAttributes =
                MethodAttributes.Public |
                MethodAttributes.ReuseSlot |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig;

            foreach (var property in extensionType.GetProperties())
            {
                var propertyBuilder = proxyTypeBuilder.DefineProperty(property.Name, PropertyAttributes.HasDefault,
                    property.PropertyType, null);
                DefineProxyTypePropertyGetter(proxyTypeBuilder, property, propertyMethodAttributes, extendedType,
                    propertyBuilder);
                DefinesProxyTypePropertySetter(proxyTypeBuilder, property, propertyMethodAttributes, extendedType,
                    propertyBuilder);
            }
        }

        /// <summary>
        ///     Defines a getter for a specific property of the proxy type that will get the value from the property bag instead of
        ///     a member of the entity
        /// </summary>
        /// <param name="typeBuilder"><see cref="TypeBuilder" /> to create a view model proxy type</param>
        /// <param name="property">Info of the property to define in the type builder</param>
        /// <param name="attr">Attributes to set to the getter method</param>
        /// <param name="extendedType">Type of the extented view model</param>
        /// <param name="propBuilder">
        ///     <see cref="PropertyBuilder" /> to define the property to the <paramref name="typeBuilder" />
        /// </param>
        private void DefineProxyTypePropertyGetter(TypeBuilder typeBuilder, PropertyInfo property, MethodAttributes attr,
            Type extendedType,
            PropertyBuilder propBuilder)
        {
            var getPropertyMethod =
                typeBuilder.DefineMethod("get_" + property.Name,
                    attr,
                    property.PropertyType,
                    Type.EmptyTypes);

            var getIL = getPropertyMethod.GetILGenerator();

            getIL.Emit(OpCodes.Nop);
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldstr, property.Name);
            getIL.Emit(OpCodes.Call, GetGetValueMethod(extendedType, property.PropertyType));
            getIL.Emit(OpCodes.Ret);

            propBuilder.SetGetMethod(getPropertyMethod);
            typeBuilder.DefineMethodOverride(propBuilder.GetMethod, property.GetMethod);
        }

        /// <summary>
        ///     Defines a setter for a specific property of the proxy type that will set the value to the property bag instead of a
        ///     member of the entity
        /// </summary>
        /// <param name="typeBuilder">
        ///     A <see cref="TypeBuilder" /> to create a type of <see cref="ExtensionOf{T}" />, where T is
        ///     the <paramref name="extendedType" /> of the extended view model.
        /// </param>
        /// <param name="property">Info of the property to define in the type builder</param>
        /// <param name="attr">Attributes to set to the setter method</param>
        /// <param name="extendedType">Type of the extented view model</param>
        /// <param name="propBuilder">
        ///     <see cref="PropertyBuilder" /> to define the property to the <paramref name="typeBuilder" />
        /// </param>
        private void DefinesProxyTypePropertySetter(TypeBuilder typeBuilder, PropertyInfo property,
            MethodAttributes attr,
            Type extendedType, PropertyBuilder propBuilder)
        {
            var setPropertyMethod =
                typeBuilder.DefineMethod("set_" + property.Name,
                    attr,
                    null,
                    new[] { property.PropertyType });

            var setIL = setPropertyMethod.GetILGenerator();

            setIL.Emit(OpCodes.Nop);
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Ldstr, property.Name);
            setIL.Emit(OpCodes.Call, GetSetValueMethod(extendedType, property.PropertyType));
            setIL.Emit(OpCodes.Nop);
            setIL.Emit(OpCodes.Ret);

            propBuilder.SetSetMethod(setPropertyMethod);
            typeBuilder.DefineMethodOverride(propBuilder.SetMethod, property.SetMethod);
        }

        /// <summary>
        ///     Ensures a dynamic proxy assembly is defined as well as a module builder within that assembly. The proxy type will
        ///     be used to define the proxy type.
        ///     Sets the private members <see cref="_assemblyBuilder" /> and <see cref="_moduleBuilder" /> accordingly.
        /// </summary>
        private void EnsureAssembly()
        {
            if (_assemblyBuilder == null)
            {
                lock (_proxyTypes)
                {
                    if (_assemblyBuilder == null)
                    {
                        var assemblyName = new AssemblyName { Name = "DynamicProxy" };
                        var thisDomain = Thread.GetDomain();
                        _assemblyBuilder = thisDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                        _moduleBuilder = _assemblyBuilder.DefineDynamicModule(_assemblyBuilder.GetName().Name, false);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the type of view model <paramref name="extensionType" /> is extending.
        /// </summary>
        /// <param name="extensionType">Type extending <see cref="IExtensionOf{T}" />, where T is the type to return</param>
        private Type GetExtendedType(Type extensionType)
        {
            var extensionOfType = extensionType.GetInterfaces()
                .Single(t => t.IsGenericType &&
                             typeof(IExtensionOf<>).IsAssignableFrom(t.GetGenericTypeDefinition()));

            var extendedType = extensionOfType.GetGenericArguments()[0];
            return extendedType;
        }

        /// <summary>
        ///     Get the <see cref="ExtensionOf{extendedType}.GetValue{propertyType}(string)" /> method
        /// </summary>
        private MethodInfo GetGetValueMethod(Type extendedType, Type propertyType)
        {
            var methodInfo = typeof(ExtensionOf<>)
                .MakeGenericType(extendedType)
                .GetMethod("GetValue", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(propertyType);
            return methodInfo;
        }

        /// <summary>
        ///     Get the name of the proxy type for the given <paramref name="extensionType" />.
        /// </summary>
        /// <param name="extensionType">Type extending <see cref="IExtensionOf{T}" /></param>
        private string GetProxyTypeName(Type extensionType)
        {
            return string.Concat(extensionType.Name, extensionType.GUID.ToString("N"), "_Proxy");
        }

        /// <summary>
        ///     Get the <see cref="ExtensionOf{extendedType}.SetValue{propertyType}(propertyType, string)" /> method
        /// </summary>
        private MethodInfo GetSetValueMethod(Type extendedType, Type propertyType)
        {
            var methodInfo = typeof(ExtensionOf<>)
                .MakeGenericType(extendedType)
                .GetMethod("SetValue", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(propertyType);
            return methodInfo;
        }

        /// <summary>
        ///     Validates that the given <paramref name="extensionType" /> is an interface,
        ///     extends <see cref="IExtensionOf{T}" /> and all its properties have a getter and setter and exposes no method
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="extensionType" /> does not meet the criteria</exception>
        private void ValidateType(Type extensionType)
        {
            if (extensionType == null) { throw new ArgumentNullException(nameof(extensionType)); }

            ValidateTypeIsInterface(extensionType);
            ValidateTypeExtendsIExtensionOf(extensionType);
            ValidateTypePropertiesAccessors(extensionType);
            ValidateTypeHasNoMethod(extensionType);
        }

        /// <summary>
        ///     Validates that <paramref name="type" /> extends from <see cref="IExtensionOf{T}" />
        /// </summary>
        private void ValidateTypeExtendsIExtensionOf(Type type)
        {
            var exensionType = typeof(IExtensionOf<>);

            var typeInterfaces = type.GetInterfaces();
            var extensionInterfacesCount = 0;
            Type extensionInterface = null;

            foreach (var t in typeInterfaces)
            {
                if (t.IsGenericType && exensionType.IsAssignableFrom(t.GetGenericTypeDefinition()))
                {
                    if (extensionInterfacesCount == 0)
                    {
                        extensionInterface = t;
                    }

                    extensionInterfacesCount++;

                    if (extensionInterfacesCount > 1)
                    {
                        break;
                    }         
                }
            }

            if (extensionInterfacesCount > 1)
            {
                throw new InvalidOperationException(string.Format("The type {0} cannot extend more than once {1}",
                    type.Name, exensionType.Name));
            }
            if (extensionInterfacesCount < 1)
            {
                throw new InvalidOperationException(string.Format("The type '{0}' does not extend '{1}'", type.Name,
                    exensionType.Name));
            }

            // Ensure type extends a view model
            var extendedType = extensionInterface.GetGenericArguments()[0];
            if (extendedType == null)
            {
                throw new InvalidOperationException(string.Format("The type '{0}' does not extend any view model",
                    type.Name));
            }
        }

        /// <summary>
        ///     Validates that <paramref name="type" /> does not define any method.
        /// </summary>
        private static void ValidateTypeHasNoMethod(Type type)
        {
            // Exclude accessor methods
            var hasMethods = type
                .GetMethods()
                .Any(
                    m =>
                        m.DeclaringType != null &&
                        m.DeclaringType.GetProperties().All(p => p.GetGetMethod() != m && p.GetSetMethod() != m));

            if (hasMethods)
            {
                throw new InvalidOperationException(string.Format("The type '{0}' has methods.", type.Name));
            }
        }

        /// <summary>
        ///     Validate that <paramref name="type" /> is an interface
        /// </summary>
        private void ValidateTypeIsInterface(Type type)
        {
            if (!type.IsInterface)
            {
                throw new InvalidOperationException(string.Format("The type '{0}' is not an interface.", type.Name));
            }
        }

        /// <summary>
        ///     Validates that all properties of <paramref name="type" /> have a getter and setter methods
        /// </summary>
        private static void ValidateTypePropertiesAccessors(Type type)
        {
            if (type.GetProperties().Any(p => !p.CanRead || !p.CanWrite))
            {
                throw new InvalidOperationException(
                    string.Format("The type '{0}' has properties without getter or setter.", type.Name));
            }
        }
    }
}