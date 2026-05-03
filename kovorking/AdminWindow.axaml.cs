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

public partial class AdminWindow : Window
{
    private List<OrderViewModele> _originalOrders;
    private List<RoomViewModeles> _originalRooms;
    private string _currentOrderSortTag = "BigH";
    private string _currentRoomSortTag = "BigH";
    private string _orderSearchText = "";
    private string _orderStatusFilter = "All";
    private string _roomSearchText = "";
    private string _roomTypeFilter = "All";
    public int id{get;set;}
    public AdminWindow()
    {
        InitializeComponent();
    }
    public AdminWindow(int id)
    {
        InitializeComponent();
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
                created = book.CreatedAt,
                Capacity = book.Room.Capacity
            });
        }
        _originalOrders = items;
        Applyfilter();
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

        _originalRooms = items;
        ApplyRoom();

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
            if (dels.Status != "Новое" && dels.Status != "Подтверждено")
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
    
    private void Sort_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var Tag = (Sort.SelectedItem as ComboBoxItem)?.Tag.ToString();
        if (!string.IsNullOrEmpty(Tag))
        {
            _currentOrderSortTag = Tag;
            Applyfilter();
        }
    }

   

    private void Sort_r_OnSelectionChanged1(object? sender, SelectionChangedEventArgs e)
    {
        var Tag = (Sort_r.SelectedItem as ComboBoxItem)?.Tag.ToString();
        if (!string.IsNullOrEmpty(Tag))
        {
            _currentRoomSortTag = Tag;
            ApplyRoom();
        }
    }

    private void AppSortRoom(string tag)
    {
        if (_originalRooms == null)
        {
            return;
        }

        IEnumerable<RoomViewModeles> Room = tag switch
        {
            "BigH" => _originalRooms.OrderByDescending(x => x.MinimumDurration),
            "SmallH" => _originalRooms.OrderBy(x => x.MinimumDurration),
            "BigP" => _originalRooms.OrderByDescending((x => x.Capacity)),
            "SmallP" => _originalRooms.OrderBy(x => x.Capacity),
        };
        Box.ItemsSource = Room.ToList();
    }

    private void Search_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
            _orderSearchText = (sender as TextBox)?.Text?.Trim().ToLower() ?? "";
            Applyfilter();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item)
        {
            _orderStatusFilter = item.Tag?.ToString() ?? "All";
            Applyfilter();
        }
    }

    private void Applyfilter()
    {
        if (_originalOrders == null)
        {
            return;
        }
        var filtered = _originalOrders.Where(order =>
        {
            bool matchesSearch = string.IsNullOrEmpty(_orderSearchText) || 
                                 (order.Name?.ToLower().Contains(_orderSearchText) ?? false);
            bool matchesStatus = _orderStatusFilter == "All" || order.status == _orderStatusFilter;
        
            return matchesSearch && matchesStatus;
        }).ToList();
        
        IEnumerable<OrderViewModele> sorted = _currentOrderSortTag switch
        {
            "BigH"   => filtered.OrderByDescending(x => x.Duration),
            "SmallH" => filtered.OrderBy(x => x.Duration),
            "BigP"   => filtered.OrderByDescending(x => x.Capacity),
            "SmallP" => filtered.OrderBy(x => x.Capacity),
            _ => filtered
        };

        ListBoxse.ItemsSource = sorted.ToList();

    }

    private void StatsR_OnSelectionChanged2(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem item)
        {
            _roomTypeFilter = item.Tag?.ToString() ?? "All";
            ApplyRoom();
        }
    }

    private void SearchR_OnTextChanged2(object? sender, TextChangedEventArgs e)
    {
        _roomSearchText = (sender as TextBox)?.Text?.Trim().ToLower() ?? "";
        ApplyRoom();
    }
    
    private void ApplyRoom()
    {
        if (_originalRooms == null) return;

        var filtered = _originalRooms.Where(room =>
        {
            bool matchesSearch = string.IsNullOrEmpty(_roomSearchText) || 
                                 (room.Name?.ToLower().Contains(_roomSearchText) ?? false) ||
                                 (room.Type?.ToLower().Contains(_roomSearchText) ?? false);
        
            bool matchesType = _roomTypeFilter == "All" || room.Type == _roomTypeFilter;
        
            return matchesSearch && matchesType;
        }).ToList();

        IEnumerable<RoomViewModeles> sorted = _currentRoomSortTag switch
        {
            "BigH"   => filtered.OrderByDescending(x => x.MinimumDurration),
            "SmallH" => filtered.OrderBy(x => x.MinimumDurration),
            "BigP"   => filtered.OrderByDescending(x => x.Capacity),
            "SmallP" => filtered.OrderBy(x => x.Capacity),
            _ => filtered
        };

        Box.ItemsSource = sorted.ToList();
    }
}