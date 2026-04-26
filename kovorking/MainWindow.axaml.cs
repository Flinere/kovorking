using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using kovorking.Context;
using Microsoft.EntityFrameworkCore;

namespace kovorking;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Login.Text != null && Password.Text != null)
        {
            using var bd = new PostgresContext();
            var regestration = bd.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == Login.Text && u.PasswordHash == Password.Text);
            if (regestration != null)
            {
                if (regestration.Role.Name == "Администратор")
                {
                    var window = new AdminWindow(regestration.Id);
                    window.Show();
                    this.Close();
                }
                else if (regestration.Role.Name == "Оператор")
                {
                    var window = new OperatorWindow(regestration.Id);
                    window.Show();
                    this.Close();
                }
                else if (regestration.Role.Name == "Клиент")
                {
                    var window = new UserWindow(regestration.Id);
                    window.Show();
                    this.Close();
                }
            }
            else
            {
                Blocks.Text = "Неправильные данные";
                return;
            }
        }
        else
        {
            Blocks.Text = "Введите данные";
            return;
        }
    }

    private void Button_OnClick1(object? sender, RoutedEventArgs e)
    {
        var window = new NotReg();
            window.Show();
            this.Close();
    }
}