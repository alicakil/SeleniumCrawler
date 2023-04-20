using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.Models
{
    public class Story : BaseModified
    {
        [StringLength(180)]
        [Required]
        [DisplayName("Story Title")]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }
        public string[] Keywords { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(500)]
        public string? CoverPhotoURL { get; set; }
        public double? CoverPhotoAspectRatio { get; set; }

        [StringLength(400)]
        public string? InstagramUrl { get; set; }
    }
}
