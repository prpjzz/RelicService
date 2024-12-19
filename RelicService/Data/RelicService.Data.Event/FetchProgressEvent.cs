using RelicService.Data.Config;

namespace RelicService.Data.Event;

internal class FetchProgressEvent
{
	public FetchType Type { get; set; }

	public uint Current { get; set; }

	public uint Total { get; set; }

	public string Name => Type switch
	{
		FetchType.AvatarMetadata => "角色元数据", 
		FetchType.AvatarResource => "角色资源", 
		FetchType.RelicMetadata => "圣遗物元数据", 
		FetchType.RelicResource => "圣遗物资源", 
		_ => "未知", 
	};

	public FetchProgressEvent(FetchType type, uint current, uint total)
	{
		Type = type;
		Current = current;
		Total = total;
	}
}
