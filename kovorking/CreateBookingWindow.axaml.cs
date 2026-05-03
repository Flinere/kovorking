using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using kovorking.Context;
using kovorking.Models;
using kovorking.Services;
using Microsoft.EntityFrameworkCore;


namespace kovorking;

public partial class CreateBookingWindow : Window
{
    private readonly BookingServices _services;
    private readonly PostgresContext _context;
    public bool Success { get; private set; }

    public CreateBookingWindow()
    {
        InitializeComponent();
    }
    public CreateBookingWindow(BookingServices services, PostgresContext context)
    {
        InitializeComponent();
        _services = services;
        _context = context;
        Loaded += OnLoaded;
    }
    private async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        ComboUsers.ItemsSource = await _context.Users.OrderBy(u => u.FullName).ToListAsync();
        ComboRooms.ItemsSource = await _context.Rooms.ToListAsync();
    }

    private async void BtnCheckAvailability_Click(object? sender, RoutedEventArgs e)
    {
        if (PickDate.SelectedDate is not DateTimeOffset dateOffset || 
            PickTime.SelectedTime is not TimeSpan time || 
            !int.TryParse(TxtHours.Text, out int hours) || hours <= 0)
        {
            LblStatus.Text = "Укажите дату, время и длительность";
            LblStatus.Foreground = Avalonia.Media.Brushes.Orange;
            return;
        }

        var date = dateOffset.DateTime;
        var start = date.Date + time;
    
        var freeRooms = await _services.GetAvailableRoomsAsync(start, hours);
    
        ComboRooms.ItemsSource = freeRooms;
        LblStatus.Text = $"Найдено {freeRooms.Count} свободных комнат";
        LblStatus.Foreground = Avalonia.Media.Brushes.Green;
    }

    private async void BtnSave_Click(object? sender, RoutedEventArgs e)
    {
        if (ComboUsers.SelectedItem is not User user || 
            ComboRooms.SelectedItem is not Room room || 
            PickDate.SelectedDate is not DateTimeOffset date || 
            PickTime.SelectedTime is not TimeSpan time || 
            !int.TryParse(TxtHours.Text, out int hours) || hours <= 0)
        {
            LblStatus.Text = "Заполните все поля";
            LblStatus.Foreground = Avalonia.Media.Brushes.Red;
            return;
        }
        
        int minHours = room.MinHours;
        if (hours < minHours)
        {
            LblStatus.Text = $"Минимальное время аренды для этой комнаты: {minHours} ч.";
            LblStatus.Foreground = Avalonia.Media.Brushes.Red;
            return;
        }

        try
        {
            var booking = new Booking
            {
                UserId = user.Id,
                RoomId = room.Id,
                StartTime = date.Date + time,
                DurationHours = hours
            };

            await _services.CreateAsync(booking);
            Success = true;
            Close();
        }
        catch (Exception ex)
        {
            LblStatus.Text = $"{ex.Message}";
            LblStatus.Foreground = Avalonia.Media.Brushes.Red;
        }
    }

    private void BtnCancel_Click(object? sender, RoutedEventArgs e) => Close();
}