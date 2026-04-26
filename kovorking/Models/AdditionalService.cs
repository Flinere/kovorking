using System;
using System.Collections.Generic;

namespace kovorking.Models;

public partial class AdditionalService
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Cost { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
}
