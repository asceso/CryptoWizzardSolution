using Services.Models;

namespace Services.HashingService
{
    public interface IHashingService
    {
        /// <summary>
        /// Зашифровать строку
        /// </summary>
        /// <param name="source">Источник</param>
        /// <returns>Зашифрованная строка</returns>
        string EncodeBotCrypt(string source);
        /// <summary>
        /// Дешифровать строку
        /// </summary>
        /// <param name="source">Источник</param>
        /// <returns>Дешифрованная строка</returns>
        string DecodeBotCrypt(string source);

        void EncodeBlowFish(CryptoKeyModel key, string file_path);
        bool DecodeBlowFish(CryptoKeyModel key, string file_path);
        bool DecodeBlowFish(CryptoKeyModel key, string file_path, out string output_filename);
    }
}
