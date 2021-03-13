using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VNCLNIC.Models.Data
{
    public class District
    {

        public int Id { get; set; }

        public string Name
        {
            get; set;
        }
        public virtual City City { get; set; }

    }
}