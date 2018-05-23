using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelCalendar
{
    public class SimpleException
    {
        public string Message;
        public string StackTrace;
        public SimpleException(Exception exc)
        {
            Message = exc.Message;
            StackTrace = exc.StackTrace;
        }
    }
}
