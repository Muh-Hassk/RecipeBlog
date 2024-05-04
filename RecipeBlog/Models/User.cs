using System;
using System.Collections.Generic;

namespace RecipeBlog.Models;

public partial class User
{
    public decimal Userid { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public decimal Roleid { get; set; }

    public DateTime Registrationdate { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Reciperequest> Reciperequests { get; set; } = new List<Reciperequest>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();
}
