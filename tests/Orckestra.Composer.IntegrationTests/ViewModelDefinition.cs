using System;
using System.Text;
using Orckestra.Composer.ViewModels;

namespace Orckestra.Composer.Tests.IntegrationTests
{
    public sealed class ViewModelDefinition
    {
        private static readonly string BaseViewModelName = typeof (BaseViewModel).FullName;

        public Type TargettedType { get; private set; }

        public bool IsSealed { get; set; }

        public bool InheritsBase { get; set; }

        public ViewModelDefinition(Type targettedType)
        {
            TargettedType = targettedType;
        }

        public bool IsValid()
        {
            return InheritsBase;
        }

        public override string ToString()
        {
            if (IsValid())
            {
                return string.Format("Type '{0}' is valid.", TargettedType.FullName);
            }

            var sBuilder = new StringBuilder();
            sBuilder.AppendFormat("Type '{0}' is invalid ", TargettedType.FullName);

            if (!IsSealed)
            {
                sBuilder.AppendFormat("because it is not SEALED");
            }
            if (!IsSealed && !InheritsBase)
            {
                sBuilder.AppendFormat(" and ");
            }
            if (!InheritsBase)
            {
                sBuilder.AppendFormat("because does not extend '{0}' directly", BaseViewModelName);
            }

            sBuilder.Append(".");
            return sBuilder.ToString();
        }
    }
}
