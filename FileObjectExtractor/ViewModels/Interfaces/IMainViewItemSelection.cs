using System.Threading.Tasks;

namespace FileObjectExtractor.ViewModels.Interfaces
{
    public interface IMainViewItemSelection
    {
        public void SelectAll();
        public void SelectNone();
        public Task SaveSelectedFiles();
        public Task SelectFile();
    }
}
