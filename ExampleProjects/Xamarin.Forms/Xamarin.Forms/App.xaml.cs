using System;
using Xamarin.Forms;
using Xamarin.Forms.Services;
using Xamarin.Forms.Views;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
