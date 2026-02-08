using Quiz.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public partial class User
{
    [Key]
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string AcademicID { get; set; } = null!;

    public int Section { get; set; }

    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public int YearId { get; set; }

    public int? Term { get; set; }

    [ForeignKey(nameof(YearId))]
    public virtual Year Year { get; set; } = null!;

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
