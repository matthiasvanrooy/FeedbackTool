using AutoMapper;
using Feedbacktool.DTOs.ClassGroupDTOs;
using Feedbacktool.DTOs.ExerciseDTOs;
using Feedbacktool.DTOs.ExerciseItemDTOs;
using Feedbacktool.DTOs.ExerciseItemResultDTOs;
using Feedbacktool.DTOs.FeedbackRuleDTOs;
using Feedbacktool.DTOs.ScoreGroupDTOs;
using Feedbacktool.DTOs.ScoreRecordDTOs;
using Feedbacktool.DTOs.SubjectDTOs;
using Feedbacktool.DTOs.UserDTOs;

namespace Feedbacktool.Api.AutoMapper;

using Models;

//No licence! What if I need one? Oh no!
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<ScoreGroup, ScoreGroupDto>();
        CreateMap<ClassGroup, ClassGroupDto>();
        CreateMap<Subject, SubjectDto>();
        CreateMap<Exercise, ExerciseDto>();
        CreateMap<ExerciseItem, ExerciseItemDto>();
        CreateMap<ScoreRecord, ScoreRecordDto>();
        CreateMap<ExerciseItemResult, ExerciseItemResultDto>();
        CreateMap<Exercise, SimpleExerciseDto>();

        CreateMap<FeedbackRule, FeedbackRuleDto>()
            .ForMember(dest => dest.SuggestedExercises, opt => opt.MapFrom(src => src.SuggestedExercises));
    }
}
