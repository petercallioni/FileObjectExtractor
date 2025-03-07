using System;

namespace FileObjectExtractor.ViewModels.Design
{
    public class DesignErrorWindowViewModel : ErrorWindowViewModel
    {
        public DesignErrorWindowViewModel() : base(new Exception("This is a sample exception"), null!)
        {

        }
    }
}