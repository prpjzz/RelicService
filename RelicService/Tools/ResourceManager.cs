using RelicService.Data.Database;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelicService.Tools
{
    internal class ResourceManager(SqliteContext dbContext)
    {
        private class ImageEntry(Image image)
        {
            public Image Image { get; } = image;
            public int RefCount { get; set; } = 1; // Starts with one reference when created
        }

        private ConcurrentDictionary<uint, ImageEntry> _avatarImageMap = new();
        private ConcurrentDictionary<uint, ImageEntry> _relicImageMap = new();

        public async Task<Image?> GetAvatarImage(uint avatarId)
        {
            if (_avatarImageMap.TryGetValue(avatarId, out var entry))
            {
                entry.RefCount++; // Increment reference count
                return entry.Image;
            }

            var avatar = await dbContext.Avatars.FindAsync(avatarId);
            if (avatar == null)
            {
                return null;
            }

            var imageBase64 = avatar.IconBase64;
            var imageBytes = Convert.FromBase64String(imageBase64);
            using var ms = new MemoryStream(imageBytes);
            var image = Image.FromStream(ms);
            image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);

            var imageEntry = new ImageEntry(image);
            _avatarImageMap.TryAdd(avatarId, imageEntry);

            return image;
        }

        public async Task<Image?> GetRelicImage(uint relicId)
        {
            if (_relicImageMap.TryGetValue(relicId, out var entry))
            {
                entry.RefCount++; // Increment reference count
                return entry.Image;
            }

            var relic = await dbContext.Relics.FindAsync(relicId);
            if (relic == null)
            {
                return null;
            }

            var imageBase64 = relic.IconBase64;
            var imageBytes = Convert.FromBase64String(imageBase64);
            using var ms = new MemoryStream(imageBytes);
            var image = Image.FromStream(ms);
            image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            image.RotateFlip(RotateFlipType.RotateNoneFlipX);

            var imageEntry = new ImageEntry(image);
            _relicImageMap.TryAdd(relicId, imageEntry);

            return image;
        }

        public void FreeAvatarImage(uint avatarId)
        {
            if (_avatarImageMap.TryGetValue(avatarId, out var entry))
            {
                entry.RefCount--; // Decrement reference count

                if (entry.RefCount <= 0)
                {
                    // Remove and dispose the image when no more references exist
                    if (_avatarImageMap.TryRemove(avatarId, out var removedEntry))
                    {
                        removedEntry.Image.Dispose();
                    }
                }
            }

        }

        public void FreeRelicImage(uint relicId)
        {
            if (_relicImageMap.TryGetValue(relicId, out var entry))
            {
                entry.RefCount--; // Decrement reference count

                if (entry.RefCount <= 0)
                {
                    // Remove and dispose the image when no more references exist
                    if (_relicImageMap.TryRemove(relicId, out var removedEntry))
                    {
                        removedEntry.Image.Dispose();
                    }
                }
            }
        }

    }
}
