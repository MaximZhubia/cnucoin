using CNUCoin.Server.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CNUCoin.Server.Handlers
{
    static class CryptoHandler
    {
        public static string GetSHA512Hash(string content)
        {
            SHA512Managed sha512 = new SHA512Managed();
            byte[] bytes = Encoding.Unicode.GetBytes(content);
            byte[] hash = sha512.ComputeHash(bytes);
            return BitConverter.ToString(hash);
        }

        private static byte[] GetRandomByteArray(int size)
        {
            Random random = new Random();
            byte[] bytes = new byte[size];
            random.NextBytes(bytes);
            return bytes;
        }

        public static RsaSignResult CreateSignature()
        {
            byte[] hashValue = GetRandomByteArray(64);
            byte[] signature;

            // Generate a public/private key pair.  
            RSA rsa = RSA.Create();
            // Create an RSAPKCS1SignatureFormatter object and pass it the
            // RSA instance to transfer the private key.
            RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
            // Set the hash algorithm to SHA512.
            rsaFormatter.SetHashAlgorithm(HashAlgorithmName.SHA512.Name);
            // Create a signature for hashValue and assign it to signedHashValue.
            signature = rsaFormatter.CreateSignature(hashValue);

            //return new RsaKeyPair(rsaKeyInfo, signedHashValue);
            return new RsaSignResult(signature, rsa.ExportRSAPublicKey(), rsa.ExportRSAPrivateKey());
        }

        public static bool ValidateRSASignature(byte[] hashValue, byte[] signedHashValue, byte[] publicKey)
        {
            RSA rsa = RSA.Create();
            rsa.ImportRSAPublicKey(publicKey, out _);
            RSAPKCS1SignatureDeformatter rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm(HashAlgorithmName.SHA512.Name);

            if (rsaDeformatter.VerifySignature(hashValue, signedHashValue))
            {
                // The signature is valid
                return true;
            }
            else
            {
                // The signature is not valid
                return false;
            }
        }
    }
}
