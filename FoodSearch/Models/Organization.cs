using System;
using System.Collections.Generic;

namespace FoodSearch.Models;

public partial class Organization
{
    public int Id { get; set; }

    public string OGRN { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string EMail { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
