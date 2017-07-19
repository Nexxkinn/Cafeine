using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cafeine.Models {
    public class LocalDirectorySetupLocalFoldersModel {
        public string FolderName;
        public LocalDirectorySetupItems LinkedItems;
    }
    public class LocalDirectorySetupItems {
        public string Category;
        public int ItemID;
        public string Title;
    }
}
