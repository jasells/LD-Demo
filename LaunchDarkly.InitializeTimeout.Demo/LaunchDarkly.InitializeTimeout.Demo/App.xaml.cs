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

            _ = InitLD();

            MainPage = new MainPage();
        }

        private static async Task<ILdClient> InitLD()
        {
            const string key = "mob-250c78a9-79a9-40b6-9b05-329ce8b2295e";

            var context = Context.Builder("anon demo user")
                                 .Anonymous(true)
                                 .Set("paltform", Device.RuntimePlatform)
                                 .Build();

            var timeout = TimeSpan.FromMilliseconds(5000);

            var result = new AsyncResult<ILdClient>();

            var client = await InitializeLdClient(key, context, result, timeout);

            return client;
        }

        private static Task<ILdClient> InitializeLdClient(string mobileKey, Context context, AsyncResult<ILdClient> result, TimeSpan timeoutSpan) =>
            System.Threading.Tasks.Task.Run(() =>
            {
                if (LaunchDarklyClient == null)
                {
                    var client = LdClient.Init(mobileKey, context, timeoutSpan);

                    if (!client.Initialized)
                    {
                        var errorInfo = client.DataSourceStatusProvider.Status.LastError?.ToString()
                                        ??
                                        client.DataSourceStatusProvider.Status.ToString();
                        result.Error = new Exception(errorInfo);
                    }

                    LaunchDarklyClient = client;
                }
                return LaunchDarklyClient;
            });

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
