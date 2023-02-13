using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LaunchDarkly.InitializeTimeout.Demo
{
    public class LdEnvironment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string MobileSdkKey { get; set; }

        public string Name { get; set; }

    }
}
