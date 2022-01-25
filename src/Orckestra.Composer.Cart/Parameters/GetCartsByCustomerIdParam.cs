namespace Orckestra.Composer.Cart.Parameters
{
    public class GetCartsByCustomerIdParam : BaseCartParam
    {
        /// <summary>
        /// A value indicating whether to include carts found in child scopes
        /// Optional 
        /// false by default
        /// </summary>
        public bool IncludeChildScopes { get; set; }
    }
}
