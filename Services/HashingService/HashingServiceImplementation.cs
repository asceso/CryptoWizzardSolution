using BotCrypt.Core;
using Services.Models;

namespace Services.HashingService
{
    public class HashingServiceImplementation : IHashingService
    {
        private static readonly string bot_password = "mWfh6nKRKG%bJJz";
        public string EncodeBotCrypt(string source) => Crypter.EncryptString(bot_password, source);
        public string DecodeBotCrypt(string source) => Crypter.DecryptString(bot_password, source);
        public void EncodeBlowFish(CryptoKeyModel key, string file_path)
        {
            throw new System.NotImplementedException();
        }
        public bool DecodeBlowFish(CryptoKeyModel key, string file_path)
        {
            throw new System.NotImplementedException();
        }
    }
}
