using System.ComponentModel.DataAnnotations;

namespace CourseLibrary.Entiies;

public class Author
{
    [Key]
    public Guid AuthorId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string MainCategory { get; set; }
    
    [Required]
    public DateTimeOffset  DateOfBirth { get; set; }
    
    public ICollection <Course>  Courses { get; set; } = new List<Course>();
    public Author(string firstName, string lastName, string mainCategory)
    { 
        FirstName = firstName;
        LastName = lastName;
        MainCategory = mainCategory;
    }
    
}