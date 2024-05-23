using System;
using System.Collections.Generic;

namespace RecipeBlog.Models;

public partial class Aboutuspage
{
    public decimal Id { get; set; }

    public string Pagecontent { get; set; } = null!;

    public string? Imagepath { get; set; }
}
