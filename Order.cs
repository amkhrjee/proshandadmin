using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Common;


namespace proshandadmin
{
    public class Order
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string HandSize { get; set; }
        public string HandColor { get; set; }
        public bool IsDone { get; set; }

        public static async Task<List<Order>> Orders()
        {
            FirestoreDb db = FirestoreDb.Create("prosthetichand-c0f57");
            CollectionReference usersRef = db.Collection("Orders");
            QuerySnapshot snapshot = await usersRef.GetSnapshotAsync();
            List<Order> data = new();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                Order order = new()
                {
                    Id = document.Id.ToString(),
                    FirstName = documentDictionary["FirstName"].ToString(),
                    LastName = documentDictionary["LastName"].ToString(),
                    Email = documentDictionary["Email"].ToString(),
                    Address = documentDictionary["Address"].ToString(),
                    Mobile = documentDictionary["Mobile"].ToString(),
                    HandColor = documentDictionary["HandColor"].ToString(),
                    HandSize = documentDictionary["HandSize"].ToString(),
                    IsDone = (bool)documentDictionary["isDone"]
                };

                data.Add(order);
            }
            return data;
        }
    }
}
