﻿using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MobileDevNoSpaces
{
    [Activity(Theme = "@style/Maui.SplashTheme",NoHistory =false , MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
