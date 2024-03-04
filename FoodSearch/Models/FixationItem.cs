using System;
using System.Collections.Generic;

namespace FoodSearch.Models;

public partial class FixationItem
{
    public int Id { get; set; }

    public int? FixationId { get; set; }

    public int? ProductId { get; set; }

    public int Count { get; set; }

    public virtual Fixation? Fixation { get; set; }

    public virtual Product? Product { get; set; }
}
