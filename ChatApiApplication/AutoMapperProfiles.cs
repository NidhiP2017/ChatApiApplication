using AutoMapper;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;

namespace ChatApiApplication
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {
            CreateMap<UsersDTO, Users>().ReverseMap();
        }
    }
}