using Newtonsoft.Json;
using Prism.Mvvm;

namespace Services.Models
{
    public class CryptoKeyModel : BindableBase
    {
        private string _publicName;
        private long _byteLength;

        [JsonProperty(nameof(PublicName))]
        public string PublicName { get => _publicName; set => SetProperty(ref _publicName, value); }

        [JsonProperty(nameof(ByteLength))]
        public long ByteLength { get => _byteLength; set => SetProperty(ref _byteLength, value); }
    }
}
