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
public class RoomViewModeles
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

public class OrderViewModele
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

public partial class OperatorWindow : Window
{
    public int id{get;set;}
    public OperatorWindow()
    {
        InitializeComponent();
    }

    public OperatorWindow(int id)
    {
        InitializeComponent();
        this.id = id;
        var bd = new PostgresContext();
        var dd = bd.Users.FirstOrDefault(u => u.Id == id);
        Blocke.Text = dd.FullName;
        listes();
        Order();
    }
    
    private async Task Order()
    {
        using var bd = new PostgresContext();
        var OrderRoom = await bd.Bookings.Include(b => b.Room).Include(b => b.User)
            .ToListAsync();
        
        var items = new List<OrderViewModele>();
        foreach (var book in OrderRoom)
        {
            items.Add(new OrderViewModele()
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
        
    }

    private async Task listes()
    {
        using var bd = new PostgresContext();
        var today = DateTime.Today;
        var rooms = await bd.Rooms.ToListAsync();
        var items = new List<RoomViewModeles>();
        

        foreach (var room in rooms)
        {
            bool isBooked = await bd.Bookings.AnyAsync(b => b.RoomId == room.Id && b.Status != "Отменено" && b.StartTime.Date == today);
            items.Add(new RoomViewModeles
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
    
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var window = new MainWindow();
        window.Show();
        this.Close();
    }

    private async void Button_OnClick1(object? sender, RoutedEventArgs e)
    {
        err.Text = "";
        if (ListBoxse.SelectedItem == null)
        {
            err.Text = "Выберите элемент для подтверждения";
            return;
        }

        try
        {
            var dele = ListBoxse.SelectedItem as OrderViewModele;
            using var bd = new PostgresContext();
            var dels = await bd.Bookings.FirstOrDefaultAsync(b => b.Id == dele.Id);
            if (dels.Status != "Новое")
            {
                err.Text = "Подтвердить можно только новый заказ";
                return;
            }

            dels.Status = "Подтверждено";
            await bd.SaveChangesAsync();
            err.Text = "Подтверждение выполнено";
            await Order();
        }
        catch
        {
            err.Text = "Ощибка";
        }
    }


    private async void Button_OnClickDel(object? sender, RoutedEventArgs e)
    {
        err.Text = "";
        if (ListBoxse.SelectedItem == null)
        {
            err.Text = "Выберите элемент для отмены";
            return;
        }

        try
        {
            var dele = ListBoxse.SelectedItem as OrderViewModele;
            using var bd = new PostgresContext();
            var dels = await bd.Bookings.FirstOrDefaultAsync(b => b.Id == dele.Id);
            if (dels.Status != "Новое" || dels.Status != "Подтверждено")
            {
                err.Text = "Отменять можно только новый заказ или подтвержденый";
                return;
            }

            dels.Status = "Отменено";
            await bd.SaveChangesAsync();
            err.Text = "Отмена выполнена";
            await Order();
        }
        catch
        {
            err.Text = "Ощибка";
        }
    }

   
}