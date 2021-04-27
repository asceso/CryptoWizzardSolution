using CryptoWizzard.Models;
using CryptoWizzard.Views;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Services.HashingService;
using Services.MemoryService;
using Services.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace CryptoWizzard.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IMemoryService memory;
        private readonly IHashingService hashing;
        private string[] mediaExtensions = {
                ".PNG", ".JPG", ".JPEG", ".BMP", ".GIF", ".ICO",
                ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA",
                ".AVI", ".MP4", ".DIVX", ".WMV",
            };
        private int tempCounter = 1;

        private bool _isBackEnabled;
        private bool _isEncryptEnabled;
        private bool _isDecryptEnabled;
        private string _title;
        private string _currentPath;
        private ObservableCollection<FileManagerItem> _managerItems;
        private WindowState _currentWindowState;

        public bool IsBackEnabled { get => _isBackEnabled; set => SetProperty(ref _isBackEnabled, value); }
        public bool IsEncryptEnabled { get => _isEncryptEnabled; set => SetProperty(ref _isEncryptEnabled, value); }
        public bool IsDecryptEnabled { get => _isDecryptEnabled; set => SetProperty(ref _isDecryptEnabled, value); }
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string CurrentPath { get => _currentPath; set => SetProperty(ref _currentPath, value); }
        public ObservableCollection<FileManagerItem> ManagerItems { get => _managerItems ??= new(); set => SetProperty(ref _managerItems, value); }
        public WindowState CurrentWindowState { get => _currentWindowState; set => SetProperty(ref _currentWindowState, value); }

        public DelegateCommand MinimizeCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand LoadedCommand { get; set; }
        public DelegateCommand OpenKeysCommand { get; set; }
        public DelegateCommand BackOnPathCommand { get; set; }
        public DelegateCommand RefreshDirectoryCommand { get; set; }
        public DelegateCommand<object> DoubleClickGridItemCommand { get; set; }
        public DelegateCommand<object> SelectedChangedCommand { get; set; }
        public DelegateCommand<object> EcnryptFileCommand { get; set; }
        public DelegateCommand<object> DecryptFileCommand { get; set; }
        public DelegateCommand<object> OpenEncryptedCommand { get; set; }

        public MainWindowViewModel(IMemoryService memory, IHashingService hashing)
        {
            this.memory = memory;
            this.hashing = hashing;
            Title = "Криптографический блочный шифр с ключом переменной длины";
            MinimizeCommand = new DelegateCommand(OnMinimize);
            CloseCommand = new DelegateCommand(OnClose);
            LoadedCommand = new DelegateCommand(OnLoaded);
            OpenKeysCommand = new DelegateCommand(OnOpenKeys);
            BackOnPathCommand = new DelegateCommand(OnBackwardPathNavigation);
            RefreshDirectoryCommand = new DelegateCommand(RefreshDirectory);
            DoubleClickGridItemCommand = new DelegateCommand<object>(OnGridDoubleCLick);
            SelectedChangedCommand = new DelegateCommand<object>(OnSelectedChanged);
            EcnryptFileCommand = new DelegateCommand<object>(OnEncryptFile);
            DecryptFileCommand = new DelegateCommand<object>(OnDecryptFile);
            OpenEncryptedCommand = new DelegateCommand<object>(OnOpenEncrypted);
            SelectedChangedCommand = new DelegateCommand<object>(OnSelectedChanged);
            IsEncryptEnabled = false;
            IsDecryptEnabled = false;
        }
        private void OnMinimize() => CurrentWindowState = CurrentWindowState == WindowState.Normal ? WindowState.Minimized : WindowState.Normal;
        private void OnClose() => Application.Current.Shutdown();
        private void OnLoaded() => ReadDrives();
        private void OnOpenKeys()
        {
            KeysWindow kw = new(memory, hashing);
            kw.ShowDialog();
        }

        private void OnGridDoubleCLick(object grid_item)
        {
            if (grid_item is FileManagerItem fmi)
            {
                if (fmi.Extension != FileManagerExtension.Drive && fmi.Extension != FileManagerExtension.Folder && fmi.Extension != FileManagerExtension.Back) return;
                OnChangeDirectory(fmi);
            }
        }
        private void OnSelectedChanged(object grid_item)
        {
            if (grid_item is FileManagerItem fmi)
            {
                IsEncryptEnabled = false;
                IsDecryptEnabled = false;
                if (fmi.Extension == FileManagerExtension.CryptedFile)
                {
                    IsEncryptEnabled = false;
                    IsDecryptEnabled = true;
                    return;
                }
                if (fmi.Extension != FileManagerExtension.File && fmi.Extension != FileManagerExtension.Media && fmi.Extension != FileManagerExtension.Executable) return;
                IsEncryptEnabled = true;
                IsDecryptEnabled = false;
            }
        }
        private void OnBackwardPathNavigation()
        {
            FileManagerItem back_fmi = ManagerItems.FirstOrDefault(i => i.Name == "...");
            if (back_fmi == null) return;
            DirectoryInfo dir = new(back_fmi.Path);
            if (dir.Parent != null)
            {
                ManagerItems.Clear();
                ManagerItems.Add(new FileManagerItem()
                {
                    Name = "...",
                    Path = dir.Parent.FullName,
                    Extension = FileManagerExtension.Back
                });
                ReadFolders(dir.Parent.FullName);
                ReadFiles(dir.Parent.FullName);
                CurrentPath = dir.Parent.FullName;
                return;
            }
            if (dir.Root != null)
            {
                ReadDrives();
            }
        }
        private async void OnEncryptFile(object grid_item)
        {
            if (grid_item is FileManagerItem fmi)
            {
                string decoded_key = await memory.GetItem<string>("EncryptKey");
                if (decoded_key == null)
                {
                    ErrorWindow error = new("Ключ шифрования не установлен");
                    error.ShowDialog();
                    return;
                }
                string json = hashing.DecodeBotCrypt(decoded_key);
                CryptoKeyModel key = JsonConvert.DeserializeObject<CryptoKeyModel>(json);
                hashing.EncodeBlowFish(key, fmi.Path);
                File.Delete(fmi.Path);
                RefreshDirectory();
            }
        }
        private async void OnDecryptFile(object grid_item)
        {
            if (grid_item is FileManagerItem fmi)
            {
                string decoded_key = await memory.GetItem<string>("EncryptKey");
                if (decoded_key == null)
                {
                    ErrorWindow error = new("Ключ шифрования не установлен");
                    error.ShowDialog();
                    return;
                }
                string json = hashing.DecodeBotCrypt(decoded_key);
                CryptoKeyModel key = JsonConvert.DeserializeObject<CryptoKeyModel>(json);
                var result = hashing.DecodeBlowFish(key, fmi.Path);
                if (result == false)
                {
                    ErrorWindow error = new("Ключ шифрования не подходит");
                    error.ShowDialog();
                    return;
                }
                File.Delete(fmi.Path);
                RefreshDirectory();
            }
        }
        private async void OnOpenEncrypted(object grid_item)
        {
            if (grid_item is FileManagerItem fmi)
            {
                string decoded_key = await memory.GetItem<string>("EncryptKey");
                if (decoded_key == null)
                {
                    ErrorWindow error = new("Ключ шифрования не установлен");
                    error.ShowDialog();
                    return;
                }
                string json = hashing.DecodeBotCrypt(decoded_key);
                CryptoKeyModel key = JsonConvert.DeserializeObject<CryptoKeyModel>(json);
                var result = hashing.DecodeBlowFish(key, fmi.Path, out string output_filename);
                if (result == false)
                {
                    ErrorWindow error = new("Ключ шифрования не подходит");
                    error.ShowDialog();
                    return;
                }
                string tempFileName = Environment.CurrentDirectory + $"\\temp\\tmp{tempCounter}.{output_filename.Remove(0, output_filename.LastIndexOf('.') + 1)}";
                File.Move(output_filename, tempFileName);
                tempCounter++;
                
                FileInfo tfi = new(tempFileName);
                using StreamWriter writer = new(Environment.CurrentDirectory + $"\\temp\\run.cmd");
                writer.Write($"{tfi.FullName}");
                writer.Close();

                ProcessStartInfo processStart = new();
                processStart.FileName = Environment.CurrentDirectory + $"\\temp\\run.cmd";
                processStart.CreateNoWindow = true;
                processStart.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(processStart);
            }
        }

        private string GetByteString(long value)
        {
            var kb = value / 1024;
            var mb = kb / 1024;
            return $"{value} б ( {kb} кб, {mb} мб )";
        }
        private void ReadDrives()
        {
            ManagerItems.Clear();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (var drive in allDrives)
            {
                if (!drive.IsReady) continue;
                ManagerItems.Add(new FileManagerItem()
                {
                    Name = drive.Name,
                    Path = drive.RootDirectory.FullName,
                    Extension = FileManagerExtension.Drive,
                    Size = GetByteString(drive.TotalSize)
                });
            }
            CurrentPath = "Root";
            IsBackEnabled = false;
        }
        private void ReadFolders(string path)
        {
            DirectoryInfo dir = new(path);
            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                ManagerItems.Add(new FileManagerItem()
                {
                    Name = subdir.Name,
                    Path = subdir.FullName,
                    UpdateDate = subdir.LastAccessTime.ToString(),
                    Extension = FileManagerExtension.Folder
                });
            }
        }
        private void ReadFiles(string path)
        {
            DirectoryInfo dir = new(path);
            foreach (FileInfo subfile in dir.GetFiles())
            {
                ManagerItems.Add(new FileManagerItem()
                {
                    Name = subfile.Name,
                    Path = subfile.FullName,
                    UpdateDate = subfile.LastAccessTime.ToString(),
                    Extension = GetFileManagerExtension(subfile.Extension),
                    Size = GetByteString(subfile.Length)
                });
            }
        }
        private FileManagerExtension GetFileManagerExtension(string extension)
        {
            if (extension == ".exe") return FileManagerExtension.Executable;
            else if (extension == ".cwf") return FileManagerExtension.CryptedFile;
            else if (mediaExtensions.Any(e => e.ToLower() == extension.ToLower())) return FileManagerExtension.Media;
            else return FileManagerExtension.File;
        }
        private void OnChangeDirectory(FileManagerItem fmi)
        {
            IsBackEnabled = true;
            if (fmi.Name == "...")
            {
                OnBackwardPathNavigation();
            }
            else
            {
                CurrentPath = fmi.Path;
                ManagerItems.Clear();
                if (fmi.Extension == FileManagerExtension.Drive || fmi.Extension == FileManagerExtension.Folder)
                {
                    ManagerItems.Add(new FileManagerItem()
                    {
                        Name = "...",
                        Path = fmi.Path,
                        Extension = FileManagerExtension.Back,
                        SortPriority = 1
                    });
                }
                ReadFolders(fmi.Path);
                ReadFiles(fmi.Path);
            }
        }
        private void RefreshDirectory()
        {
            if (CurrentPath == "Root")
            {
                ReadDrives();
                return;
            }
            FileManagerItem back_fmi = ManagerItems.FirstOrDefault(i => i.Name == "...");
            ManagerItems.Clear();
            ManagerItems.Add(back_fmi);
            ReadFolders(CurrentPath);
            ReadFiles(CurrentPath);
        }
    }
}
