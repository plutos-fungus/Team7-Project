using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurfsUp.Models
{
    public class Surfboard
    {
        public enum BoardTypes { shortboard, longboard, SUP, fish };

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
    }

    //public class SurfDBContext : DbContext
    //{
    //    public SurfDBContext()
    //    {}
    //    public DbSet<Surfboard> surfboards { get; set; }
    //}
}