using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Models;

public partial class Subject
{
    [Key]
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = null!;
    public int YearId { get; set; }
    public int Term { get; set; }

    // السطر ده هو اللي هيصلح إيرور "ViewData" في الصورة 2
    [ForeignKey("YearId")]
    [InverseProperty("Subjects")]
    public virtual Year Year { get; set; } = null!;

    public virtual ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
}