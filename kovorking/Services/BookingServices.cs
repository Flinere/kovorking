using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using kovorking.Context;
using kovorking.Models;
using Microsoft.EntityFrameworkCore;

namespace kovorking.Services;

public class BookingServices
{
    private readonly PostgresContext _context;
    public BookingServices(PostgresContext context)
    {
        _context = context;
    }
    
    public async Task<Booking> CreateAsync(Booking booking)
    {
        bool conflict = await HasTimeConflictAsync(booking.RoomId, booking.StartTime, (int)booking.DurationHours);
        if (conflict) throw new Exception("Комната уже занята на это время!");

        booking.Status = "Новое";
        booking.CreatedAt = DateTime.Now;

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }
    
    public async Task<bool> UpdateStatusAsync(int id, string newStatus)
    {
        var b = await _context.Bookings.FindAsync(id);
        if (b == null || b.Status == "Завершено") return false;
        
        b.Status = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var b = await _context.Bookings.FindAsync(id);
        if (b == null) return false;
        _context.Bookings.Remove(b);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> HasTimeConflictAsync(int roomId, DateTime start, int hours, int? excludeId = null)
    {
        var end = start.AddHours(hours);
        var q = _context.Bookings.Where(b => b.RoomId == roomId && b.Status != "Отменено" 
                                          && b.StartTime < end && b.StartTime.AddHours((double)b.DurationHours) > start);
        if (excludeId.HasValue) q = q.Where(b => b.Id != excludeId);
        return await q.AnyAsync();
    }
    
    public async Task<List<Room>> GetAvailableRoomsAsync(DateTime start, int hours)
    {
        var rooms = await _context.Rooms.ToListAsync();
        var end = start.AddHours(hours);
        var available = new List<Room>();
        
        foreach (var r in rooms)
        {
            bool busy = await _context.Bookings.AnyAsync(b => 
                b.RoomId == r.Id && b.Status != "Отменено" && b.StartTime < end && b.StartTime.AddHours((double)b.DurationHours) > start);
            if (!busy) available.Add(r);
        }
        return available;
    }

    public async Task<string> GetStatisticsTextAsync(DateTime? from = null, DateTime? to = null)
    {
        from ??= DateTime.Today;
        to ??= DateTime.Today.AddDays(30);

        var bookings = await _context.Bookings.Include(b => b.Room)
            .Where(b => b.Status != "Отменено" && b.StartTime >= from && b.StartTime < to).ToListAsync();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"📊 За период {from:dd.MM} - {to:dd.MM}");
        sb.AppendLine($"Всего броней: {bookings.Count}");
        sb.AppendLine($"Ср. длительность: {(bookings.Any() ? bookings.Average(b => b.DurationHours) : 0):F1} ч.");
        sb.AppendLine("\n🏢 По комнатам:");

        foreach (var g in bookings.GroupBy(b => b.Room?.Name ?? "Неизвестно"))
        {
            sb.AppendLine($"• {g.Key}: {g.Count()} броней, {g.Sum(b => b.DurationHours)} ч.");
        }

        return sb.ToString();
    }
}