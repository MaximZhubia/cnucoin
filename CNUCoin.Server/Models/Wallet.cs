using System;
using System.Collections.Generic;
using System.Text;

namespace CNUCoin.Server.Models
{
    class Wallet
    {
        public int Id { get; set; }
        //public string CoinId { get; set; }
        public string FromCoinId { get; set; }
        public string ToCoinId { get; set; }
        public int TransactionId { get; set; }
        //public List<Transaction> Transactions { get; set; }
        //public int FromMemberId { get; set; }
        //public int ToMemberId { get; set; }
    }
}
