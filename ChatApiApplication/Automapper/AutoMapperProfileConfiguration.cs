using AutoMapper;
using ChatApiApplication.DTO;
using ChatApiApplication.Model;
namespace ChatApiApplication.Automapper
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {

            CreateMap<Messages, MessagesDTO>();
            CreateMap<Conversations, MessagesDTO>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId));
        }
    }
}
