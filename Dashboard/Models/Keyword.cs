using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models
{
    public class Keyword : BaseModified
    {
        [StringLength(180)]
        [Required]
        public string Word { get; set; }

    }
}
