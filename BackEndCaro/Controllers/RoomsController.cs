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
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace BackEndCaro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        public static int count = 1;
        private readonly AppDbContext _context;
   
        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("number")] //tạo nhiều phòng 1 lúc
        [Authorize(Roles = "Administrator")]
        public object CreateRooms(int number) 
        {
            for(int i = count; i < count+number; i++)
            {
                var room=new Room();
                room.Name = "Phòng " + i;
                room.Status = 0; //Mới tạo nên cho phòng rỗng
                _context.Rooms.Add(room);
            }
            _context.SaveChanges();
            count += number;
            return Ok("Tạo "+ number +" phòng thành công");
        }

        // POST: api/Rooms
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Room>> PostRoom(string Name)
        {
            var room = new Room();
            room.Name = Name;
            room.Status = 0;
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return Ok("Thêm phòng thành công");
        }

        // GET: /api/rooms
        // get all room
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            return await _context.Rooms.ToListAsync();
        }

        // GET: api/Rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        // DELETE: /api/rooms
        // delete all room
        [HttpDelete]
        public async Task<IActionResult> DeleteRoom()
        {
            foreach(var room in _context.Rooms)
            {
                _context.Rooms.Remove(room);
            }
            await _context.SaveChangesAsync();
            return Ok("Xoá toàn bộ phòng thành công");
        }

        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok("Xoá phòng thành công");
        }

    }
}
