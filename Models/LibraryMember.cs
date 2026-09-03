namespace LibraryManagementMVC.Models;

public class LibraryMember
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string StudentId { get; set; } = string.Empty;

    public string Course { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}