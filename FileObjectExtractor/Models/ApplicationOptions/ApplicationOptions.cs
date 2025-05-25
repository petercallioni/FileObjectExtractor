using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileObjectExtractor.Models.ApplicationOptions
{
    public class ApplicationOptions
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ApplicationOptions()
        {
            // Subscribe to automatically save whenever any property changes.
            PropertyChanged += HandlePropertyChanged;
        }

        private bool _checkForUpdateOnStartup = false;
        public bool CheckForUpdateOnStartup
        {
            get => _checkForUpdateOnStartup;
            set => SetProperty(_checkForUpdateOnStartup, value);
        }

        protected bool SetProperty<T>(T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
                return false;

            field = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Handler for the PropertyChanged event that saves the file whenever any property changes.
        /// </summary>
        private void HandlePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Save();
        }

        public void Save()
        {
            ApplicationOptionsManager.SaveOptions(this);
        }

    }
}