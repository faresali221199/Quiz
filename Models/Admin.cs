using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quiz.Models;

[Index("Username", Name = "UQ__Admins__536C85E4762CABCA", IsUnique = true)]
public partial class Admin
{
    [Key]
    public int AdminId { get; set; }

    [StringLength(100)]
    public string Username { get; set; } = null!;

    [StringLength(500)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(150)]
    public string? FullName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }
}
