/*
** Cafeine custom filename parser 
** Copyright (c) 2019- , Николай
*/

using Cafeine.Models;
using Fastenshtein;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using Windows.Storage;
using Cafeine.Services.Anitomy;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml.Documents;
using System.Globalization;

namespace Cafeine.Services.FilenameParser
{
    public partial class CafeineFilenameParser
    {
        string Filename;
        string[] Elements;
        List<MediaList> ContentList;

        private static readonly char[] brackets = { '[', ']', '(', ')', '<', '>', '{', '}', '\u300C', '\u300D', '\u300E', '\u300F', '\u3010', '\u3011', '\uFF08', '\uFF09' };

        private static readonly char[] separators = { ' ', '.', '_', ',' };

        private static readonly char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        private static readonly ushort[] filters_utf =
        {
            0x5B,   0x5D,   0x28,   0x29,   // [ ] ( )
            0x3C,   0x3E,   0x7B,   0x7D,   // < > { }
            0x300C, 0x300D, 0x300E, 0x300F, // \u300C \u300D \u300E \u300F
            0x3010, 0x3011, 0xFF08, 0xFF09  // \u3010 \u3011 \uFF08 \uFF09
        };

        private static readonly ushort[] sep_utf =
        {
            0x20, 0x2E, 0x5F, 0x2C, // space // . // _ // ,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00
        };

        private static readonly ushort[] hex_utf =
        {
            0x41, 0x42, 0x43, 0x44, // A B C D
            0x45, 0x46, 0x30, 0x31, // E F 0 1
            0x32, 0x33, 0x34, 0x35, // 2 3 4 5
            0x36, 0x37, 0x38, 0x39, // 6 7 8 9
            0X00, 0X00, 0X00, 0X00, // additional values for 
            0X00, 0X00,             // vector number
        };

        public CafeineFilenameParser(StorageFile filename) : this(filename, null) { }

        public CafeineFilenameParser(StorageFile filename,List<MediaList> contentlist)
        {
            this.Filename = filename.DisplayName;
            this.ContentList = contentlist;
        }

        public bool TryCreateFingerprint(out string[] fingerprint,bool useSIMD = true)
        {
            var splits = Filename.Split(brackets,StringSplitOptions.RemoveEmptyEntries);

            List<string> ElementsList = new List<string>();

            // remove any separators : space, dot, comma, underscore, dash
            if (Vector.IsHardwareAccelerated && useSIMD)
            {
                var Vsep = new Vector<ushort>(sep_utf);

                int i = 0;
                for (; i< splits.Length; i++)
                {
                    ReadOnlySpan<char> element = splits[i].AsSpan();
                    Span<char> newElement = stackalloc char[element.Length];

                    int j = 0,index=0;
                    for (;j<element.Length;j++)
                    {
                        ushort key = element[j];
                        Vector<ushort> Vkey = new Vector<ushort>(key);
                        if (Vector.Equals(Vkey, Vsep).Equals(Vector<ushort>.Zero))
                        {
                            newElement[index++] = (char)key;
                        }
                    }
                    newElement = newElement.Slice(0, index);

                    // Prevent CRC32 to be added as fingerprint
                    if (newElement.Length != 0 && !isCRC32(newElement)) ElementsList.Add(newElement.ToString());
                }
            }
            else
            {
                // fallback non-SIMD non-Span version
                for (int i = 0; i < splits.Length; i++)
                {
                    string element = splits[i];
                    string[] element_split = element.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    string newElement = string.Join(string.Empty,element_split);

                    // Prevent CRC32 to be listed as fingerprint
                    if (newElement != string.Empty && !isCRC32(newElement.AsSpan())) ElementsList.Add(newElement);
                }
            }

            Elements = ElementsList.ToArray();
            fingerprint = Elements;
            return fingerprint.Length != 0;
        }

        public bool TryCreateUniqNumbers(out string[] unique_number,bool useSIMD = true)
        {
            if (Elements == null && !TryCreateFingerprint(out Elements, useSIMD))
            {
                unique_number = new string[0];
                return false;
            }

            Span<string> uq_num = new string[Elements.Length];
            int uq_len = 0;

            string buffer = string.Empty;
            for (int i = 0; i< Elements.Length; i++)
            {
                Span<char> num = Elements[i].ToArray();
                int  start  = -1;
                for(int j = 0; j < num.Length; j++)
                {
                    char v = num[j];
                    if (v >= '0' && v <= '9')
                    {
                       if (start == -1) start = j;
                    }
                    else if (start != -1)
                    {
                        num[j] = ' ';
                    }
                }
                if (start != -1) uq_num[uq_len++] = num.Slice(start).ToString();
            }
            unique_number = uq_num.Slice(0, uq_len).ToArray();
            return true;
        }

        /// <summary>
        /// Parse and get array of possible episode numbers <br/>
        /// </summary>
        /// <param name="episode">output result</param>
        /// <param name="useSIMD">Force SIMD operation. Currently not available</param>
        /// <returns></returns>
        public bool TryGetEpisodeNumber(out int[] episode, bool useSIMD = true)
        {
            if (TryRegexMethod(out episode)) return true;
            if (!TryCreateUniqNumbers(out string[] Uniq, useSIMD)) return false;

            ReadOnlySpan<string> uniq = Uniq;

            // 0th stage : filter Elements with samples using Leincstein distance
            if(ContentList != null) FilterUniqBasedOnSamples(ref uniq);

            // 1st stage : filter elements that doesn't contain any number
            List<char[]> containsNumber = new List<char[]>(uniq.Length);
            for(int i = 0; i < uniq.Length; i++)
            {
                ReadOnlySpan<char> element = Uniq[i].AsSpan();

                for(int j = 0; j < element.Length; j++)
                {
                    ushort chr  = element[j];
                    if (chr >= 0x30 && chr <= 0x39) // 0 and 9
                    {
                        containsNumber.Add(element.Slice(j).ToArray());
                        break;
                    }
                }
            }


            // TODO: number parser. 
            return false;
        }

        public bool TryGetEpisodeUsingAnitomy(out int? episode, out string[] fingerprint, out string[] uniq)
        {
            var item = AnitomySharp.Parse(Filename, new Options(episode: true));
            episode  = item.Where (x => x.Category == Element.ElementCategory.ElementEpisodeNumber)
                           .Select(x => {
                               if (Int32.TryParse(x.Value, NumberStyles.Integer, null, out int value)){
                                   return value;
                               }
                               else return -1;
                           })
                           .FirstOrDefault();
            TryCreateFingerprint(out fingerprint);
            TryCreateUniqNumbers(out uniq);
            return  episode.HasValue && episode.Value != -1;
        }

        private void FilterUniqBasedOnSamples(ref ReadOnlySpan<string> elements)
        {
            Span<int> dist = stackalloc int[elements.Length];
            List<string> newElements = new List<string>();
            if(TryGetFileList(out List<MediaFile> samples)) return;

            foreach (MediaFile file in samples)
            {
                string[] sample = file.Fingerprint;
                // Oth pass : Just don't bother for mismatch length. 
                // Blame fansubbers for not following standard practice.
                if (sample.Length != elements.Length) continue;

                // assume both elements and sample follow the same identifier.
                for (int i = 0; i < sample.Length; i++)
                {
                    dist[i] += Levenshtein.Distance(elements[i], sample[i]);
                }
            }

            for (int index = 0; index < dist.Length; index++)
            {
                if (dist[index] != 0) newElements.Add(elements[index]);
            }

            elements = newElements.ToArray();
        }
        private bool TryGetFileList(out List<MediaFile> samples)
        {

            throw new NotSupportedException();
        }
        private bool TryRegexMethod(out int[] episodes)
        {
            Regex R_eps_num = new Regex(@"(?<=\-\s)(?:\d+)", RegexOptions.IgnoreCase);
            if (R_eps_num.IsMatch(Filename)){
                int num = Convert.ToInt32(R_eps_num.Match(Filename).Value);
                episodes = new int[1] { num };
                return true;
            }
            else
            {
                episodes = new int[0];
                return false;
            }
        }
        private bool isCRC32(ReadOnlySpan<char> num)
        {
            if (num.Length != 8)  return false;
            for(int i = 0; i < num.Length; i++)
            {
                char c = num[i];
                if (c >= '0' && c <= '9' || c >= 'A' && c <= 'F' || c >= 'a' && c <= 'f') continue;
                else return false;
            }
            return true;
        }
    }
}
