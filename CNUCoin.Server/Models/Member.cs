using System;
using System.Collections.Generic;
using System.Text;

namespace CNUCoin.Server.Models
{
    class Member
    {
        public int Id { get; set; }
        public string CoinId { get; set; }
        public string PublicKey { get; set; }
        public bool IsMiner { get; set; }
    }
}
