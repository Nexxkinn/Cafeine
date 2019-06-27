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

namespace Cafeine.Services.FilenameParser
{
    public partial class CafeineFilenameParser
    {
        string Filename;
        string[] Elements;
        List<ContentList> ContentList;

        private static readonly char[] brackets = { '[', ']', '(', ')', '<', '>', '{', '}', '\u300C', '\u300D', '\u300E', '\u300F', '\u3010', '\u3011', '\uFF08', '\uFF09' };

        private static readonly char[] separators = { ' ', '.', '_', ',' };

        private static readonly ushort[] filters_utf =
        {
            0x5B, // [
            0x5D, // ]
            0x28, // (
            0x29, // )
            0x3C, // <
            0x3E, // >
            0x7B, // {
            0x7D, // }
            0x300C, // \u300C
            0x300D, // \u300D
            0x300E, // \u300E
            0x300F, // \u300F
            0x3010, // \u3010
            0x3011, // \u3011
            0xFF08, // \uFF08
            0xFF09 // \uFF09
        };

        private static readonly ushort[] sep_utf =
        {
            0x20, // space
            0x2E, // .
            0x5F, // _
            0x2C, // ,
            0x00, // For 16-byte Vector
            0x00, // Left blank reserved
            0x00, // for future possible
            0x00, // char separator
            0x00, // 
            0x00, // 
            0x00, //
            0x00, //
            0x00, // 
            0x00, // 
            0x00, //
            0x00, //
        };

        private static readonly char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        public CafeineFilenameParser(StorageFile filename) : this(filename, null) { }

        public CafeineFilenameParser(StorageFile filename,List<ContentList> contentlist)
        {
            this.Filename = filename.DisplayName;
            this.ContentList = contentlist;
        }

        public bool TryCreateFingerprint(out string[] fingerprint,bool useSIMD = true)
        {
            var ar_splits = Filename.Split(brackets,StringSplitOptions.RemoveEmptyEntries);
            var splits    = new Span<string>(ar_splits);

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
                            newElement[index] = (char)key;
                            index++;
                        }
                    }
                    newElement = newElement.Slice(0, index);

                    // Prevent CRC32 to be listed as fingerprint
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

            fingerprint = ElementsList.ToArray();
            return fingerprint.Length != 0;
        }

        public bool TryCreateUniqNumbers(out string[] unique_number,bool useSIMD = true)
        {
            unique_number = new string[0];
            if (Elements == null && !TryCreateFingerprint(out Elements, useSIMD)) return false;
            ReadOnlySpan<string> elements = Elements;

            Span<string> uq_num = new string[elements.Length];
            int uq_len = 0;
            for(int i = 0; i<elements.Length; i++)
            {
                ReadOnlySpan<char> element = elements[i].AsSpan();
                Span<char> num = stackalloc char[element.Length];
                int len = 0;
                bool exists = false;
                for(int j = 0; j < element.Length; j++)
                {
                    char v = element[j];
                    if (v >= '0' && v <= '9')
                    {
                        num[len++] =  v;
                        exists = true;
                    }
                    else if(exists)
                    {
                        num[len++] = ' ';
                    }
                }
                if (exists) uq_num[uq_len++] = num.Slice(0,len).ToString();
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
                           .Select(x => Int32.Parse(x.Value))
                           .FirstOrDefault();
            TryCreateFingerprint(out fingerprint);
            TryCreateUniqNumbers(out uniq);
            return  episode.HasValue;
        }

        private void FilterUniqBasedOnSamples(ref ReadOnlySpan<string> elements)
        {
            Span<int> dist = stackalloc int[elements.Length];
            List<string> newElements = new List<string>();
            if(TryGetFileList(out List<File> samples)) return;

            foreach (File file in samples)
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
        private bool TryGetFileList(out List<File> samples)
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
