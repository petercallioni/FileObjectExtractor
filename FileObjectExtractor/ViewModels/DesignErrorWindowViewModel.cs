using System;

namespace FileObjectExtractor.ViewModels
{
    public class DesignErrorWindowViewModel : ErrorWindowViewModel
    {
        public DesignErrorWindowViewModel() : base(new Exception("This is a sample exception"), null!) { }
    }
}