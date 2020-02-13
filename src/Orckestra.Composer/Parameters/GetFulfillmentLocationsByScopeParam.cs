namespace Orckestra.Composer.Parameters
{
    public class GetFulfillmentLocationsByScopeParam
    {
        public string Scope { get; set; }

        public bool IncludeChildScopes { get; set; }

        public bool IncludeSchedules { get; set; }
    }
}
