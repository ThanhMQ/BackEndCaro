using AutoMapper;
using BackEndCaro.DTO;
using BackEndCaro.Models;

namespace BackEndCaro.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, GetUser>();
            CreateMap<RegisterUser, User>().BeforeMap((s, d) => d.Role = "User");
            CreateMap<UserRoom, GetUserRoom>();
            //CreateMap<PostUserRoom, UserRoom>();
        }
    }
}
