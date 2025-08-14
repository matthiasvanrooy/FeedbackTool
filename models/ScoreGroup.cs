namespace Feedbacktool.models;

public class ScoreGroup : ClassGroup
{ 
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }
    
    public ScoreGroup(int subjectId, Subject subject)
    {
        Id = subjectId;
        Name = subject.Name;
        Users = subject.Users;
        SubjectId = subjectId;
        Subject = subject;
    }

    public ScoreGroup()
    {
        
    }
}