/*
** Cafeine custom filename parser 
** Copyright (c) 2019- , Николай
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Storage;

namespace Cafeine.Services.FilenameParser
{
    public class CafeineFilenameParser
    {
        string Filename;
        string[] Elements;

        private static readonly char[] brackets = {'[',']','(',')','<','>','{','}','\u300C','\u300D','\u300E','\u300F','\u3010','\u3011','\uFF08','\uFF09' };

        private static readonly char[] separators = { ' ', '.', '_', ',' };

        private static readonly ushort[] filters_utf =
        {
            0x20, // space
            0x2E, // .
            0x5F, // _
            0x2D, // -
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

        public CafeineFilenameParser(StorageFile filename)
        {
            this.Filename = filename.DisplayName;
        }
        public CafeineFilenameParser(string test)
        {
            this.Filename = test;
        }

        public bool TryGetArrayIdentifier(out string[] elements)
        {
            // remove any separator
            elements = Filename.Split(brackets);
            return elements.Length != 0;
        }

        public bool TryGetEpisodeNumber(out int[] episode)
        {
            episode = new int[] { };
            if (!TryGetArrayIdentifier(out Elements)) return false;

            return false;
        }
    }
}
