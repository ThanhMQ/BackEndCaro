using AutoMapper;
using BackEndCaro.DTO;
using BackEndCaro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEndCaro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UsersController(AppDbContext context, IConfiguration config, IMapper mapper)
        {

            _context = context;
            _config = config;
            _mapper = mapper;

        }
        [HttpPost("register")]
        [AllowAnonymous]
        public object Register(RegisterUser request)
        {
            var userDb = _context.Users.FirstOrDefault(x => x.UserName == request.UserName);

            if (userDb != null)
                return BadRequest("Username này đã được đăng ký!");

            if (request.Password != request.ConfirmPassword)
                return BadRequest("Mật khẩu xác nhận không đúng!");

            User user = _mapper.Map<User>(request);

            _context.Users.Add(user); // lưu vào biến user
            _context.SaveChanges();     // lưu vào CSDL
         
            return Ok("Đăng ký tài khoản thành công");
            //return Ok(_context.Users.ToListAsync());
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public object Login(LoginUser request)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == request.UserName);

            if (user == null)
                return BadRequest("Tài khoản này chưa được đăng ký!");

            if (user.Password != request.Password)
                return BadRequest("Sai mật khẩu!");

            if (user.Status == 1)
                return BadRequest("Tài khoản đã được đăng nhập rồi!");

            user.Status = 1;

            _context.Users.Update(user);
            _context.SaveChanges();


            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("userName", user.UserName),
                new Claim("name", user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var users = _context.Users.ToList();

            // gửi danh sách user đi
           // _hub.Clients.All.SendAsync("user-online", users);

            return Ok("Bearer "+new JwtSecurityTokenHandler().WriteToken(token));
        }

        [HttpPost("logout")]
        [Authorize]
        public object Logout(LoginUser request)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserName == request.UserName);

            if (user == null)
                return BadRequest("Tài khoản này chưa được đăng ký!");

            if (user.Password != request.Password)
                return BadRequest("Sai mật khẩu!");

            if (user.Status == 0)
                return BadRequest("Tài khoản đã được đăng xuất rồi!");

            user.Status = 0;

            _context.Users.Update(user);
            _context.SaveChanges();

            var users = _context.Users.ToList();

            // gửi danh sách user đi
            //_hub.Clients.All.SendAsync("user-online", users);

            return Ok("Đăng xuất thành công");

        }

        [HttpGet]
        [Authorize]
        public object GetUsers()
        {
            var users = _context.Users.ToList();
            var getUsers = users.Select(user => _mapper.Map<User, GetUser>(user));
            if (null == getUsers)
                return BadRequest("Không có danh sách người dùng!");

            return Ok(getUsers);
        }
        //public object GetUsers()
        //{
        //    var users = _context.Users.ToList();
        //    var getUsers=new List<GetUser>();
        //    foreach(var user in users)
        //    {
        //        var getUser=new GetUser();
        //        getUser.Id = user.Id;
        //        getUser.UserName = user.UserName;
        //        getUser.Name = user.Name;
        //        getUser.Status = user.Status;
        //        getUsers.Add(getUser);
        //    }

        //    if (null == getUsers || 0 == getUsers.Count)
        //        return BadRequest("Không có danh sách người dùng!");

        //    return Ok(getUsers);
        //}

        //public object GetUsers()
        //{
        //    var users = _context.Users.ToList();
        //    var getUsers = new List<GetUser>();
        //    foreach (var user in users)
        //    {
        //        GetUser getUser = _mapper.Map<GetUser>(user);
        //        getUsers.Add(getUser);
        //    }

        //    if (null == getUsers || 0 == getUsers.Count)
        //        return BadRequest("Không có danh sách người dùng!");

        //    return Ok(getUsers);
        //}

    }
}
