using System;
using System.Collections.Generic;

namespace FoodSearch.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? OrganizationId { get; set; }

    public int? SubscriptionId { get; set; }

    public virtual ICollection<Fixation> Fixations { get; set; } = new List<Fixation>();

    public virtual Organization? Organization { get; set; }

    public virtual Subscription? Subscription { get; set; }

    public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();
}
