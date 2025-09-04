using AutoMapper;
using Feedbacktool.DTOs.ClassGroupDTOs;
using Feedbacktool.DTOs.ExerciseDTOs;
using Feedbacktool.DTOs.ScoreGroupDTOs;
using Feedbacktool.DTOs.SubjectDTOs;
using Feedbacktool.DTOs.UserDTOs;

namespace Feedbacktool.Api.AutoMapper;

using Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<ScoreGroup, ScoreGroupDto>();
        CreateMap<ClassGroup, ClassGroupDto>();
        CreateMap<Subject, SubjectDto>();
        CreateMap<Exercise, ExerciseDto>();
    }
}
