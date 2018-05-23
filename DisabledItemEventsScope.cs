using Microsoft.SharePoint;
using System;

namespace TravelCalendar
{

    public class DisabledItemEventsScope : SPItemEventReceiver, IDisposable
    {
        bool oldValue;
        public DisabledItemEventsScope()
        {
            this.oldValue = base.EventFiringEnabled;
            base.EventFiringEnabled = false;
        }
        public void Dispose()
        {
            base.EventFiringEnabled = oldValue;
        }
    }
}
