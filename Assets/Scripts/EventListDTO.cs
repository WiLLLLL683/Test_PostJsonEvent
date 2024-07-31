using System;
using System.Collections.Generic;
using System.Linq;

namespace Test_PostJsonEvent
{
    [Serializable]
    public class EventListDTO
    {
        public List<EventDTO> events = new();
    }
}
