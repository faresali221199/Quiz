using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quiz.Models;

public partial class UserAnswer
{
    [Key]
    public int UserAnswerId { get; set; }

    public int ResultId { get; set; }

    public int QuestionId { get; set; }

    public int? SelectedAnswerId { get; set; }

    public bool? IsCorrect { get; set; }

    [ForeignKey("QuestionId")]
    [InverseProperty("UserAnswers")]
    public virtual Question Question { get; set; } = null!;

    [ForeignKey("ResultId")]
    [InverseProperty("UserAnswers")]
    public virtual Result Result { get; set; } = null!;

    [ForeignKey("SelectedAnswerId")]
    [InverseProperty("UserAnswers")] // تأكد أن جدول Answer يحتوي على ICollection<UserAnswer> بنفس الاسم
    public virtual Answer? SelectedAnswer { get; set; }
}