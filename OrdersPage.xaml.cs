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
using CommunityToolkit.Mvvm.Input;
using System.Management;
using System.Threading;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.System;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Search;
using Newtonsoft.Json;
using System.IO;
using Google.Cloud.Firestore;


namespace proshandadmin
{
    public sealed partial class OrdersPage : Page
    {
        public List<Order> orders;
        public bool isBusy;
        public int newOrdersCount;
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Mount the loading UI
            ProgressRing progressRing = new ProgressRing
            {
                IsIndeterminate = true
            };
            TextBlock textBlock = new TextBlock
            {
                Text = "Fetching data",
                Style = (Style)Resources["SubtitleTextBlockStyle"],
            };
            StackPanel stackPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 10
            };
            stackPanel.Children.Add(progressRing);
            stackPanel.Children.Add(textBlock);
            scrollView.Content = stackPanel;

            //This should load the page without waiting for the data
            if (orders.Count == 0)
            {
                try
                {
                    orders = await Order.Orders();
                }
                catch (Exception ex)
                {
                    ContentDialog dialog = new()
                    {
                        XamlRoot = this.XamlRoot,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        Title = "Couldn't connect to internet",
                        PrimaryButtonText = "Network Settings",
                        CloseButtonText = "Close App",
                        DefaultButton = ContentDialogButton.Primary,
                        Content = $"Could not reach database servers. An active internet connection is required to view orders.\n\nError Description: {ex.Message}",
                        PrimaryButtonCommand = new RelayCommand(OpenNetworkSettings),
                        CloseButtonCommand = new RelayCommand(() => Application.Current.Exit()),
                    };
                    dialog.Closing += (s, e) => Application.Current.Exit();
                    await dialog.ShowAsync();
                }
            }

            while (orders.Count == 0)
            {
                //Show loading state
            }

            newOrdersCount = 0;
            foreach (var order in orders)
            {
                if (!order.IsDone)
                {
                    newOrdersCount++;
                }
            }

            NewOrdersCount.Text = newOrdersCount.ToString();

            dataGrid.ItemsSource = orders;

            scrollView.Content = dataGrid;
        }

        private static async Task CloseApp(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            Application.Current.Exit();

            await Task.CompletedTask;
        }

        private static async void OpenNetworkSettings()
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:network"));
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
            orders = await Order.Orders();
            dataGrid.ItemsSource = orders;
            newOrdersCount = 0;
            foreach (var order in orders)
            {
                if (!order.IsDone)
                {
                    newOrdersCount++;
                }
            }

            NewOrdersCount.Text = null;
            NewOrdersCount.Text = newOrdersCount.ToString();
        }

        // Process new orders
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessButtonText.Text = "Processing";
                ProcessButton.IsEnabled = false;
                var settingsFile = await StorageFile.GetFileFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\\settings.json");

                string json = File.ReadAllText(settingsFile.Path);
                var settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var thirtyPath = settings["Thirty"].Replace('>', '\\');
                var thirtyFivePath = settings["ThirtyFive"].Replace('>', '\\');
                var fortyPath = settings["Forty"].Replace('>', '\\');
                var fortyFivePath = settings["FortyFive"].Replace('>', '\\');
                var outputDirPath = settings["OutputDir"].Replace('>', '\\');
                var outputFolder = await StorageFolder.GetFolderFromPathAsync(outputDirPath);


                FirestoreDb db = FirestoreDb.Create("prosthetichand-c0f57");

                foreach (var order in orders)
                {

                    if (!order.IsDone)
                    {
                        var orderId = order.Id;
                        var handSize = order.HandSize;
                        var handColor = order.HandColor;
                        var presetPath = "";

                        // copy the appropriate size from the preset folder
                        switch (handSize)
                        {
                            case "30":
                                presetPath = thirtyPath;
                                break;
                            case "35":
                                presetPath = thirtyFivePath;
                                break;
                            case "40":
                                presetPath = fortyPath;
                                break;
                            case "45":
                                presetPath = fortyFivePath;
                                break;
                        }

                        var presetModel = await StorageFile.GetFileFromPathAsync(presetPath);
                        await presetModel.CopyAsync(outputFolder, $"{orderId}.gltf", NameCollisionOption.ReplaceExisting);

                        // Write to Firestore with updated isDone value
                        DocumentReference docRef = db.Collection("Orders").Document(orderId);
                        await docRef.UpdateAsync(new Dictionary<string, object>
                        {
                            {"isDone", true }
                        });
                    }
                }
                ContentDialog dialog = new()
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Models Generated",
                    PrimaryButtonText = "View Models",
                    DefaultButton = ContentDialogButton.Primary,
                    CloseButtonText = "OK",
                    Content = "Press the Refresh button to view updated data",
                    PrimaryButtonCommand = new RelayCommand(async() => await Launcher.LaunchFolderAsync(outputFolder))
                };
                await dialog.ShowAsync();
                ProcessButton.IsEnabled = true;
                ProcessButtonText.Text = "Proess New Orders";
            }
            catch (Exception ex)
            {
                ContentDialog dialog = new()
                {
                    XamlRoot = this.XamlRoot,
                    Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    Title = "Error occured while generating models",
                    PrimaryButtonText = "Network Settings",
                    CloseButtonText = "Close App",
                    DefaultButton = ContentDialogButton.Primary,
                    Content = $"Some error occured while generating models.\n\nError Description: {ex.Message}",
                    PrimaryButtonCommand = new RelayCommand(OpenNetworkSettings),
                    CloseButtonCommand = new RelayCommand(() => Application.Current.Exit()),
                };
                dialog.Closing += (s, e) => Application.Current.Exit();
                await dialog.ShowAsync();
            }
        }
    }
}
