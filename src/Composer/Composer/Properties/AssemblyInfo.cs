using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;
using Orckestra.Composer;
using Orckestra.Composer.Kernel;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Orckestra.Composer")]
[assembly: AssemblyDescription("Base components for all Composer-related projects")]
[assembly: AssemblyProduct("Orckestra.Composer")]
[assembly: AssemblyCulture("")]

// Load any HTTP modules dynamically
[assembly: PreApplicationStartMethod(typeof(PreApplicationStartCode), "Start")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("92327689-0839-40c3-b83a-ceb3cd121d23")]

// Expose internal classes to Test project
[assembly: InternalsVisibleTo("Orckestra.Composer.Tests")]

// Expose internal classes to Moq
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

[assembly: InternalsVisibleTo("Composer.Tests")]

[assembly: ComposerAssemblyWeight(0)]
