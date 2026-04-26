using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace kovorking;

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
    }
}