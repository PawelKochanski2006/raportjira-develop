using AutoMapper;
using JiraRaporty.Application.Project;
using JiraRaporty.Domain.Entities;

namespace JiraRaporty.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProjectDetails, ProjectDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.key))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name));
        }
    }
}