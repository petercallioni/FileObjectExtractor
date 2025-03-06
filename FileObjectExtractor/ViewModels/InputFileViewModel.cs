using FileObjectExtractor.Models;
using System;

namespace FileObjectExtractor.ViewModels
{
    public class InputFileViewModel : ViewModelBase
    {
        private Uri? fileURI;
        private OfficeType officeType;

        public Uri? FileURI
        {
            get => fileURI; set
            {
                fileURI = value;
                OnPropertyChanged();
            }
        }
        public OfficeType OfficeType
        {
            get => officeType; set
            {
                officeType = value;
                OnPropertyChanged();
            }
        }

        public InputFileViewModel()
        {
            this.fileURI = null;
            this.officeType = OfficeType.UNKNOWN;
        }

        public InputFileViewModel(Uri fileURI, OfficeType officeType)
        {
            this.fileURI = fileURI;
            this.officeType = officeType;
        }

        public InputFileViewModel(Uri uri)
        {
            this.fileURI = uri;
            this.officeType = OfficeType.UNKNOWN;
        }
    }
}
