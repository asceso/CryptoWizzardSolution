using CryptoWizzard.Views;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Services.HashingService;
using Services.MemoryService;
using Services.Models;
using System;
using System.IO;
using System.Text;

namespace CryptoWizzard.ViewModels
{
    public class KeysViewModel : BindableBase
    {
        private readonly KeysWindow keys_view;
        private readonly IMemoryService memory;
        private readonly IHashingService hashing;

        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand AcceptCommand { get; private set; }
        public DelegateCommand ResetCommand { get; private set; }
        public DelegateCommand SaveKeyCommand { get; private set; }
        public DelegateCommand LoadKeyCommand { get; private set; }

        private string _title;
        private string _inputKey;
        private bool _isKeyInstalled;
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string InputKey { get => _inputKey ??= string.Empty; set => SetProperty(ref _inputKey, value); }
        public bool IsKeyInstalled { get => _isKeyInstalled; set => SetProperty(ref _isKeyInstalled, value); }

        public KeysViewModel(IMemoryService memory, IHashingService hashing, KeysWindow keys_view)
        {
            Title = "Управление ключами";
            this.keys_view = keys_view;
            this.memory = memory;
            this.hashing = hashing;
            CloseCommand = new DelegateCommand(OnClose);
            AcceptCommand = new DelegateCommand(OnAccept, CanAcceptOrSaving).ObservesProperty(() => InputKey);
            ResetCommand = new DelegateCommand(OnReset);
            SaveKeyCommand = new DelegateCommand(OnSaveKey, () => IsKeyInstalled).ObservesProperty(() => IsKeyInstalled);
            LoadKeyCommand = new DelegateCommand(OnLoadKey);
            CheckKey();
        }
        private void OnClose() => keys_view.Close();
        private async void CheckKey()
        {
            InputKey = string.Empty;
            IsKeyInstalled = false;
            var installed_key = await memory.GetItem<string>("EncryptKey");
            if (installed_key != null)
            {
                string decoded = hashing.DecodeBotCrypt(installed_key);
                CryptoKeyModel key = JsonConvert.DeserializeObject<CryptoKeyModel>(decoded);
                InputKey = key.PublicName;
                IsKeyInstalled = true;
            }
        }
        private bool CanAcceptOrSaving()
        {
            if (InputKey.Length < 6) return false;
            return true;
        }
        private void OnAccept()
        {
            CryptoKeyModel key = new();
            key.PublicName = InputKey;
            key.ByteLength = Encoding.UTF8.GetBytes(InputKey).Length;

            string json = JsonConvert.SerializeObject(key);
            string encoded = hashing.EncodeBotCrypt(json);

            memory.StoreItem("EncryptKey", encoded);
            CheckKey();
        }
        private void OnReset()
        {
            memory.RemoveItem("EncryptKey");
            CheckKey();
        }
        private void OnSaveKey()
        {
            SaveFileDialog sfd = new();
            sfd.Filter = "Crypto Wizzard Key|*.cwk";
            sfd.Title = "Сохранение ключа Crypto Wizzard";
            sfd.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
            sfd.FileName = "MyCryptoWizzardKey";
            bool? dresult = sfd.ShowDialog();
            if (dresult == true)
            {
                CryptoKeyModel key = new();
                key.PublicName = InputKey;
                key.ByteLength = Encoding.UTF8.GetBytes(InputKey).Length;

                string json = JsonConvert.SerializeObject(key);
                string encoded = hashing.EncodeBotCrypt(json);

                using StreamWriter writer = new(sfd.FileName);
                writer.Write(encoded);
                writer.Close();
            }
        }
        private void OnLoadKey()
        {
            OpenFileDialog ofd = new();
            ofd.Filter = "Crypto Wizzard Key|*.cwk";
            ofd.Title = "Открытие ключа Crypto Wizzard";
            ofd.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
            bool? dresult = ofd.ShowDialog();
            if (dresult == true)
            {
                using StreamReader reader = new(ofd.FileName);
                string decoded = hashing.DecodeBotCrypt(reader.ReadToEnd());
                CryptoKeyModel key = JsonConvert.DeserializeObject<CryptoKeyModel>(decoded);

                string json = JsonConvert.SerializeObject(key);
                string encoded = hashing.EncodeBotCrypt(json);

                memory.RemoveItem("EncryptKey");
                memory.StoreItem("EncryptKey", encoded);
                CheckKey();
            }
        }
    }
}
