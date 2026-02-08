using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quiz.Models;

public partial class Question
{
    [Key]
    public int QuestionId { get; set; }

    public int LectureId { get; set; }

    public string QuestionText { get; set; } = null!;

    [StringLength(50)]
    public string? Difficulty { get; set; }

    public int? TimeSeconds { get; set; }

    [InverseProperty("Question")]
    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    [ForeignKey("LectureId")]
    [InverseProperty("Questions")]
    public virtual Lecture Lecture { get; set; } = null!;

    [InverseProperty("Question")]
    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
