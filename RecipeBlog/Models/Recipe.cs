using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeBlog.Models;

public partial class Recipe
{
    public decimal Recipeid { get; set; }

    public string? Recipename { get; set; }

    public string? Ingredients { get; set; }

    public string? Instructions { get; set; }

    public decimal? Chefid { get; set; }

    public decimal? Categoryid { get; set; }

    public decimal? Price { get; set; }

    public string? Isaccepted { get; set; }
    public virtual Recipecategory? Category { get; set; }

    public virtual User? Chef { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Reciperequest> Reciperequests { get; set; } = new List<Reciperequest>();
}
