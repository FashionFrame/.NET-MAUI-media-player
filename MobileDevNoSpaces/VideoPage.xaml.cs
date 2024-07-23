using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace MobileDevNoSpaces;

public partial class VideoPage : ContentPage
{
    MediaSource media;

    public VideoPage(MediaSource media)
    {
        this.media = media;
        InitializeComponent();
        mediaElement.Source = media;
        play_pause.Opacity=0;
        vol.Opacity=0;
        stop.Opacity=0;
        warn.Opacity=0;
    }

    void ContentPage_Unloaded(object? sender, EventArgs e)
    {
        // Stop and cleanup MediaElement when we navigate away
        mediaElement.Handler?.DisconnectHandler();
    }

    void OnPlayPauseButtonClicked(object? sender, EventArgs e)
    {
        if (mediaElement.CurrentState == MediaElementState.Stopped ||
        mediaElement.CurrentState == MediaElementState.Paused)
        {
            mediaElement.Play();
        }
        else if (mediaElement.CurrentState == MediaElementState.Playing)
        {
            mediaElement.Pause();
        }
    }

    void OnStopButtonClicked(object? sender, EventArgs e)
    {
        mediaElement.Stop();
    }

    private void Volume_DragCompleted(object sender, EventArgs e)
    {
        mediaElement.Volume = vol.Value;
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await play_pause.FadeTo(1, 50);
        await vol.FadeTo(1, 50);
        await stop.FadeTo(1, 50);
        await Task.Delay(6000);
        await play_pause.FadeTo(0, 50);
        await vol.FadeTo(0, 50);
        await stop.FadeTo(0, 50);
    }

    private async void mediaElement_MediaFailed(object sender, MediaFailedEventArgs e)
    {
        await warn.FadeTo(1, 50);
    }
}
