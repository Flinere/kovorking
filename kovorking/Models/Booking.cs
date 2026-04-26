using System;
using System.Collections.Generic;

namespace kovorking.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RoomId { get; set; }

    public DateTime StartTime { get; set; }

    public decimal DurationHours { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
