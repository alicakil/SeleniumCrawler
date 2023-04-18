
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dashboard.Models
{
    public class Account : BaseEntity
    {
        [StringLength(200)]
        [Required]
        public string? Name { get; set; }             

        [Required]
        [StringLength(200)]
        [MinLength(6)]
        public string? Password { get; set; }

        [Required]
        [StringLength(200)]
        [RegularExpression(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", ErrorMessage = "Geçerli bir eposta adresi giriniz!")]
        public string? Email { get; set; }

    }
}
