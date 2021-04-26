using Newtonsoft.Json;
using Prism.Mvvm;

namespace Services.Models
{
    public class CryptoFileModel : BindableBase
    {
        private CryptoKeyModel _key;
        private byte[] _data;

        [JsonProperty(nameof(Key))]
        public CryptoKeyModel Key { get => _key; set => SetProperty(ref _key, value); }

        [JsonProperty(nameof(Data))]
        public byte[] Data { get => _data; set => SetProperty(ref _data, value); }
    }
}
