using System;
using System.Collections.Generic;
using System.Text;

namespace CNUCoin.Server.Models
{
    class Transaction
    {
        public int Id { get; set; }
        //public string CoinId { get; set; }
        public string FromCoinId { get; set; }
        public string ToCoinId { get; set; }
        public int Value { get; set; }
        public string Hash { get; set; }
        public DateTime HashDate { get; set; }
        public string Sign { get; set; }
        public bool IsApproved { get; set; }
    }
}
