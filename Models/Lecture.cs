using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Models;

public partial class Lecture
{
    [Key]
    public int LectureId { get; set; }

    [StringLength(255)]
    public string LectureTitle { get; set; } = null!;

    public int SubjectId { get; set; }

    [ForeignKey("SubjectId")]
    [InverseProperty("Lectures")]
    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    // الربط العكسي مع النتائج
    [InverseProperty("Lecture")]
    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}