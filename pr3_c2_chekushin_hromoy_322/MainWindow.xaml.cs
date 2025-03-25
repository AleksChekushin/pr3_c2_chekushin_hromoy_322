using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pr3_c2_chekushin_hromoy_322
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double BaseRate = 8.0; // Базовая ставка за 1 км

        public MainWindow()
        {
            InitializeComponent();
            // Очистка полей при запуске
            distanceTextBox.Text = "";
            ticketsCountTextBox.Text = "";
            resultTextBlock.Text = "";
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры и Backspace
            if (!char.IsDigit(e.Text, 0) && e.Text != "\b")
            {
                e.Handled = true;
                ShowError("Допустимы только цифры!");
                return;
            }

            var textBox = sender as TextBox;
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);

            // Проверка на пустую строку
            if (string.IsNullOrWhiteSpace(newText))
            {
                return;
            }

            // Проверка на ведущие нули
            if (newText.Length > 1 && newText.StartsWith("0"))
            {
                e.Handled = true;
                ShowError("Число не может начинаться с нуля!");
                return;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            // Проверка на пустое поле
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox == distanceTextBox)
                {
                    ShowError("Поле 'Расстояние' не может быть пустым!");
                }
                else if (textBox == ticketsCountTextBox)
                {
                    ShowError("Поле 'Количество билетов' не может быть пустым!");
                }
                return;
            }

            // Проверка на корректное положительное число
            if (!int.TryParse(textBox.Text, out int value) || value <= 0)
            {
                ShowError("Введите положительное число!");
                textBox.Text = "";
                textBox.Focus();
                return;
            }
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                resultTextBlock.Foreground = System.Windows.Media.Brushes.Black;
                resultTextBlock.Text = "";

                // Проверка заполнения всех полей
                if (string.IsNullOrWhiteSpace(distanceTextBox.Text))
                {
                    ShowError("Пожалуйста, введите расстояние!");
                    distanceTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(ticketsCountTextBox.Text))
                {
                    ShowError("Пожалуйста, введите количество билетов!");
                    ticketsCountTextBox.Focus();
                    return;
                }

                // Проверка выбора комфортабельности
                if (!platzkartRadio.IsChecked.Value &&
                    !coupeRadio.IsChecked.Value &&
                    !halfLuxRadio.IsChecked.Value &&
                    !luxRadio.IsChecked.Value)
                {
                    ShowError("Пожалуйста, выберите тип комфортабельности!");
                    return;
                }

                if (!int.TryParse(distanceTextBox.Text, out int distance) || distance <= 0)
                {
                    ShowError("Расстояние должно быть положительным числом!");
                    distanceTextBox.Focus();
                    distanceTextBox.SelectAll();
                    return;
                }

                if (!int.TryParse(ticketsCountTextBox.Text, out int ticketsCount) || ticketsCount <= 0)
                {
                    ShowError("Количество билетов должно быть положительным числом!");
                    ticketsCountTextBox.Focus();
                    ticketsCountTextBox.SelectAll();
                    return;
                }

                // Определяем коэффициент комфортабельности
                double comfortFactor = GetComfortFactor();

                // Вычисляем стоимость
                double totalCost = distance * BaseRate * comfortFactor * ticketsCount;

                // Выводим результат
                resultTextBlock.Text = $"Стоимость {ticketsCount} билетов на расстояние {distance} км:\n" +
                                    $"{totalCost:F2} рублей\n" +
                                    $"Тариф: {BaseRate} руб/км\n" +
                                    $"Коэффициент комфорта: {comfortFactor:F1}";
            }
            catch (Exception ex)
            {
                ShowError($"Критическая ошибка: {ex.Message}");
            }
        }

        private double GetComfortFactor()
        {
            if (coupeRadio.IsChecked.Value) return 1.1;
            if (halfLuxRadio.IsChecked.Value) return 1.2;
            if (luxRadio.IsChecked.Value) return 1.3;
            return 1.0;
        }

        private void ShowError(string message)
        {
            resultTextBlock.Foreground = System.Windows.Media.Brushes.Red;
            resultTextBlock.Text = message;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                ShowError("Пробелы недопустимы!");
            }
        }
    }
}
