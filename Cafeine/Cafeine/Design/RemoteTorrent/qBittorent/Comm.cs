﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace Cafeine.Design.RemoteTorrent.qBittorent {
    public enum ManageTorrent {
        Pause=1,
        Resume,
        Delete,
        DeleteWithFile,
        PauseAll,
        ResumeAll,
    }
    class Comm {
        ///GET
        ///get torrent list
        ///
        ///POST
        ///Download Torrent from URL / magnet
        ///Pause Torrent
        ///Pause All
        ///Resume Torrent
        ///Resume ALl
        ///Delete Torrent
        ///Delete Torrent with downloaded data
        ///

        public static async Task<List<TorrentModel>> GetTorrentList() {
            string TorrentList = await CoreApi.GetASync("/query/torrents").ConfigureAwait(false);
            JArray parse = JArray.Parse(TorrentList);
            var Jlist = parse.ToList();
            var Lists = new List<TorrentModel>();
            List<TorrentModel> product = new List<TorrentModel>();
            foreach(JToken item in Jlist) {
                TorrentModel itemproperty = item.ToObject<TorrentModel>();
                product.Add(itemproperty);
            }
            return await Task.FromResult(product);
        }
        public static async Task<bool> DownloadTorrent(string hash) {

            using (var content = new Http​Multipart​Form​Data​Content("-------" + DateTime.Now.ToString(CultureInfo.InvariantCulture))) {
                content.Add(new HttpStringContent("magnet:" + hash), "urls");
                //content.Add(new HttpStringContent) save path
                bool result = await CoreApi.PostAsync("/command/download", content);
                if (result == true) {
                    return await Task.FromResult(true);
                }
                else return await Task.FromResult(false);
            }
        }
        public static async Task<bool> TorrentAction(ManageTorrent manager,string hash) {
            Dictionary<string, string> hashkey = new Dictionary<string, string> {
                { "hash", hash }
            };
            var content = new HttpFormUrlEncodedContent(hashkey);
            string path = string.Empty;
            switch (manager) {
                case ManageTorrent.Pause:
                path = "/command/pause";
                break;

                case ManageTorrent.Resume:
                path = "/command/resume";
                break;

                case ManageTorrent.Delete:
                path = "/command/delete";
                break;

                case ManageTorrent.DeleteWithFile:
                path = "/command/deletePerm";
                break;

                case ManageTorrent.PauseAll:
                path = "/command/pauseAll";
                content = null;
                break;

                case ManageTorrent.ResumeAll:
                path = "/command/resumeAll";
                content = null;
                break;
            }

            bool result = await CoreApi.PostAsync(path, content);
            return await Task.FromResult(result);
        }

    }

}
