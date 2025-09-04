namespace Feedbacktool.DTOs.ScoreGroupDTOs;

public class UpdateScoreGroupRequest
{
    public string? Name { get; set; } = string.Empty;
    public int? SubjectId { get; set; }
}