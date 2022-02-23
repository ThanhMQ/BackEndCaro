using BackEndCaro.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEndCaro.Models
{
    [Table("Rooms")]
    public class Room
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Status { get; set; } //0 là phòng rỗng, 1 là có người

        
    }
}
