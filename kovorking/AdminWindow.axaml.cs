using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace kovorking;

public partial class AdminWindow : Window
{
    public int id{get;set;}
    public AdminWindow()
    {
        InitializeComponent();
    }
    public AdminWindow(int id)
    {
        InitializeComponent();
    }
}