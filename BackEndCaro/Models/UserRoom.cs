using BackEndCaro.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BackEndCaro.Models
{
    [Table("UserRooms")]
    public class UserRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public User User { get; set; }
        public Room Room { get; set; }
        

    }
}
