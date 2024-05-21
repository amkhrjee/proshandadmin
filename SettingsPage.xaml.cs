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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
                        Debug.WriteLine("Writing to file");
                        // Store the settings to the JSON
                        await FileIO.WriteTextAsync(item, $"{{Thirty: '{ThirtyDir.Text.Replace('\\', '>')}', ThirtyFive: '{ThirtyFiveDir.Text.Replace('\\', '>')}', Forty: '{FortyDir.Text.Replace('\\', '>')}', FortyFive: '{FortyFiveDir.Text.Replace('\\', '>')}', OutputDir: '{OutputDir.Text.Replace('\\', '>')}'}}");
                    }
                }

            }

            SaveButtonIcon.Glyph = "\uE73E";
            SaveButtonText.Text = "Saved";
            await Task.Delay(500);
            SaveButtonIcon.Glyph = "\uE74E";
            SaveButtonText.Text = "Save";

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
                        var thirtyPath = settings["Thirty"].Replace('>', '\\');
                        var thirtyFivePath = settings["ThirtyFive"].Replace('>', '\\');
                        var fortyPath = settings["Forty"].Replace('>', '\\');
                        var fortyFivePath = settings["FortyFive"].Replace('>', '\\');
                        var outputDirPath = settings["OutputDir"].Replace('>', '\\');

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

        private static async void SetFilePath(TextBox textBox)
        {
            var openPicker = new FileOpenPicker();
            var window = App.m_window;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add(".gltf");
            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                textBox.Text = file.Path;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetFilePath(ThirtyDir);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SetFilePath(ThirtyFiveDir);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SetFilePath(FortyDir);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SetFilePath(FortyFiveDir);
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var openPicker = new FolderPicker();
            var window = App.m_window;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add("*");

            var folder = await openPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                OutputDir.Text = folder.Path;
            }

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            foreach(var textBox in new List<TextBox>{ThirtyDir, ThirtyFiveDir, FortyDir, FortyFiveDir, OutputDir})
            {
                textBox.Text = "";
            }
        }
    }
}
