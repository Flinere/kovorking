using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using kovorking.Context;
using kovorking.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using System.IO;
using Avalonia.Media;
using System.Threading.Tasks;
using Avalonia.Interactivity;

namespace kovorking;

public class RoomViewModel
{
    public string Name { get; set; }
    public string Type { get; set; }
    public int Capacity { get; set; }
    public decimal Hourlyrate { get; set; }
    public decimal? Discount { get; set; }
    public decimal? Rating { get; set; }
    public IImage? PhotoImage { get; set; }
    
    public decimal? FinalPrice => Hourlyrate * (1 - Discount / 100);
    public bool HasDiscount => Discount > 0;
    public bool IsSuperDiscount => Discount > 20m; 
    public bool IsPopular => Rating > 4.5m;
    public bool IsAvailableToday { get; set; } = true;
}
public partial class NotReg : Window
{
    public NotReg()
    {
        InitializeComponent();
        listes();
    }

    private async Task listes()
    {
        using var bd = new PostgresContext();
        var today = DateTime.Today;
        var rooms = await bd.Rooms.ToListAsync();
        var items = new List<RoomViewModel>();
        

        foreach (var room in rooms)
        {
            bool isBooked = await bd.Bookings.AnyAsync(b => b.RoomId == room.Id && b.Status != "Отменено" && b.StartTime.Date == today);
            items.Add(new RoomViewModel
            {
                Name = room.Name,
                Type = room.Type,
                Capacity = room.Capacity,
                Hourlyrate = room.HourlyRate,
                Discount = room.DiscountPercent,
                Rating = room.Rating,
                PhotoImage = LoadLocalImage(room.PhotoUrl),
                IsAvailableToday = !isBooked
            });
        }
        Box.ItemsSource =  items;
        

    }
    private IImage? LoadLocalImage(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return null;

        var fullPath = Path.Combine(AppContext.BaseDirectory, path);

        if (!File.Exists(fullPath))
            return null;

        try
        {
            return new Bitmap(fullPath);
        }
        catch
        {
            return null;
        }
    }

    private void Box_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var window = new MainWindow();
        window.Show();
        this.Close();
    }
}