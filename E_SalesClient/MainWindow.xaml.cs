using E_SalesClient.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace E_SalesClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:57252/") };
            LoadData();
        }

        private async void LoadData()
        {
            // Load products
            var productsResponse = await _httpClient.GetStringAsync("api/Products");
            var products = JsonConvert.DeserializeObject<List<Product>>(productsResponse);
            ProductsComboBox.ItemsSource = products;

            // Load customers
            var customersResponse = await _httpClient.GetStringAsync("api/Customers");
            var customers = JsonConvert.DeserializeObject<List<Customer>>(customersResponse);
            CustomersComboBox.ItemsSource = customers;
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected data
            var selectedProduct = ProductsComboBox.SelectedItem as Product;
            var selectedCustomer = CustomersComboBox.SelectedItem as Customer;
            var quantity = int.TryParse(QuantityTextBox.Text, out var parsedQuantity) ? parsedQuantity : 0;
            var orderDate = OrderDatePicker.SelectedDate;

            if (selectedProduct == null || selectedCustomer == null || quantity <= 0 || orderDate == null)
            {
                MessageBox.Show("Please fill in all fields correctly.");
                return;
            }

            var newOrder = new
            {
                ProductId = selectedProduct.ProductId,
                CustomerId = selectedCustomer.CustomerId,
                Quantity = quantity,
                OrderDate = orderDate.Value.ToString("yyyy-MM-dd")
            };

            var content = new StringContent(JsonConvert.SerializeObject(newOrder), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Orders", content);

            if (response.IsSuccessStatusCode)
                MessageBox.Show("Order added successfully!");
            else
                MessageBox.Show("Failed to add order.");
        }

        private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrdersWindow();
            ordersWindow.Show();
        }
    }
}
