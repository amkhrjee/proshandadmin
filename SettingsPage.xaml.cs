using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;

namespace proshandadmin
{
    public sealed partial class SettingsPage : Page
    {
        Dictionary<string, string> settings = new();

        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder settingsFolder = ApplicationData.Current.LocalFolder;
            Debug.WriteLine(ApplicationData.Current.LocalFolder.Path);
            StorageFileQueryResult results = settingsFolder.CreateFileQuery();
            IReadOnlyList<StorageFile> filesInFolder = await results.GetFilesAsync();
            if (filesInFolder.Count > 0)
            {
                foreach (StorageFile item in filesInFolder)
                {
                    Debug.WriteLine(item.Name);
                    if (item.Name == "settings.json")
                    {
                        // Store the settings to the JSON
                        await FileIO.WriteTextAsync(item, $"{{Thirty: '{ThirtyDir.Text}', ThirtyFive: '{ThirtyFiveDir.Text}', Forty: '{FortyDir.Text}', FortyFive: '{FortyFiveDir.Text}', OutputDir: '{OutputDir.Text}'}}");
                    }
                }

            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //  Read the settings JSON and populate the settings page options
            StorageFolder settingsFolder = ApplicationData.Current.LocalFolder;
            Debug.WriteLine(ApplicationData.Current.LocalFolder.Path);
            StorageFileQueryResult results = settingsFolder.CreateFileQuery();
            IReadOnlyList<StorageFile> filesInFolder = await results.GetFilesAsync();
            if (filesInFolder.Count > 0)
            {
                foreach (StorageFile item in filesInFolder)
                {
                    Debug.WriteLine(item.Name);
                    if (item.Name == "settings.json")
                    {
                        // Export the options to a dictionary
                        Debug.WriteLine("Found the settings file!");
                        string json = File.ReadAllText(item.Path);
                        settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        var thirtyPath = settings["Thirty"];
                        var thirtyFivePath = settings["ThirtyFive"];
                        var fortyPath = settings["Forty"];
                        var fortyFivePath = settings["FortyFive"];
                        var outputDirPath = settings["OutputDir"];

                        if (thirtyPath.Length > 0)
                        {
                            ThirtyDir.Text = thirtyPath;
                        }
                        if (thirtyFivePath.Length > 0)
                        {
                            ThirtyFiveDir.Text = thirtyFivePath;
                        }
                        if (fortyPath.Length > 0)
                        {
                            FortyDir.Text = fortyPath;
                        }
                        if (fortyFivePath.Length > 0)
                        {
                            FortyFiveDir.Text = fortyFivePath;
                        }
                        if (fortyFivePath.Length > 0)
                        {
                            FortyFiveDir.Text = fortyFivePath;
                        }
                        if (outputDirPath.Length > 0)
                        {
                            OutputDir.Text = outputDirPath;
                        }

                    }
                }
            }
            else
            {
                // Create the file
                StorageFile settingsFile = await settingsFolder.CreateFileAsync("settings.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(settingsFile, "{Thirty: '', ThirtyFive: '', Forty: '', FortyFive: '', OutputDir: ''}");
            }

        }
    }
}
