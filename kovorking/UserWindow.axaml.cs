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

public class RoomViewModels
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Capacity { get; set; }
    public decimal Hourlyrate { get; set; }
    public decimal? Discount { get; set; }
    public decimal? Rating { get; set; }
    public int MinimumDurration { get; set; }
    public IImage? PhotoImage { get; set; }
    
    public decimal? FinalPrice => Hourlyrate * (1 - Discount / 100);
    public bool HasDiscount => Discount > 0;
    public bool IsSuperDiscount => Discount > 20m;     
    public bool IsPopular => Rating > 4.5m;                     
    public bool IsAvailableToday { get; set; } = true;
}

public class OrderViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public IImage? PhotoImage { get; set; }
    public DateTime? Date { get; set; }
    public decimal Duration { get; set; }
    public string? status { get; set; }
    public DateTime? created { get; set; }
}


public partial class UserWindow : Window
{
    public int id{get;set;}
    public UserWindow()
    {
        InitializeComponent();
    }
    public UserWindow(int id)
    {
        InitializeComponent();
        this.id = id;
        var bd = new PostgresContext();
        var dd = bd.Users.FirstOrDefault(u => u.Id == id);
        Blocke.Text = dd.FullName;
        listes();
        Order(id);
        LoadRoom();
    }

    private async Task Order(int id)
    {
        using var bd = new PostgresContext();
        var OrderRoom = await bd.Bookings.Include(b => b.Room).Include(b => b.User).Where(u => u.UserId == id)
            .ToListAsync();
        
        var items = new List<OrderViewModel>();
        foreach (var book in OrderRoom)
        {
            items.Add(new OrderViewModel()
            {
                Id  = book.Id,
                Name = book.Room.Name,
                Type = book.Room.Type,
                PhotoImage = LoadLocalImage(book.Room.PhotoUrl),
                Date = book.StartTime,
                Duration = book.DurationHours,
                status = book.Status,
                created = book.CreatedAt
            });
        }
        ListBoxse.ItemsSource = items;
    }

    private async Task LoadRoom()
    {
        using var bd = new PostgresContext();
        var roome = await bd.Rooms.ToListAsync();
        var items = new List<RoomViewModels>();
        foreach (var room in roome)
        {
            items.Add(new RoomViewModels()
            {
                Id = room.Id,
                Name = room.Name,
                Type = room.Type,
                Capacity = room.Capacity,
                Hourlyrate = room.HourlyRate,
                Discount = room.DiscountPercent,
                Rating = room.Rating,
                MinimumDurration = room.MinHours,
                PhotoImage = LoadLocalImage(room.PhotoUrl),
                IsAvailableToday = true
            });
        }

        Boxa.ItemsSource = items;

    }

    private async Task listes()
    {
        using var bd = new PostgresContext();
        var today = DateTime.Today;
        var rooms = await bd.Rooms.ToListAsync();
        var items = new List<RoomViewModels>();
        

        foreach (var room in rooms)
        {
            bool isBooked = await bd.Bookings.AnyAsync(b => b.RoomId == room.Id && b.Status != "Отменено" && b.StartTime.Date == today);
            items.Add(new RoomViewModels
            {
                Id = room.Id,
                Name = room.Name,
                Type = room.Type,
                Capacity = room.Capacity,
                Hourlyrate = room.HourlyRate,
                Discount = room.DiscountPercent,
                Rating = room.Rating,
                MinimumDurration = room.MinHours,
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

    private async void Button_OnClick1(object? sender, RoutedEventArgs e)
    {
        err.Text = "";
        if (Boxa.SelectedItem is not RoomViewModels romesel || !int.TryParse(Boxe.Text, out int durrh ) || DatePickera == null || TimePicker == null)
        {
            err.Text = "Заполните поля";
            return;
        }
        if (durrh > 24 || durrh < romesel.MinimumDurration)
        {
            err.Text = "Выберите правильное время аренды";
            return;
        }

        var start = DatePickera.SelectedDate.Value.Date.Add(TimePicker.SelectedTime.Value);
        var end = start + TimeSpan.FromHours(durrh);
        using var bd = new PostgresContext();
        bool eror = await bd.Bookings.AnyAsync(b => b.RoomId == romesel.Id && b.Status != "Отменено" && b.StartTime < end && b.StartTime.AddHours((Double)(b.DurationHours)) > start);
        if (eror)
        {
            err.Text = "Комната занята";
        }

        try
        {
            var newbook = new Booking()
            {
                UserId = id,
                RoomId = romesel.Id,
                StartTime = start,
                DurationHours = durrh,
                Status = "Новое",
                CreatedAt = DateTime.Now
            };
            bd.Bookings.Add(newbook);
            await bd.SaveChangesAsync();
            err.Text = "Добавленно";

            await Order(id);
            await listes();
        }
        catch
        {
            err.Text = "ошибка";
            return;
        }
        

    }


    private async void Button_OnClickDel(object? sender, RoutedEventArgs e)
    {
        err.Text = "";
        if (ListBoxse.SelectedItem == null)
        {
            err.Text = "Выберите элемент для удаления";
            return;
        }

        try
        {
            var dele = ListBoxse.SelectedItem as OrderViewModel;
            using var bd = new PostgresContext();
            var dels = await bd.Bookings.FirstOrDefaultAsync(b => b.Id == dele.Id);
            if (dels.Status != "Новое")
            {
                err.Text = "Отменять можно только новый заказ";
                return;
            }

            dels.Status = "Отменено";
            await bd.SaveChangesAsync();
            err.Text = "Удаление выполнено";
            await Order(id);
        }
        catch
        {
            err.Text = "Ощибка";
        }
    }
}