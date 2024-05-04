using System;
using System.Collections.Generic;

namespace RecipeBlog.Models;

public partial class Payment
{
    public decimal Paymentid { get; set; }

    public decimal? Userid { get; set; }

    public decimal? Recipeid { get; set; }

    public decimal? Paymentamount { get; set; }

    public DateTime? Paymentdate { get; set; }

    public string? Paymentstatus { get; set; }

    public virtual Recipe? Recipe { get; set; }

    public virtual User? User { get; set; }
}
