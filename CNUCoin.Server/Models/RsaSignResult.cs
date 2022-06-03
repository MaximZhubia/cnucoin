using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CNUCoin.Server.Models
{
    class RsaSignResult
    {
        public byte[] Signature { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }

        public RsaSignResult(byte[] signature, byte[] publicKey, byte[] privateKey)
        {
            Signature = signature;
            PublicKey = publicKey;
            PrivateKey = privateKey;
        }
    }
}
