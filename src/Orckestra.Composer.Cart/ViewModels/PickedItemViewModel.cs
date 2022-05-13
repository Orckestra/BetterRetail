using Newtonsoft.Json;

namespace Orckestra.Composer.Cart.ViewModels
{
    /// <summary>
    /// Picking history, including substitutions and not available items
    /// </summary>
    public class PickedItemViewModel
    {
        [JsonProperty("productId")]
        public string ProductId { get; set; }

        [JsonProperty("variantId")]
        public string VariantId { get; set; }

        [JsonProperty("quantity")]
        public double Quantity { get; set; }

        [JsonProperty("productTitle")]
        public string ProductTitle { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("totalPrice")]
        public double TotalPrice { get; set; }

        [JsonProperty("isProofOfAgeRequired")]
        public bool IsProofOfAgeRequired { get; set; }

        [JsonProperty("pickingResult")]
        public string PickingResult { get; set; }

        [JsonProperty("displaySequence")]
        public int DisplaySequence { get; set; }

        [JsonProperty("displayQuantity")]
        public double DisplayQuantity { get; set; }

        [JsonProperty("displayPrice")]
        public decimal? DisplayPrice { get; set; }

        [JsonProperty("displayTotalPrice")]
        public decimal? DisplayTotalPrice { get; set; }
    }
}
