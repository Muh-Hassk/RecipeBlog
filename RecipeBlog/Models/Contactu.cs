using System;
using System.Collections.Generic;

namespace RecipeBlog.Models;

public partial class Contactu
{
    public decimal Id { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Openhours { get; set; } = null!;
}
