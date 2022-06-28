using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hubtel.eCommerce.Cart.Models.ApiModels
{
    public class NewUserApiModel
    {
        [Required, StringLength(maximumLength: int.MaxValue, MinimumLength = 4, ErrorMessage = "Username should be at least 4 characters")]
        [DefaultValue("Test")]
        public string UserName { get; set; }
        [Required, DefaultValue("Test Person")]
        public string FullName { get; set; }
        [Required, DefaultValue(""), StringLength(maximumLength: 16, MinimumLength = 4, ErrorMessage = "Password should be between 4 and 20")]
        public string Password { get; set; }
        [Required, StringLength(maximumLength: 10, MinimumLength = 10, ErrorMessage = "MSISDN/Phone Should be 10 digits!")]
        [DefaultValue("0123456789")]
        public string Msisdn { get; set; }
    }
}
