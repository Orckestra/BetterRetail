namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class GetOrderForGuestParam : GetOrderByNumberParam
    {
        /// <summary>
        /// The email used to identify the order owner.
        /// </summary>
        public string Email { get; set; }
    }
}
