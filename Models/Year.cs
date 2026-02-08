using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quiz.Models;

public partial class Year
{
    [Key]
    public int YearId { get; set; }

    [StringLength(100)]
    public string YearName { get; set; } = null!;

    // الربط مع المواد (عشان الأدمن يعرض المواد المرفوعة)
    [InverseProperty("Year")]
    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();

    // الربط مع الطلاب (عشان الأدمن يحسب عدد الطلاب في الداتا بورد)
    [InverseProperty("Year")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}