using System;
using System.Collections.Generic;
using System.Text;

namespace CNUCoin.Server.Models
{
    // private table
    class Private
    {
        public int Id { get; set; }
        public string CoinId { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
    }
}
