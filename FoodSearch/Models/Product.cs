using System;
using System.Collections.Generic;

namespace FoodSearch.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ImageSource { get; set; } = null!;

    public string Url { get; set; } = null!;

    public decimal Price { get; set; }

    public int? FactotyId { get; set; }

    public virtual Factory? Factoty { get; set; }

    public virtual ICollection<FixationItem> FixationItems { get; set; } = new List<FixationItem>();
}
