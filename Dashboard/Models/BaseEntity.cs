using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }

    public class BaseCreated : BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("Acount")]
        public int CreatedById { get; set; }
        public Account? CreatedBy { get; set; }
    }

    public class BaseModified : BaseCreated
    {
        public DateTime? ModifiedAt { get; set; }

        [ForeignKey("Acount")]
        public int? ModifiedById { get; set; }
        public Account? ModifiedBy { get; set; }
    }
}
