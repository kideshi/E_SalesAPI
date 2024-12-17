using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using E_SalesClient.Models;
using Newtonsoft.Json;

namespace E_SalesClient
{
    /// <summary>
    /// Interaction logic for OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        private readonly HttpClient _httpClient;

        public OrdersWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:57252/") };
            LoadOrders();
        }

        private async void LoadOrders()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("api/OrdersDto");
                var orders = JsonConvert.DeserializeObject<List<OrderDto>>(response);
                OrdersDataGrid.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while loading data. Contact administrator");
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is OrderDto selectedOrder)
            {
                // Окно подтверждения удаления
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the order with ID {selectedOrder.OrderId}?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                // Проверка на подтверждение пользователя
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"api/Orders/{selectedOrder.OrderId}");
                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Order deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadOrders();
                        }
                        else
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Failed to delete order. Server response: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an order to delete.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //private async void EditButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (OrdersDataGrid.SelectedItem is OrderDto selectedOrder)
        //    {
        //        // Here you can implement logic for editing the selected order
        //        var content = new StringContent(JsonConvert.SerializeObject(selectedOrder), Encoding.UTF8, "application/json");
        //        var response = await _httpClient.PutAsync($"api/Orders/{selectedOrder.OrderId}", content);

        //        if (response.IsSuccessStatusCode)
        //            MessageBox.Show("Order updated successfully!");
        //        else
        //            MessageBox.Show("Failed to update order.");
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please select an order to edit.");
        //    }
        //}

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is OrderDto selectedOrder)
            {
                // Проверка на валидность введенных данных
                if (string.IsNullOrWhiteSpace(selectedOrder.Product) ||
                    string.IsNullOrWhiteSpace(selectedOrder.Customer) ||
                    selectedOrder.Quantity <= 0)
                {
                    MessageBox.Show("Please ensure all fields are filled correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    // Создаем тело запроса
                    var json = JsonConvert.SerializeObject(selectedOrder);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Отправляем PUT запрос
                    var response = await _httpClient.PutAsync($"api/OrdersDto/{selectedOrder.OrderId}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Order updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadOrders(); // Обновляем данные после успешного изменения
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to update order. Server response: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an order to edit.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
