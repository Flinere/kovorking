using System;
using System.Collections.Generic;
using System.IO;

namespace kovorking.Models;

public partial class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int Capacity { get; set; }

    public decimal HourlyRate { get; set; }

    public int MinHours { get; set; }

    public decimal? DiscountPercent { get; set; }

    public string? Description { get; set; }

    public decimal? Rating { get; set; }

    public string? PhotoUrl { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
