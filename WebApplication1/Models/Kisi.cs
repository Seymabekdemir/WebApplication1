using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Kisi
    {
        [Key]
        public int Id { get; set; }

        public required string Ad { get; set; }  // 🔥 Buraya dikkat
        public required string Soyad { get; set; }
        public required string Email { get; set; }
        public required string Departman { get; set; }
        public required string IsTanimi { get; set; }

        public DateTime DogumTarihi { get; set; }
    }

}