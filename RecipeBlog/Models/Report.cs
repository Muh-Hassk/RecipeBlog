using System;
using System.Collections.Generic;

namespace RecipeBlog.Models;

public partial class Report
{
    public decimal Reportid { get; set; }

    public string? Reporttype { get; set; }

    public string? Reportcontent { get; set; }

    public DateTime? Generatedate { get; set; }
}
