using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ScenarioWriter
{
    public class Step : INotifyPropertyChanged
    {
        public object Parent { get; set; }
        public int StepNumber { get; set; }       
        public bool IsConditional { get; set; }
        public string FulfilledConditionalFlow { get; set; }
        public string FulfilledTarget { get; set; }
        public string UnfulfilledConditionalFlow { get; set; }
        public string UnfulfilledTarget { get; set; }
        public BindingList<Step> SubSteps { get; set; }
        public string StepDescription
        {
            get
            {
                return this._stepDescription;
            }
            set
            {
                if (value != this._stepDescription)
                {
                    this._stepDescription = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ActionName
        {
            get
            {
                return this._actionName;
            }
            set
            {
                if (value != this._actionName)
                {
                    this._actionName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _stepDescription;
        private string _actionName;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
