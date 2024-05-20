using System;
using System.Collections.Generic;

namespace RecipeBlog.Models;

public partial class Cardinfo
{
    public decimal Cardid { get; set; }

    public decimal? Userid { get; set; }

    public string Cardnumber { get; set; } = null!;

    public string Cardholdername { get; set; } = null!;

    public DateTime Expirydate { get; set; }

    public string Cvv { get; set; } = null!;

    public decimal Balance { get; set; }

    public virtual User? User { get; set; }
}
