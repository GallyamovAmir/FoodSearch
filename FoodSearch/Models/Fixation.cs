using System;
using System.Collections.Generic;

namespace FoodSearch.Models;

public partial class Fixation
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public DateTime DateOdFixation { get; set; }

    public virtual ICollection<FixationItem> FixationItems { get; set; } = new List<FixationItem>();

    public virtual User? User { get; set; }
}
