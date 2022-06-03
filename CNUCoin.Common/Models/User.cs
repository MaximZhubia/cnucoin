using System;
using System.Collections.Generic;
using System.Text;

namespace CNUCoin.Common.Models
{
    public class User
    {
        public int Id { get; set; }
        public string CoinId { get; set; }
        public string Sign { get; set; }
        public string PrivateKey { get; set; }
    }
}
