using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Cafeine.Services
{
    public static class ImageCache
    {
        private static StorageFolder CacheFolder = ApplicationData.Current.LocalCacheFolder;

        private static StorageFolder ImageCacheFolder;

        private static HttpClient Httpclient;

        static ImageCache()
        {
            if(Httpclient == null)
            {
                Httpclient = new HttpClient(new HttpClientHandler { MaxConnectionsPerServer = 100 });
            }
        }

        public static async Task CreateImageCacheFolder()
        {
            ImageCacheFolder = await CacheFolder.CreateFolderAsync("ImageCache", CreationCollisionOption.OpenIfExists);
        }

        public static async Task<StorageFile> GetFromCacheAsync(string url)
        {
            StorageFile baseFile = null;
            var hash = GetCacheFileName(url);
            try
            {
                baseFile = await ImageCacheFolder.TryGetItemAsync(hash).AsTask().ConfigureAwait(false) as StorageFile;
                if (baseFile != null)
                {
                    //Even if baseFile says that there's a file in that folder,
                    //You should check whether nor not the app is already cached.
                    var baseFileSize = await baseFile.GetBasicPropertiesAsync().AsTask().ConfigureAwait(false);
                    if (baseFileSize.Size != 0) return baseFile;
                }
                baseFile = await DownloadAsync(url, hash);
                return baseFile;
            }
            finally
            {
                baseFile = null;
                hash = string.Empty;
            }
        }

        private static async Task<StorageFile> DownloadAsync(string url, string hash)
        {
            var file = await ImageCacheFolder.CreateFileAsync(hash, CreationCollisionOption.ReplaceExisting);
            using (MemoryStream ms = new MemoryStream())
            {
                using (var stream = await Httpclient.GetStreamAsync(url))
                {
                    stream.CopyTo(ms);
                    ms.Flush();
                    ms.Position = 0;

                    using (var streamfile = await file.OpenStreamForWriteAsync())
                    {
                        ms.CopyTo(streamfile);
                        streamfile.Flush();
                    }
                }
            }
            return file;
        }

        private static string GetCacheFileName(string uri)
        {
            return CreateHash64(uri).ToString();
        }

        private static ulong CreateHash64(string str)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(str);
            ulong value = (ulong)utf8.Length;
            for (int n = 0; n < utf8.Length; n++)
            {
                value += (ulong)utf8[n] << ((n * 5) % 56);
            }
            return value;
        }
    }
}
