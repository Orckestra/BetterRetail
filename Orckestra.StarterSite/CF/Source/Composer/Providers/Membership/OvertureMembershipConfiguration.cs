namespace Orckestra.Composer.Providers.Membership
{
    public static class OvertureMembershipConfiguration
    {
        private static string _defaultMembershipDomain = "composer";

        private static string _defaultFirstName = "OvertureConnect";
        private static string _defaultLastName = "OvertureConnect";

        /// <summary>
        /// This name must match the one in the file App_Config\Security\Domains.config
        /// </summary>
        public static string DefaultMembershipDomain
        {
            get { return _defaultMembershipDomain; }
            set { _defaultMembershipDomain = value; }
        }
        public static string DefaultFirstName
        {
            get { return _defaultFirstName; }
            set { _defaultFirstName = value; }
        }
        public static string DefaultLastName
        {
            get { return _defaultLastName; }
            set { _defaultLastName = value; }
        }
    }
}