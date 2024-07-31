using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_PostJsonEvent
{
    [Serializable]
    public class EventDTO
    {
        public string type;
        public string data;
    }
}
