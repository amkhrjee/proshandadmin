using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;

using Google.Cloud.Firestore;
using System.Threading.Tasks;
using Microsoft.UI.Text;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace proshandadmin
{
    public sealed partial class OrdersPage : Page
    {
        public OrdersPage()
        {
            this.InitializeComponent();
            LoadData();

        }

        private async void LoadData()
        {
            FirestoreDb db = FirestoreDb.Create("prosthetichand-c0f57");
            CollectionReference usersRef = db.Collection("Orders");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
            List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                data.Add(documentDictionary);
            }
            foreach (var document in data)
            {
                var stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 60,
                    CornerRadius = new CornerRadius(8),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Padding = new Thickness(16)
                };
                itemsContainer.Children.Add(stackPanel);

                // The TextBlocks
                foreach (KeyValuePair<string, object> item in document)
                {
                    var textBlock = new TextBlock
                    {
                        Text = item.Value.ToString()
                    };
                    stackPanel.Children.Add(textBlock);
                }
            }
        }
    }
}
