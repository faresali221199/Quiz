using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Models;

public partial class Result
{
    [Key]
    public int ResultId { get; set; }

    public int UserId { get; set; }

    public int TotalScore { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndTime { get; set; }

    public int? LectureId { get; set; }

    // الربط مع الطالب
    [ForeignKey("UserId")]
    [InverseProperty("Results")]
    public virtual User User { get; set; } = null!;

    // الربط مع المحاضرة (هذا السطر هو حل الإيرور الأخير)
    [ForeignKey("LectureId")]
    [InverseProperty("Results")]
    public virtual Lecture? Lecture { get; set; }

    // الربط مع تفاصيل إجابات الطالب
    [InverseProperty("Result")]
    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}