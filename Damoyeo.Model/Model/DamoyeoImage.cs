using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damoyeo.Model.Model
{
    public class DamoyeoImage
    {
        public int Id { get; set; }
        public string save_filename { get; set; }
        public string origin_filename { get; set; }
        public string table_name { get; set; }
        public string directory_path { get; set; }

    }
}

