// Decompiled with JetBrains decompiler
// Type: RelicService.Tools.ResourceManager
// Assembly: RelicService, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EA9BEB7B-7841-4D0A-A232-DCAF9A27085B
// Assembly location: RelicService.dll inside C:\Users\MBAINT\Downloads\win-x64\RelicService.exe)

using RelicService.Data.Database;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

#nullable enable
namespace RelicService.Tools
{
  internal class ResourceManager
  {
    private ConcurrentDictionary<uint, ResourceManager.ImageEntry> _avatarImageMap;
    private ConcurrentDictionary<uint, ResourceManager.ImageEntry> _relicImageMap;

    public ResourceManager(SqliteContext dbContext)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CdbContext\u003EP = dbContext;
      this._avatarImageMap = new ConcurrentDictionary<uint, ResourceManager.ImageEntry>();
      this._relicImageMap = new ConcurrentDictionary<uint, ResourceManager.ImageEntry>();
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public async Task<Image?> GetAvatarImage(uint avatarId)
    {
      ResourceManager.ImageEntry imageEntry1;
      if (this._avatarImageMap.TryGetValue(avatarId, ref imageEntry1))
      {
        ++imageEntry1.RefCount;
        return imageEntry1.Image;
      }
      // ISSUE: reference to a compiler-generated field
      DbAvatar async = await this.\u003CdbContext\u003EP.Avatars.FindAsync((object) avatarId);
      if (async == null)
        return (Image) null;
      using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(async.IconBase64)))
      {
        Image image = Image.FromStream((Stream) memoryStream);
        image.RotateFlip((RotateFlipType) 2);
        image.RotateFlip((RotateFlipType) 4);
        ResourceManager.ImageEntry imageEntry2 = new ResourceManager.ImageEntry(image);
        this._avatarImageMap.TryAdd(avatarId, imageEntry2);
        return image;
      }
    }

    public async Task<Image?> GetRelicImage(uint relicId)
    {
      ResourceManager.ImageEntry imageEntry1;
      if (this._relicImageMap.TryGetValue(relicId, ref imageEntry1))
      {
        ++imageEntry1.RefCount;
        return imageEntry1.Image;
      }
      // ISSUE: reference to a compiler-generated field
      DbRelic async = await this.\u003CdbContext\u003EP.Relics.FindAsync((object) relicId);
      if (async == null)
        return (Image) null;
      using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(async.IconBase64)))
      {
        Image image = Image.FromStream((Stream) memoryStream);
        image.RotateFlip((RotateFlipType) 2);
        image.RotateFlip((RotateFlipType) 4);
        ResourceManager.ImageEntry imageEntry2 = new ResourceManager.ImageEntry(image);
        this._relicImageMap.TryAdd(relicId, imageEntry2);
        return image;
      }
    }

    public void FreeAvatarImage(uint avatarId)
    {
      ResourceManager.ImageEntry imageEntry1;
      if (!this._avatarImageMap.TryGetValue(avatarId, ref imageEntry1))
        return;
      --imageEntry1.RefCount;
      ResourceManager.ImageEntry imageEntry2;
      if (imageEntry1.RefCount > 0 || !this._avatarImageMap.TryRemove(avatarId, ref imageEntry2))
        return;
      imageEntry2.Image.Dispose();
    }

    public void FreeRelicImage(uint relicId)
    {
      ResourceManager.ImageEntry imageEntry1;
      if (!this._relicImageMap.TryGetValue(relicId, ref imageEntry1))
        return;
      --imageEntry1.RefCount;
      ResourceManager.ImageEntry imageEntry2;
      if (imageEntry1.RefCount > 0 || !this._relicImageMap.TryRemove(relicId, ref imageEntry2))
        return;
      imageEntry2.Image.Dispose();
    }

    private class ImageEntry
    {
      public ImageEntry(Image image)
      {
        this.Image = image;
        // ISSUE: reference to a compiler-generated field
        this.\u003CRefCount\u003Ek__BackingField = 1;
        // ISSUE: explicit constructor call
        base.\u002Ector();
      }

      public Image Image { get; }

      public int RefCount { get; set; }
    }
  }
}
