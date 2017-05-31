using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafeine.Models;
namespace Cafeine.ViewModels
{
    class DirectoryExplorerViewModel
    {
        public static List<VirtualDirectory> DefaultDirectory (VirtualDirectory e )
        {
            List<VirtualDirectory> Dir = new List<VirtualDirectory>();
            try
            {
                switch (e.DirectoryType)
                {
                    case 1:
                        {
                            Dir.Add(new VirtualDirectory { AnimeOrManga = true, DirectoryType = 4, DirectoryTitle = "Watching" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = true, DirectoryType = 6, DirectoryTitle = "On Hold" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = true, DirectoryType = 9, DirectoryTitle = "Planned to Watch" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = true, DirectoryType = 5, DirectoryTitle = "Completed" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = true, DirectoryType = 7, DirectoryTitle = "Dropped" });
                            break;
                        }
                    case 2:
                        {
                            Dir.Add(new VirtualDirectory { AnimeOrManga = false, DirectoryType = 4, DirectoryTitle = "Reading" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = false, DirectoryType = 6, DirectoryTitle = "On Hold" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = false, DirectoryType = 9, DirectoryTitle = "Planned to Read" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = false, DirectoryType = 5, DirectoryTitle = "Completed" });
                            Dir.Add(new VirtualDirectory { AnimeOrManga = false, DirectoryType = 7, DirectoryTitle = "Dropped" });
                            break;
                        }
                }
            }
            catch (NullReferenceException)
            {
                Dir.Add(new VirtualDirectory { DirectoryType = 1, DirectoryTitle = "Anime" });
                Dir.Add(new VirtualDirectory { DirectoryType = 2, DirectoryTitle = "Manga" });
                //Dir.Add for custom folder ( DirectoryType = 3 ), and always navigate to CollectionLibrary filtered.
            }
            return Dir;
        }
    }
}
