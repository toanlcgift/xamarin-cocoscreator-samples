using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GameStoreAndHotUpdateSample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;
            await LaunchGame("game1.zip");
            (sender as Button).IsEnabled = true;
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;
            await LaunchGame("game2.zip");
            (sender as Button).IsEnabled = true;
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            (sender as Button).IsEnabled = false;
            await LaunchGame("game3.zip");
            (sender as Button).IsEnabled = true;
        }

        async Task LaunchGame(string zipFileName)
        {
            var check = await PCLStorage.FileSystem.Current.LocalStorage.CheckExistsAsync("game");
            if (check != PCLStorage.ExistenceCheckResult.FolderExists)
                await PCLStorage.FileSystem.Current.LocalStorage.CreateFolderAsync("game", PCLStorage.CreationCollisionOption.ReplaceExisting);

            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GameStoreAndHotUpdateSample." + zipFileName);
            var file = await PCLStorage.FileSystem.Current.LocalStorage.CreateFileAsync("game.zip", PCLStorage.CreationCollisionOption.ReplaceExisting);
            var folder = await PCLStorage.FileSystem.Current.LocalStorage.GetFolderAsync("game");
            var path = folder.Path;

            using (var gamefile = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
            {
                await stream.CopyToAsync(gamefile);
                gamefile.Close();
            }
            await UnzipFileAsync(file.Path, path);

            var script = File.ReadAllText(Path.Combine(path, "main.js"));
            script = script.Replace("src/", Path.Combine(path, "src") + "/");
            script = script.Replace("res/", Path.Combine(path, "res") + "/");
            script = script.Replace("jsb-adapter", Path.Combine(path, "jsb-adapter"));

            App.Current.MainPage = new NavigationPage(new GamePage(script));
        }

        private async Task<bool> UnzipFileAsync(string zipFilePath, string unzipFolderPath)
        {
            try
            {
                var entry = new ZipEntry(Path.GetFileNameWithoutExtension(zipFilePath));
                var fileStreamIn = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);
                var zipInStream = new ZipInputStream(fileStreamIn);
                entry = zipInStream.GetNextEntry();
                while (entry != null && entry.CanDecompress)
                {
                    var outputFile = unzipFolderPath + @"/" + entry.Name;
                    var outputDirectory = Path.GetDirectoryName(outputFile);
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    if (entry.IsFile)
                    {
                        var fileStreamOut = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                        int size;
                        byte[] buffer = new byte[4096];
                        do
                        {
                            size = await zipInStream.ReadAsync(buffer, 0, buffer.Length);
                            await fileStreamOut.WriteAsync(buffer, 0, size);
                        } while (size > 0);
                        fileStreamOut.Close();
                    }

                    entry = zipInStream.GetNextEntry();
                }
                zipInStream.Close();
                fileStreamIn.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
