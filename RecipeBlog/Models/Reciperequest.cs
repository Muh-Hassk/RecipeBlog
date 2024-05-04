﻿using System;
using System.Collections.Generic;

namespace RecipeBlog.Models;

public partial class Reciperequest
{
    public decimal Requestid { get; set; }

    public decimal? Userid { get; set; }

    public decimal? Recipeid { get; set; }

    public DateTime? Requestdate { get; set; }

    public virtual Recipe? Recipe { get; set; }

    public virtual User? User { get; set; }
}
