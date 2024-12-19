namespace RelicService.Data.Event;

internal enum EventId
{
	EvtNone,
	EvtUidChanged,
	EvtSceneIdChanged,
	EvtServiceStatusChanged,
	EvtShutdown,
	EvtFetchProgress,
	EvtProfileRefresh,
	EvtProfileConflict
}
