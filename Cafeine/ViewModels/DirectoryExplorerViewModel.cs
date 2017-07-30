using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafeine.Model;
namespace Cafeine.ViewModels
{
    public class DirectoryExplorerViewModel
    {
        public static List<VirtualDirectory> DefaultDirectory(VirtualDirectory e)
        {
            List<VirtualDirectory> Dir = new List<VirtualDirectory>();
            try
            {
                switch (e.DirectoryType)
                {
                    case 1:
                        {
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.anime, DirectoryType = 4, DirectoryTitle = "Watching" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.anime, DirectoryType = 6, DirectoryTitle = "On Hold" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.anime, DirectoryType = 9, DirectoryTitle = "Planned to Watch" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.anime, DirectoryType = 5, DirectoryTitle = "Completed" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.anime, DirectoryType = 7, DirectoryTitle = "Dropped" });
                            break;
                        }
                    case 2:
                        {
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.manga, DirectoryType = 4, DirectoryTitle = "Reading" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.manga, DirectoryType = 6, DirectoryTitle = "On Hold" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.manga, DirectoryType = 9, DirectoryTitle = "Planned to Read" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.manga, DirectoryType = 5, DirectoryTitle = "Completed" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.manga, DirectoryType = 7, DirectoryTitle = "Dropped" });
                            break;
                        }
                }
            }
            catch (NullReferenceException)
            {
                Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.Directory, DirectoryType = 1, DirectoryTitle = "Anime" });
                Dir.Add(new VirtualDirectory { AnimeOrManga = AnimeOrManga.Directory, DirectoryType = 2, DirectoryTitle = "Manga" });
                //Dir.Add for custom folder ( DirectoryType = 3 ), and always navigate to CollectionLibrary filtered.
            }
            return Dir;
        }
    }
}