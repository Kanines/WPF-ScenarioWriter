using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ScenarioWriter
{
    public class Scenario : INotifyPropertyChanged
    {
        public int ScenarioNumber { get; set; }    
        public BindingList<Step> Steps { get; set; } 
        public string ScenarioDescription
        {
            get
            {
                return this._scenarioDescription;
            }
            set
            {
                if (value != this._scenarioDescription)
                {
                    this._scenarioDescription = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _scenarioDescription;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
