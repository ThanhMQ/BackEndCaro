#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEndCaro;
using BackEndCaro.Models;
using BackEndCaro.DTO;
using AutoMapper;

namespace BackEndCaro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoomsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserRoomsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //public async Task<ActionResult<IEnumerable<UserRoom>>> GetUserRooms()
        //{
        //    return await _context.UserRooms.ToListAsync();
        //}

        // GET: api/UserRooms
        [HttpGet]
        public object GetUserRooms()
        {
            var userRooms = _context.UserRooms.ToList();
            var getUserRooms = userRooms.Select(userRoom => _mapper.Map<UserRoom, GetUserRoom>(userRoom));
            if (null == getUserRooms)
                return BadRequest("Tất cả các phòng đều đang trống!");
            return Ok(getUserRooms);
        }

        // api vào phòng (bản chất là thêm bản ghi vào talbe UserRoom)
        //đầu vào là đối tượng. Post/get: object; delete/put: param
        // POST: api/UserRooms
        //[HttpPost]
        //public async Task<ActionResult<UserRoom>> PostUserRoom(PostUserRoom request)
        //{
        //    var room = _context.Rooms.FirstOrDefault(e => e.Id == request.RoomId);
        //    if (room == null)
        //    {
        //        return BadRequest("Phòng không tồn tại");
        //    }

        //    var user = _context.Users.FirstOrDefault(e => e.Id == request.UserId);
        //    if (user == null)
        //    {
        //        return BadRequest("User không tồn tại");
        //    }

        //    var currentUserRoom = _context.UserRooms.FirstOrDefault(e => e.UserId == request.UserId);
        //    if ( currentUserRoom != null)
        //    {
        //        return BadRequest("User đang ở trong một phòng. Yêu cầu rời phòng trước");
        //    }

        //    var userRoom = _mapper.Map<UserRoom>(request);
        //    _context.UserRooms.Add(userRoom);

        //    room.Status = 1; //cập nhật rằng phòng có người
        //    _context.Rooms.Update(room);

        //    await _context.SaveChangesAsync();
        //    return Ok("Vào phòng thành công");
        //}
        [HttpPost("{roomId}/{userId}")]
        public object JoinRoom(int roomId, int userId)
        {
            var room = _context.Rooms.FirstOrDefault(e => e.Id == roomId);
            if (room == null)
            {
                return BadRequest("Phòng không tồn tại");
            }

            var user = _context.Users.FirstOrDefault(e => e.Id == userId);
            if (user == null)
            {
                return BadRequest("User không tồn tại");
            }

            var currentUserRoom = _context.UserRooms.FirstOrDefault(e => e.UserId == userId);
            if (currentUserRoom != null)
            {
                return BadRequest("User đang ở trong một phòng. Yêu cầu rời phòng trước");
            }

            var userRoom = new UserRoom();
            userRoom.RoomId= roomId;
            userRoom.UserId= userId;
            _context.UserRooms.Add(userRoom);

            room.Status = 1; //cập nhật rằng phòng có người
            _context.Rooms.Update(room);

            _context.SaveChangesAsync();
            return Ok("Vào phòng thành công");
        }
        // api rời phòng (bản chất là xoá bản ghi trong talbe UserRoom)
        // DELETE: api/UserRooms/userId/roomId
        [HttpDelete("{roomId}/{userId}")]
        public object DeleteUserRoom(int roomId, int userId)
        {
            var room = _context.Rooms.FirstOrDefault(e => e.Id == roomId);
            if (room == null)
            {
                return BadRequest("Phòng không tồn tại");
            }

            var user = _context.Users.FirstOrDefault(e => e.Id == userId);
            if (user == null)
            {
                return BadRequest("User không tồn tại");
            }
      
            var userRoom = _context.UserRooms.FirstOrDefault(e => e.UserId == userId && e.RoomId == roomId);
            if (userRoom == null)
            {
                return BadRequest("User không ở trong phòng này");
            }

            _context.UserRooms.Remove(userRoom);
            
            var userRooms = _context.UserRooms.Where(e => e.RoomId == roomId);

            if (userRooms.Count() == 0) //không còn bản ghi UserRoom nào ứng với RoomId đó -> phòng không còn ai 
            {
                room.Status = 0;
                _context.Rooms.Update(room);
            }
            _context.SaveChangesAsync();
            return Ok("Rời phòng thành công!");
        }

        // GET: api/UserRooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserRoom>> GetUserRoom(int id)
        {
            var userRoom = await _context.UserRooms.FindAsync(id);

            if (userRoom == null)
            {
                return NotFound();
            }

            return userRoom;
        }

        // DELETE: api/UserRooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserRoom(int id)
        {
            var userRoom = await _context.UserRooms.FindAsync(id);
            if (userRoom == null)
            {
                return NotFound();
            }

            _context.UserRooms.Remove(userRoom);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
