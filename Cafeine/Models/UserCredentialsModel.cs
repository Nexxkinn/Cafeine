using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Models
{
    public class UserAccountModel
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        public Enums.ServiceType Service { get; set; }

        public bool IsDefaultService { get; set; }

        public string Name { get; set; }

        public string HashID { get; set; }

        public Avatar Avatar { get; set; }

        public object AdditionalOption { get; set; }
    }

    public class Avatar
    {
        public string Large { get; set; }

        public string Medium { get; set; }
    }
}
