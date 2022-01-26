namespace Orckestra.Composer.Cart.Parameters.Order
{
    public class GetOrderForGuestParam : GetOrderParam
    {
        /// <summary>
        /// The email used to identify the order owner.
        /// </summary>
        public string Email { get; set; }
    }
}
