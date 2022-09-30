using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurfUpApi.Models
{
    public class Rental
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
        public int SurfboardID { get; set; }
    }
}
