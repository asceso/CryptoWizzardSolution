using Prism.Mvvm;

namespace CryptoWizzard.Models
{
    public class FileManagerItem : BindableBase
    {
        private int _sortPriority;
        private string _name;
        private string _path;
        private string _updateDate;
        private FileManagerExtension _extension;
        private string _size;

        public int SortPriority { get => _sortPriority; set => SetProperty(ref _sortPriority, value); }
        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Path { get => _path; set => SetProperty(ref _path, value); }
        public string UpdateDate { get => _updateDate; set => SetProperty(ref _updateDate, value); }
        public FileManagerExtension Extension { get => _extension; set => SetProperty(ref _extension, value); }
        public string Size { get => _size; set => SetProperty(ref _size, value); }
    }
    public enum FileManagerExtension
    {
        Drive, Folder, Media, Executable, File, Back, CryptedFile
    }
}
