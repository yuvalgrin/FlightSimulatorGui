﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorGui.ViewModel
{
    public class ControlRoomViewModel : BaseNotify
    {
        public ControlRoomViewModel()
        {
            model.PropertyChanged +=
                delegate (Object sender, PropertyChangedEventArgs e) {
                    NotifyPropertyChanged("VM_" + e.PropertyName);
                };
        }

        public String VM_QueryRes
        {
            get { return model.QueryRes; }
            set
            {
                NotifyPropertyChanged("VM_QueryRes");
            }
        }

        public void queryUpdate(String query)
        {
            VM_QueryRes = "Loading...";
            model.executeCtrlRoomQuery(query);
        }
    }
}