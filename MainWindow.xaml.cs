using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace proshandadmin
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.Title = "Prosthetic Hand Store Admin";
            this.ExtendsContentIntoTitleBar = true;

            this.InitializeComponent();
        }

        private void handleSelectionChange(NavigationView sender,
                                        NavigationViewSelectionChangedEventArgs args)
        {

            contentFrame.SourcePageType = typeof(MainPage);
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (selectedItem.Name == navMainPage.Name)
            {
                contentFrame.SourcePageType = typeof(MainPage);
            }
            else if (selectedItem.Name == navOrdersPage.Name)
            {
                contentFrame.SourcePageType = typeof(OrdersPage);
            }

        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.SourcePageType = typeof(MainPage);
            NavView.SelectedItem = navMainPage;
        }
    }
}
