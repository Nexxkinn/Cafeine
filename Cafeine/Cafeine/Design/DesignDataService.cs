﻿using System.Threading.Tasks;
using Cafeine.Model;

namespace Cafeine.Design {
    public class DesignDataService : IDataService {
        public Task<DataItem> GetData() {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]");
            return Task.FromResult(item);
        }
    }
}