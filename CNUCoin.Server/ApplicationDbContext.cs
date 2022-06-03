using CNUCoin.Server.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CNUCoin.Server
{
    class ApplicationDbContext : DbContext
    {
        public DbSet<BlockChain> BlockChains { get; set; }
        public DbSet<Private> Privates { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }


        public ApplicationDbContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CNUCoin;Trusted_Connection=True;");
        }

        public void AddCoin(Private coin)
        {
            Privates.Add(coin);
            SaveChanges();
        }

        public void AddMember(Member member)
        {
            Members.Add(member);
            SaveChanges();
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
            SaveChanges();
        }

        public void AddBlockChain(BlockChain blockChain)
        {
            BlockChains.Add(blockChain);
            SaveChanges();
        }

        public void AddWallet(Wallet wallet)
        {
            Wallets.Add(wallet);
            SaveChanges();
        }
    }
}
