using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using RelicService.Data.Database;

namespace RelicService.Tools;

internal class ResourceManager(SqliteContext dbContext)
{
	private class ImageEntry(Image image)
	{
		public Image Image { get; } = image;

		public int RefCount { get; set; } = 1;
	}

	private ConcurrentDictionary<uint, ImageEntry> _avatarImageMap = new ConcurrentDictionary<uint, ImageEntry>();

	private ConcurrentDictionary<uint, ImageEntry> _relicImageMap = new ConcurrentDictionary<uint, ImageEntry>();

	public async Task<Image?> GetAvatarImage(uint avatarId)
	{
		if (_avatarImageMap.TryGetValue(avatarId, out ImageEntry value))
		{
			value.RefCount++;
			return value.Image;
		}
		DbAvatar dbAvatar = await dbContext.Avatars.FindAsync(avatarId);
		if (dbAvatar == null)
		{
			return null;
		}
		using MemoryStream stream = new MemoryStream(Convert.FromBase64String(dbAvatar.IconBase64));
		Image image = Image.FromStream(stream);
		image.RotateFlip(RotateFlipType.Rotate180FlipNone);
		image.RotateFlip(RotateFlipType.RotateNoneFlipX);
		ImageEntry value2 = new ImageEntry(image);
		_avatarImageMap.TryAdd(avatarId, value2);
		return image;
	}

	public async Task<Image?> GetRelicImage(uint relicId)
	{
		if (_relicImageMap.TryGetValue(relicId, out ImageEntry value))
		{
			value.RefCount++;
			return value.Image;
		}
		DbRelic dbRelic = await dbContext.Relics.FindAsync(relicId);
		if (dbRelic == null)
		{
			return null;
		}
		using MemoryStream stream = new MemoryStream(Convert.FromBase64String(dbRelic.IconBase64));
		Image image = Image.FromStream(stream);
		image.RotateFlip(RotateFlipType.Rotate180FlipNone);
		image.RotateFlip(RotateFlipType.RotateNoneFlipX);
		ImageEntry value2 = new ImageEntry(image);
		_relicImageMap.TryAdd(relicId, value2);
		return image;
	}

	public void FreeAvatarImage(uint avatarId)
	{
		if (_avatarImageMap.TryGetValue(avatarId, out ImageEntry value))
		{
			value.RefCount--;
			if (value.RefCount <= 0 && _avatarImageMap.TryRemove(avatarId, out ImageEntry value2))
			{
				value2.Image.Dispose();
			}
		}
	}

	public void FreeRelicImage(uint relicId)
	{
		if (_relicImageMap.TryGetValue(relicId, out ImageEntry value))
		{
			value.RefCount--;
			if (value.RefCount <= 0 && _relicImageMap.TryRemove(relicId, out ImageEntry value2))
			{
				value2.Image.Dispose();
			}
		}
	}
}
