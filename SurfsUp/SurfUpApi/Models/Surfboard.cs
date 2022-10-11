using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SurfUpApi.Models
{
    public class Surfboard
    {
        public enum BoardTypes { shortboard, funboard, longboard, SUP, fish };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        //[DataType(DataType.Text)]
        [StringLength(255)]
        public string Name { get; set; }
        public BoardTypes BoardType { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Thickness { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        [StringLength(50)]
        public string? EquipmentTypes { get; set; }
        [StringLength(50)]
        public string? Image { get; set; }
        public bool IsRented { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public IdentityUser? IdentityUser { get; set; }
    }
}
