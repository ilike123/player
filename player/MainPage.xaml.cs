using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Web.Http;




// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace player
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await SetLocalMedia();
        }

        async private System.Threading.Tasks.Task SetLocalMedia()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".mp3");

            var file = await openPicker.PickSingleFileAsync();

            // mediaPlayer is a MediaElement defined in XAML
            if (file != null)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                player.SetSource(stream, file.ContentType);

                player.Play();
            }
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //停止播放，并且重头开始
            player.Stop();
        }

        private void txtFilePath_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                TextBox tbPath = sender as TextBox; if (tbPath != null)
                {
                    LoadMediaFromString(tbPath.Text);
                }
            }
        }
        private void LoadMediaFromString(string path)
        {
            try
            {
                Uri pathUri = new Uri(path);
                player.Source = pathUri;
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                }
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TextBox path = txtFilePath;
            if (path != null)
            {
                LoadMediaFromString(path.Text);
            }
            await Load();
            text.Text = "下载完成";
        }
        public async Task<StorageFile> Load()
        {
            try
            {
                var httpClient = new HttpClient();
                var uuri = new Uri(txtFilePath.Text);
                var buffer = await httpClient.GetBufferAsync(uuri);
                if (buffer != null && buffer.Length > 0u)
                {
                    string str = "NewSong.mp3";
                    StorageFolder simplefile = KnownFolders.MusicLibrary;
                    if (txtFilePath.Text.EndsWith(".mp3"))
                    {
                        str = "NewSong.mp3";
                        simplefile = KnownFolders.MusicLibrary;
                    }
                 
                    StorageFile file = await simplefile.CreateFileAsync(str, CreationCollisionOption.GenerateUniqueName);//在指定文件夹中创建文件，并且在文件夹中若已有该文件，那么文件名加数字后缀
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await stream.WriteAsync(buffer);
                        await stream.FlushAsync();
                    }
                    return file;
                  
                }
            }
            catch { }
            return null;
        }
    }
}
