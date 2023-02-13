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

        public bool EnablePickers { get; set; } = true;

        public string LdInitDisplay { get; set; } = "LaunchDarkly not available";

        public string Flag1Display { get; set; } = "Flag1 not available";

        public string LastError { get; set; } = string.Empty;

        public IList<Project> Projects { get; set; }

        public Project SelectedProject { get; set; }

        public LdEnvironment? SelectedEnvironment { get; set; }

        public MainPageViewModel()
        {
            IsInitializedVisible = LaunchDarklyClient?.Initialized ?? false;

            PropertyChanged += MainPageViewModel_PropertyChanged;

            var projects = new Project[]
            {

                new Project
                {
                    Name = "Demo Project1", //JASells-Playground
                    Environments = new LdEnvironment[]
                    {
                        new LdEnvironment
                        {
                            Name = "Test",
                            MobileSdkKey="mob-250c78a9-79a9-40b6-9b05-329ce8b2295e",
                        },
                        new LdEnvironment
                        {
                            Name = "Production",
                            MobileSdkKey="mob-eafac57c-a768-4e15-960d-2ef1cb585bb0",
                        },
                    },
                },
            };

            Projects = projects;

            //InitLD(projects[0].Environments[0].MobileSdkKey).ContinueWith(_ =>  SetDisplayText());
        }

        private async void MainPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(SelectedProject)))
            {
                if (SelectedProject != null)
                {
                    CleanUpLdClient();
                    ResetUI();
                    SelectedEnvironment = null;
                }
            }
            else if (e.PropertyName.Equals(nameof(SelectedEnvironment)))
            {
                if (SelectedEnvironment != null)
                {
                    CleanUpLdClient();
                    ResetUI();

                    EnablePickers = false;
                    var result = 
                    await InitLD(SelectedEnvironment.MobileSdkKey).ConfigureAwait(false);
                    
                    if (result.Success)
                    {
                        LaunchDarklyClient!.FlagTracker.FlagValueChanged += LD_FlagValueChanged;
                    }

                    SetDisplayText(result);

                    EnablePickers = true;
                }
            }
        }

        private void ResetUI()
        {
            LdInitDisplay = "LaunchDarkly not available";
            Flag1Display = $"TestFlag1 not available";
            LastError = string.Empty;
        }

        private void CleanUpLdClient()
        {
            if (LaunchDarklyClient != null)
            {
                LaunchDarklyClient.FlagTracker.FlagValueChanged -= LD_FlagValueChanged;
            }

            LaunchDarklyClient?.Dispose();
            LaunchDarklyClient = null;
        }

        private void LD_FlagValueChanged(object sender, FlagValueChangeEvent e)
        {
            if (e.Key.Equals(_flag1Key))
            {
                Flag1Display = $"TestFlag1 evaluated as: '{e.NewValue.AsBool}'";
            }
        }

        private void SetDisplayText(AsyncResult<ILdClient> result)
        {
            LdInitDisplay = LaunchDarklyClient != null
                           ? $"LaunchDarkly initialized: {result.Result?.Initialized ?? false}"
                           : LdInitDisplay;

            Flag1Display = LaunchDarklyClient != null
                           ? $"TestFlag1 evaluated as: '{LaunchDarklyClient.BoolVariation(_flag1Key, false)}'"
                           : Flag1Display;
            
            string? err = result.Error?.Message ?? null;
            LastError = err == null ? string.Empty : $"LD error: {err}";

        }

        private async Task<AsyncResult<ILdClient>> InitLD(string key)
        {
            //const string key = "mob-250c78a9-79a9-40b6-9b05-329ce8b2295e";

            var context = Context.Builder("anon demo user")
                                 .Anonymous(true)
                                 .Set("paltform", Device.RuntimePlatform)
                                 .Build();

            var timeout = TimeSpan.FromMilliseconds(5000);

            var result = new AsyncResult<ILdClient>();

            var client = await InitializeLdClient(key, context, result, timeout);

            result.Result = client;

            return result;
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

        private const string _flag1Key = "testFlag1";
    }
}
