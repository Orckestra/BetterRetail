namespace Orckestra.Composer.Parameters
{
    public class SearchFilter
    {
        /// <summary>
        /// Gets or sets the search filter name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }


        /// <summary>
        /// Gets or sets the search filter value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the value indicating if the filter is imposed by the system of optionally added by the user
        /// </summary>
        public bool IsSystem { get; set; }

        public SearchFilter Clone()
        {
            return (SearchFilter)MemberwiseClone();
        }
    }
}
