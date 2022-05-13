using System;
using System.Collections.Generic;

namespace Orckestra.Composer.Parameters
{
    public class GetCustomProfilesParam
    {
        public List<Guid> CustomProfileIds { get; set; }
        public string Scope { get; set; }
        public string EntityTypeName { get; set; }

        public GetCustomProfilesParam()
        {
            CustomProfileIds = new List<Guid>();
        }
    }
}
