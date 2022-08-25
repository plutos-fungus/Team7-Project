using System.ComponentModel.DataAnnotations;

namespace SurfsUp.Models
{
    public class Surfboard
    {
        public int ID { get; set; }
        [DataType(DataType.Text)]
        public string? Name { get; set; }
        public Type? BoardType { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Thickness { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public List<Equipment>? EquipmentTypes { get; set; }
        public string? Image { get; set; }
    }
}