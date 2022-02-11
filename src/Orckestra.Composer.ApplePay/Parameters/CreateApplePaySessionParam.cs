using System.ComponentModel.DataAnnotations;

namespace Orckestra.Composer.ApplePay.Parameters
{
    public class CreateApplePaySessionParam
    {
        [DataType(DataType.Url)]
        [Required]
        public string ValidationUrl { get; set; }
    }
}
