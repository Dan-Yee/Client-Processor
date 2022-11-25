using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Models
{
    public class ListOfFieldsModel : IObservable<string>
    {
        public string Name { get; set; }
        
        private List<IObserver<string>> observers;
        
        public ListOfFieldsModel(string name)
        {
            Name = name;
            observers = new List<IObserver<string>>();
            
        }

        public IDisposable Subscribe(IObserver<string> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("title", "Object: " + Name).Show();
            return new MyField(observers, observer);
            //throw new NotImplementedException();
        }
        private class MyField: IDisposable
        {
            private List<IObserver<string>> _observers;
            private IObserver<string> _observer;

            public MyField(List<IObserver<string>> observers, IObserver<string> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }


}
