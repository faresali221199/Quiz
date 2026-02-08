using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Quiz.Models;

public partial class JsonFile
{
    [Key]
    public int JsonFileId { get; set; }

    [StringLength(255)]
    public string FileName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? UploadedAt { get; set; }

    [StringLength(100)]
    public string? UploadedBy { get; set; }

    public string FileContent { get; set; } = null!;
}
