using System;
using System.Collections.Generic;

namespace FoodSearch.Models;

public partial class Subscription
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
