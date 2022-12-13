using System.ComponentModel;

namespace ClientApp.ViewModels
{
    /*
    public class ViewModelBase : ReactiveObject
    {
    }
    */


    public class ViewModelBase : INotifyPropertyChanged//,IControl
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void Dispose() { }
        
    }
    
}
