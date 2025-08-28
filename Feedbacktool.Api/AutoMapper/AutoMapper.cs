using AutoMapper;
using Feedbacktool.DTOs.SubjectDTOs;

namespace Feedbacktool.Api.AutoMapper;

using Models;
using DTOs;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<ScoreGroup, ScoreGroupDto>();
        CreateMap<ClassGroup, ClassGroupDto>();
        CreateMap<Subject, SubjectDto>();
    }
}
