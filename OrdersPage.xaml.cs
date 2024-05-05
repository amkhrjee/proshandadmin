using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Text;
using CommunityToolkit.WinUI.UI.Controls;
using System.Management;
using System.Threading;
using Microsoft.UI.Xaml.Media.Animation;


namespace proshandadmin
{
	public sealed partial class OrdersPage : Page
	{
		public List<Order> orders;
		public bool isBusy;
        readonly DataGrid dataGrid;
		public OrdersPage()
		{
			orders = new();
			isBusy = false;
            dataGrid = new()
			{
				AutoGenerateColumns = true,
				AlternatingRowBackground = (Brush)Application.Current.Resources["ControlFillColorTertiaryBrush"],
				IsReadOnly = true,
				ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader,
			};
			this.InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			if (orders.Count == 0)
			{
				orders = Order.Orders();
			}
			dataGrid.ItemsSource = orders;

			scrollView.Content = dataGrid;
		}

		// Refresh Button
		private async void Button_Click(object sender, RoutedEventArgs e)
		{
            myStoryboard.Begin();

            RefreshText.Text = "Loading";
			RefreshButton.IsEnabled = false;
            refreshIconTransform.Angle = 90;

            await Task.Delay(1000);

            refreshIconTransform.Angle = 0;
            RefreshButton.IsEnabled = true;
            RefreshText.Text = "Refresh";

			dataGrid.ItemsSource = null;
			orders = Order.Orders();
			dataGrid.ItemsSource = orders;
		}
	}
}
