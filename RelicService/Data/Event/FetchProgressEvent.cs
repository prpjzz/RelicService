using RelicService.Data.Config;

namespace RelicService.Data.Event;

internal class FetchProgressEvent
{
	public FetchType Type { get; set; }

	public uint Current { get; set; }

	public uint Total { get; set; }

	public string Name => Type switch
	{
		FetchType.AvatarMetadata => "Character Data", 
		FetchType.AvatarResource => "Character Resource", 
		FetchType.RelicMetadata => "Artifact Data", 
		FetchType.RelicResource => "Artifact Resource", 
		_ => "None", 
	};

	public FetchProgressEvent(FetchType type, uint current, uint total)
	{
		Type = type;
		Current = current;
		Total = total;
	}
}
