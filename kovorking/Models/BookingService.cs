using System;
using System.Collections.Generic;

namespace kovorking.Models;

public partial class BookingService
{
    public int BookingId { get; set; }

    public int ServiceId { get; set; }

    public int? Quantity { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual AdditionalService Service { get; set; } = null!;
}
