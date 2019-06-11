using Cafeine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.Services
{
    public class ServiceItemComparer : IEqualityComparer<ServiceItem>
    {
        public bool Equals(ServiceItem x, ServiceItem y)
        {
            if (x.MalID != 0 && x.MalID != 0) return x.MalID == y.MalID;
            else return x.ServiceID == y.ServiceID;
        }

        public int GetHashCode(ServiceItem obj)
        {
            return obj.MalID.GetHashCode();
        }
    }
}
