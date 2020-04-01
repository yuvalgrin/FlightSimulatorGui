﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorGui.ViewModel
{
    class ControlRoomViewModel : BaseNotify
    {
        String VM_QueryRes
        {
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
