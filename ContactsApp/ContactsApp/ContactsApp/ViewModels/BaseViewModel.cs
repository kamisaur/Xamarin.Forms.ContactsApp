using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ContactsApp.ViewModels
{
	public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// Raises the property changed event.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") 
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	}
}
