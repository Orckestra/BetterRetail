using Orckestra.Composer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Cart.ViewModels
{
    public class LightLineItemDetailViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Product LineItemId
        /// </summary>
        public string ProductId { get; set; }


        /// <summary>
        /// Image path of the product
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Fallback image to use when the ProductMainImage does a 404
        /// </summary>
        public string FallbackImageUrl { get; set; }

        /// <summary>
        /// The Url of the product page
        /// </summary>
        public string ProductUrl { get; set; }

        /// <summary>
        /// The display name of the product
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Display the VariantId cause we don't have access to the Variant object yet
        /// </summary>
        public string VariantId { get; set; }
             
        /// <summary>
        /// Quantity ordered of a product
        /// </summary>
        public double Quantity { get; set; }
        
        
        //public List<KeyVariantAttributes> KeyVariantAttributesList { get; set; }

        /// <summary>
        /// Determines if the LineItem is Valid.
        /// </summary>
        public bool? IsValid { get; set; }


        public LightLineItemDetailViewModel()
        {
        }
    }
}
