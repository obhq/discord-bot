using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oboteration.Models
{
    public class Sony
    {
        public class Piece
        {
            public string url { get; set; }
            public int fileOffset { get; set; }
            public int fileSize { get; set; }
            public string hashValue { get; set; }
        }

        public class Root
        {
            public int originalFileSize { get; set; }
            public string packageDigest { get; set; }
            public int numberOfSplitFiles { get; set; }
            public List<Piece> pieces { get; set; }
        }


    }
}
