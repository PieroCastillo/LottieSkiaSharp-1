using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LottieSkiaSharp.Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            load();

        }
        async void load()
        {
#if WINDOWS_UWP
            v.FileName = (await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Assets\\lottiefiles.com - ATM.json")).Path;
#endif
            v.UseHardwareAcceleration(false);
            v.EnableMergePaths(true);
            
            //v.UseExperimentalHardwareAcceleration()
            v.PlayAnimation();
        }
    }
}
