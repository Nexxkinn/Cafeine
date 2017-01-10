using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Datalist
{
    public class ItemDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int My_score { get; set; }
        public string Imgurl { get; set; }
        public int Totalepisodes { get; set; }
        public Animestatus My_status { get; set; }
    }

    class ExpandedItemDetail
    {
    }
}
