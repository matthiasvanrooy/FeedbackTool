﻿namespace Feedbacktool.Models;

public class ScoreRecord
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
    public int Value { get; set; } // 0-100 percentage
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
