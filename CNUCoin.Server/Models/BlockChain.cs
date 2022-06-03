using System;
using System.Collections.Generic;
using System.Text;

namespace CNUCoin.Server.Models
{
    class BlockChain
    {
        public int Id { get; set; }
        public int MinerId { get; set; }
        public string Hash { get; set; }
        public DateTime HashDate { get; set; }
        public int Nonce { get; set; }
        //public string CoinId { get; set; }
        public string Sign { get; set; }
        public double TotalSeconds { get; set; }
    }
}
