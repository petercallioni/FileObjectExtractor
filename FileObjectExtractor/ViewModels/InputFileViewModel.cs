using Avalonia.Media;
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

        public SolidColorBrush OfficeColour
        {
            get
            {
                switch (officeType)
                {
                    case OfficeType.WORD:
                        return SolidColorBrush.Parse("#108EF4");
                    case OfficeType.EXCEL:
                        return SolidColorBrush.Parse("#3DC33A");
                    case OfficeType.POWERPOINT:
                        return SolidColorBrush.Parse("#D98B23");
                    default:
                        return SolidColorBrush.Parse("#282828");
                }
            }
        }

        public InputFileViewModel() : base(null!)
        {
            this.fileURI = null;
            this.officeType = OfficeType.UNKNOWN;
        }

        public InputFileViewModel(Uri fileURI, OfficeType officeType) : base(null!)
        {
            this.fileURI = fileURI;
            this.officeType = officeType;
        }

        public InputFileViewModel(Uri uri) : base(null!)
        {
            this.fileURI = uri;
            this.officeType = OfficeType.UNKNOWN;
        }
    }
}
