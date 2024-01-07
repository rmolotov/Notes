namespace Notes.Core.Entities;

public class Note
{
    public Guid UserId { get; set; }
    public Guid Id { get; set; }

    public string Title { get; set; }
    public string Details { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime? EditedDate { get; set; }
}