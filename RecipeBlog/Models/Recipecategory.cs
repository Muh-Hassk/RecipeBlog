using System;
using System.Collections.Generic;

namespace RecipeBlog.Models;

public partial class Recipecategory
{
    public decimal Categoryid { get; set; }

    public string? Categoryname { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
