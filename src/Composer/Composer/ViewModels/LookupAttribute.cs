using System;
using Orckestra.Composer.Enums;

namespace Orckestra.Composer.ViewModels
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LookupAttribute : Attribute
    {
        public LookupType LookupType { get; private set; }
        public string LookupName { get; private set; }
	    public string Delimiter { get; private set; }

        public LookupAttribute(LookupType lookupType, string lookupName, string delimiter=", ")
        {
            Delimiter = delimiter;
            LookupType = lookupType;
            LookupName = lookupName;
        }

    }
}