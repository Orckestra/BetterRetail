using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Orckestra.Composer.Kernel;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Orckestra.Composer.MyAccount")]
[assembly: AssemblyDescription("Reusable components specific for MyAccount-related concerns in an E-Commerce site")]
[assembly: AssemblyProduct("Orckestra.Composer.MyAccount")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5c3361da-b138-495d-b684-23b33df414c8")]

// Expose internal classes to Test project
[assembly: InternalsVisibleTo("Orckestra.Composer.MyAccount.Tests")]
// Expose internal classes to Moq
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

[assembly: ComposerAssemblyWeight(0)]
