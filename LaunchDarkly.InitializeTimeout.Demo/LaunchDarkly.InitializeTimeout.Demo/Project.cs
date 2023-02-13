using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LaunchDarkly.InitializeTimeout.Demo
{
    public class Project : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
     
        public string Name { get; set; }

        public IList<LdEnvironment> Environments { get; set; }

    }
}
