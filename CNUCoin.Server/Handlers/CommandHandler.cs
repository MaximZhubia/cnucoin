using CNUCoin.Server.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CNUCoin.Server.Handlers
{
    static class CommandHandler
    {
        private static ApplicationDbContext dbContext;

        static CommandHandler()
        {
            dbContext = new ApplicationDbContext();
        }

        public static void GenerateUser(out byte[] privateKey, out byte[] signedData, out int userId, out string coinId)
        {
            RsaSignResult signature = CryptoHandler.CreateSignature();
            coinId = BitConverter.ToString(new SHA512Managed().ComputeHash(signature.PublicKey));

            Member member = new Member
            {
                CoinId = coinId,
                PublicKey = BitConverter.ToString(signature.PublicKey),
                //IsMiner = true
            };

            dbContext.AddMember(member);

            Private @private = new Private
            {
                CoinId = coinId,
                PrivateKey = BitConverter.ToString(signature.PrivateKey),
                PublicKey = BitConverter.ToString(signature.PublicKey)
            };

            dbContext.AddCoin(@private);

            privateKey = signature.PrivateKey;
            signedData = signature.Signature;
            userId = member.Id;
        }

        public static int CreateTransaction(/*string fromCoinId, */string toCoinId, int value, string sign)
        {
            Transaction transaction;
            
            if (dbContext.BlockChains.Count() == 0)
            {
                transaction = new Transaction
                {
                    //FromCoinId = fromCoinId,
                    FromCoinId = string.Empty,
                    ToCoinId = toCoinId,
                    Value = value,
                    HashDate = DateTime.Now.AddTicks(new Random().Next(-999999, 999999)) // extra field
                };
            }
            else
            {
                BlockChain blockChain = dbContext.BlockChains.OrderBy(x => x.HashDate).Last();

                transaction = new Transaction
                {
                    //FromCoinId = fromCoinId,
                    FromCoinId = string.Empty,
                    ToCoinId = toCoinId,
                    Value = value,
                    Hash = blockChain.Hash,
                    HashDate = DateTime.Now.AddTicks(new Random().Next(-999999, 999999)) // extra field
                };
            }

            string concatenatedFields = transaction.FromCoinId + transaction.ToCoinId + transaction.Value + transaction.Hash + transaction.HashDate;
            string transactionHash = CryptoHandler.GetSHA512Hash(concatenatedFields);

            transaction.Hash = transactionHash;
            transaction.HashDate = DateTime.Now;
            transaction.Sign = sign;

            dbContext.AddTransaction(transaction);

            return transaction.Id;
        }

        public static double Mine(int minerId, string sign)
        {
            Transaction transaction = dbContext.Transactions.Where(x => x.IsApproved == false).OrderBy(x => x.HashDate).First();

            BlockChain blockChain = new BlockChain
            {
                Hash = transaction.Hash,
                HashDate = transaction.HashDate,
                Nonce = 0
            };

            string concatenatedHashes = transaction.Hash + blockChain.Hash + blockChain.Nonce;
            blockChain.Hash = CryptoHandler.GetSHA512Hash(concatenatedHashes);
            blockChain.HashDate = DateTime.Now;

            Stopwatch stopwatch = Stopwatch.StartNew();

            //while (!blockChain.Hash.StartsWith('0'))
            while (!blockChain.Hash.StartsWith("06-4D-5"))
            {
                blockChain.Nonce++;

                concatenatedHashes = transaction.Hash + blockChain.Hash + blockChain.Nonce;
                blockChain.Hash = CryptoHandler.GetSHA512Hash(concatenatedHashes);
                blockChain.HashDate = DateTime.Now;
            }

            stopwatch.Stop();
            blockChain.TotalSeconds = stopwatch.Elapsed.TotalSeconds;
            blockChain.MinerId = minerId;
            blockChain.Sign = sign;

            ApproveTransaction(transaction);
            dbContext.AddBlockChain(blockChain);

            return blockChain.TotalSeconds;
        }

        private static void ApproveTransaction(Transaction transaction)
        {
            transaction.IsApproved = true;

            Wallet wallet = new Wallet
            {
                FromCoinId = transaction.FromCoinId,
                ToCoinId = transaction.ToCoinId,
                TransactionId = transaction.Id
            };

            dbContext.AddWallet(wallet);
        }
    }
}
