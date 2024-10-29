using DriverSettlements.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriverSettlements.App.Helpers
{
    public class SelectableDriverWrapper:Driver, ISelectable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _selected = false;
        private readonly Driver _driver;
        public SelectableDriverWrapper(Driver driver)
        {
            _driver = driver;
            Id = driver.Id;
            CompanyName = driver.CompanyName;
            BoltName = driver.BoltName;
            UberName = driver.UberName;
            FreenowName = driver.FreenowName;
        }
        public bool Selected 
        {
            get => _selected;
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged("Selected");
                }
            }
        }

        public Driver Driver => _driver;

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
