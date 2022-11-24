using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurfsUp.Models
{
    public class Rental
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime EndDate { get; set; }
        [StringLength(255)]
        public string Email { get; set; }

        [ForeignKey("Surfboard")]
        public int SurfboardID { get; set; }
    }
}
