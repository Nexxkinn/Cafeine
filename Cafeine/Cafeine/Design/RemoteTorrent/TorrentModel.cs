using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Design.RemoteTorrent {
    public class TorrentModel {
        public string hash;
        public string name;
        public int size;
        public float progress;
        public int eta;
        public string state;
    }
}
