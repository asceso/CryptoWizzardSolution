using BotCrypt.Core;
using Elskom.Generic.Libs;
using Services.Models;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Services.HashingService
{
    public class HashingServiceImplementation : IHashingService
    {
        private static readonly string bot_password = "VOmKgYYXV2KKZRKjfk0BudBNtUqALBZvBXQQHcFAybBJel6z63DYZH3IgJbfEtdbgTLGTCRm8dDfteZ6%bJJz";
        private static readonly string blow_password = "UPEZ2B7dFmYrEHFm7qNQwxx7dxWmqz9pf3FkI8kiZVrwXTz5YKk40hEVoLfwhJMO9WPpxcZKT8FwUeTK%bJJz";
        public string EncodeBotCrypt(string source) => Crypter.EncryptString(bot_password, source);
        public string DecodeBotCrypt(string source) => Crypter.DecryptString(bot_password, source);
        public void EncodeBlowFish(CryptoKeyModel key, string file_path)
        {
            BlowFish blow = new(blow_password);
            blow.IV = new byte[] { 33, 11, 233, 156, 155, 230, 79, 129 };

            using FileStream fs = new(file_path, FileMode.Open);

            FileInfo fi = new(file_path);
            byte[] key_buffer = Encoding.UTF8.GetBytes($"KEY==='{key.PublicName}',EXTL==='{fi.Extension.Length - 1}',EXT==='{fi.Extension}'");
            byte[] file_buffer = new byte[(int)fs.Length];
            fs.Read(file_buffer, 0, (int)fs.Length);
            fs.Close();

            List<byte> key_with_file_buffer = new();
            key_with_file_buffer.AddRange(key_buffer);
            key_with_file_buffer.AddRange(file_buffer);

            byte[] bytes = blow.EncryptCBC(key_with_file_buffer.ToArray());

            using FileStream efs = new(file_path.Replace(fi.Extension, ".cwf"), FileMode.Create);
            efs.Write(bytes, 0, bytes.Length);
            efs.Close();
        }
        public bool DecodeBlowFish(CryptoKeyModel key, string file_path)
        {
            BlowFish blow = new(blow_password);
            blow.IV = new byte[] { 33, 11, 233, 156, 155, 230, 79, 129 };

            using FileStream fs = new(file_path, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
            fs.Close();

            byte[] bytes = blow.DecryptCBC(buffer);

            List<byte> key_bytes = new();
            for (int i = 0; i < key.ByteLength + 8; i++)
            {
                key_bytes.Add(bytes[i]);
            }
            string crypted_key = Encoding.UTF8.GetString(key_bytes.ToArray());

            if ($"KEY==='{key.PublicName}'" != crypted_key)
            {
                return false;
            }
            else
            {
                int key_ext_offset = key_bytes.Count + 1;
                List<byte> ext_l_bytes = new();
                for (int i = key_ext_offset; i < key_ext_offset + 10; i++)
                {
                    ext_l_bytes.Add(bytes[i]);
                }
                string encrypted_ext_l = Encoding.UTF8.GetString(ext_l_bytes.ToArray());
                int old_extension_length = int.Parse(encrypted_ext_l.Replace("EXTL==='", string.Empty).Replace("'", string.Empty));

                key_ext_offset += ext_l_bytes.Count + 1;
                List<byte> ext_bytes = new();
                for (int i = key_ext_offset; i < key_ext_offset + 9 + old_extension_length; i++)
                {
                    ext_bytes.Add(bytes[i]);
                }
                string crypted_ext = Encoding.UTF8.GetString(ext_bytes.ToArray());
                string old_extension = crypted_ext.Replace("EXT==='", string.Empty).Replace("'", string.Empty);

                key_ext_offset += ext_bytes.Count;

                List<byte> file_bytes = new();
                for (int i = key_ext_offset; i < bytes.Length; i++)
                {
                    file_bytes.Add(bytes[i]);
                }
                using FileStream efs = new(file_path.Replace(".cwf", old_extension), FileMode.Create);
                efs.Write(file_bytes.ToArray(), 0, file_bytes.Count);
                efs.Close();
            }
            return true;
        }
        public bool DecodeBlowFish(CryptoKeyModel key, string file_path, out string output_filename)
        {
            BlowFish blow = new(blow_password);
            blow.IV = new byte[] { 33, 11, 233, 156, 155, 230, 79, 129 };

            using FileStream fs = new(file_path, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, (int)fs.Length);
            fs.Close();

            byte[] bytes = blow.DecryptCBC(buffer);

            List<byte> key_bytes = new();
            for (int i = 0; i < key.ByteLength + 8; i++)
            {
                key_bytes.Add(bytes[i]);
            }
            string crypted_key = Encoding.UTF8.GetString(key_bytes.ToArray());

            if ($"KEY==='{key.PublicName}'" != crypted_key)
            {
                output_filename = string.Empty;
                return false;
            }
            else
            {
                int key_ext_offset = key_bytes.Count + 1;
                List<byte> ext_l_bytes = new();
                for (int i = key_ext_offset; i < key_ext_offset + 10; i++)
                {
                    ext_l_bytes.Add(bytes[i]);
                }
                string encrypted_ext_l = Encoding.UTF8.GetString(ext_l_bytes.ToArray());
                int old_extension_length = int.Parse(encrypted_ext_l.Replace("EXTL==='", string.Empty).Replace("'", string.Empty));

                key_ext_offset += ext_l_bytes.Count + 1;
                List<byte> ext_bytes = new();
                for (int i = key_ext_offset; i < key_ext_offset + 9 + old_extension_length; i++)
                {
                    ext_bytes.Add(bytes[i]);
                }
                string crypted_ext = Encoding.UTF8.GetString(ext_bytes.ToArray());
                string old_extension = crypted_ext.Replace("EXT==='", string.Empty).Replace("'", string.Empty);

                key_ext_offset += ext_bytes.Count;

                List<byte> file_bytes = new();
                for (int i = key_ext_offset; i < bytes.Length; i++)
                {
                    file_bytes.Add(bytes[i]);
                }
                using FileStream efs = new(file_path.Replace(".cwf", old_extension), FileMode.Create);
                efs.Write(file_bytes.ToArray(), 0, file_bytes.Count);
                efs.Close();
                output_filename = file_path.Replace(".cwf", old_extension);
            }
            return true;
        }
    }
}
