using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Data.Event
{
    internal enum EventId
    {
        EvtNone = 0,
        EvtUidChanged = 1,
        EvtSceneIdChanged = 2,
        EvtServiceStatusChanged = 3,
        EvtShutdown = 4,
        EvtFetchProgress = 5,
        EvtProfileRefresh = 6,
        EvtProfileConflict = 7,
    }

}
