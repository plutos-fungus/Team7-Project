using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurfsUp.Models
{
    public class Surfboard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        //[DataType(DataType.Text)]
        public string Name { get; set; }
        public string BoardType { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Thickness { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public string? EquipmentTypes { get; set; }
        public string? Image { get; set; }
    }
}