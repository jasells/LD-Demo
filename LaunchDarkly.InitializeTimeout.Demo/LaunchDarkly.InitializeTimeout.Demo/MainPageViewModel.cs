using LaunchDarkly.Sdk.Client.Interfaces;
using LaunchDarkly.Sdk.Client;
using LaunchDarkly.Sdk;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.ComponentModel;

namespace LaunchDarkly.InitializeTimeout.Demo
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ILdClient? LaunchDarklyClient { get; private set; } = null;

        public bool IsInitializedVisible { get; set; }

        public MainPageViewModel()
        {
            IsInitializedVisible = LaunchDarklyClient?.Initialized ?? false;

            _ = InitLD();
        }

        private async Task<ILdClient> InitLD()
        {
            const string key = "mob-250c78a9-79a9-40b6-9b05-329ce8b2295e";

            var context = Context.Builder("anon demo user")
                                 .Anonymous(true)
                                 .Set("paltform", Device.RuntimePlatform)
                                 .Build();

            var timeout = TimeSpan.FromMilliseconds(5000);

            var result = new AsyncResult<ILdClient>();

            var client = await InitializeLdClient(key, context, result, timeout);

            IsInitializedVisible = client.Initialized;

            return client;
        }

        private Task<ILdClient> InitializeLdClient(string mobileKey, Context context, AsyncResult<ILdClient> result, TimeSpan timeoutSpan) =>
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
    }
}
