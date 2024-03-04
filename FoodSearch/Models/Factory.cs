using System;
using System.Collections.Generic;

namespace FoodSearch.Models;

public partial class Factory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
