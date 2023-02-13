using LaunchDarkly.Sdk;
using LaunchDarkly.Sdk.Client;
using LaunchDarkly.Sdk.Client.Interfaces;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LaunchDarkly.InitializeTimeout.Demo
{
    public partial class App : Application
    {
        public static ILdClient LaunchDarklyClient { get; private set; }

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
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
