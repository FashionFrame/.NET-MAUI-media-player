using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MobileDevNoSpaces;

public partial class Nav : ContentPage
{
    public ObservableCollection<Model> Media { get; set; }

    public Nav()
	{
		InitializeComponent();
        Media = new ObservableCollection<Model>(){};
        LoadList();
        BindingContext = this;
        if (MediaPicker.Default.IsCaptureSupported)
        {
            Button newButton = new Button { Text = "Take Video", HorizontalOptions = LayoutOptions.Fill};
            newButton.Clicked += async (sender, args) => TakeVideo();
            navGrid.Add(newButton, 0,1);

        }
    }

    public async void SaveList()
    {
        string jsonString = JsonSerializer.Serialize(Media);
        string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "list.json");
        using (FileStream fs = File.Create(targetFile))
        {
            byte[] info = new UTF8Encoding(true).GetBytes(jsonString);
            fs.Write(info, 0, info.Length);
        }
    }

    public void LoadList()
    {
        try
        {
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "list.json");
            using (StreamReader r = new StreamReader(targetFile))
            {
                string json = r.ReadToEnd();
                Media = JsonSerializer.Deserialize<ObservableCollection<Model>>(json);
            }
        }catch (Exception ex)
        {

        }
    }

    public async void ShareFile(object sender, EventArgs e)
    {
        MenuItem menuItem = sender as MenuItem;
        var contextItem = menuItem.BindingContext;
        Model item = contextItem as Model;
        string file = item.FilePath;
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            File = new ShareFile(file, item.Type)
        });
    }

    public async void TakeVideo()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CaptureVideoAsync();

            if (photo != null)
            {
                // save the file into local storage
                string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);

                await sourceStream.CopyToAsync(localFileStream);

                Media.Add(new Model() { Name = photo.FileName, Type = photo.ContentType, FilePath = photo.FullPath });
            }
            else
            {
                await DisplayAlert("Alert", "Please take a video", "OK");
            }
        }
    }


    public void DeleteItem(object sender, EventArgs e)
    {
        MenuItem menuItem = sender as MenuItem;

        // Access the list item through the BindingContext
        var contextItem = menuItem.BindingContext;
        Model item = contextItem as Model;
        Media.Remove(item);
        SaveList();
    }

    async void NextPageNavigation(object sender, ItemTappedEventArgs args)
    {
        Model item = args.Item as Model;
        MediaSource newMediaSource = MediaSource.FromFile(item.FilePath);
        await Navigation.PushAsync(new VideoPage(newMediaSource), true);
        list.SelectedItem = null;
    }

    private async void GetMedia(object sender, EventArgs e)
    {
        var customFileType = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                    { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                    { DevicePlatform.Android, new[] { "video/mp4", "audio/mpeg", "video/webm", "audio/wav", "audio/webm" } }, // MIME types
                    { DevicePlatform.WinUI, new[] { ".mp3", ".mp4", ".wav", ".webm", } }, // file extension
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "cbr", "cbz" } }, // UTType values
            });
        PickOptions options = new()
        {
            PickerTitle = "Please select a media file",
            FileTypes = customFileType
        };
        try
        {
            var file = await PickAndShow(options);
            MediaSource newMediaSource = MediaSource.FromFile(file.FullPath);
            Media.Add(new Model() { Name = file.FileName, Type = file.ContentType, FilePath = file.FullPath });
            SaveList();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Alert","Please select a file", "OK");
        }
    }

    public async Task<FileResult> PickAndShow(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                {
                    using var stream = await result.OpenReadAsync();
                    var image = ImageSource.FromStream(() => stream);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
            await DisplayAlert("Alert", "Please select a file", "OK");
        }
        return null;
    }
}

public class Model
{
    public string Name { get; set; }
    public string FilePath { get; set; }
    public string Type { get; set; }
}