using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using Orckestra.Composer.Website;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Orckestra.Composer.Website")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyProduct("Orckestra.Composer.Website")]
[assembly: AssemblyCulture("")]

// Load any HTTP modules dynamically
[assembly: PreApplicationStartMethod(typeof(StartupHandler), "Start")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("16f0fd39-78e7-466e-844b-5b35443a0ce6")]
