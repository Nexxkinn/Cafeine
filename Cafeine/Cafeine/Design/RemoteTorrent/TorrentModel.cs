using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Design.RemoteTorrent {
    public class TorrentModel {
        public string hash;
        public string name;
        public long size;
        public float progress;
        public long eta;
        public string state;
    }
}
