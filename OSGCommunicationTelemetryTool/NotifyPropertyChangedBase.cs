//  
// Copyright (c) Microsoft Corporation.  All rights reserved.
// 
// 
// Use of this source code is subject to the terms of the Microsoft
// premium shared source license agreement under which you licensed
// this source code. If you did not accept the terms of the license
// agreement, you are not authorized to use this source code.
// For the terms of the license, please see the license agreement
// signed by you and Microsoft.
// THE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
//  

using System.Collections.Generic;

namespace System.ComponentModel
{
    /// <summary>
    /// The class Notify Property Changed Base.
    /// </summary>
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <summary>
        /// Raises PropertyChanged event for all properties.
        /// </summary>
        protected virtual void RaiseAllPropertyChanged()
        {
            this.RaisePropertyChanged(string.Empty);
        }

        /// <summary>
        /// Raises PropertyChanged event for specified properties.
        /// </summary>
        /// <param name="propertyNames">The property names.</param>
        protected virtual void RaisePropertyChanged(params string[] propertyNames)
        {
            if (propertyNames == null)
            {
                throw new ArgumentNullException("propertyNames");
            }

            foreach (var name in propertyNames)
            {
                this.RaisePropertyChanged(name);
            }
        }

        /// <summary>
        /// Raises PropertyChanged event for specified property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets a field. If the field has a change of value, a PropertyChanged event will be raised.
        /// </summary>
        /// <typeparam name="T">The type of the field.</typeparam>
        /// <param name="name">The property name for PropertyChanged event to raise.</param>
        /// <param name="field">The reference to the field.</param>
        /// <param name="value">The new value for the field.</param>
        /// <param name="comparer">The comparer to determine if the value has changed.</param>
        /// <returns>True if the field value has changed; false otherwise.</returns>
        protected virtual bool SetField<T>(string name, ref T field, T value, IEqualityComparer<T> comparer = null)
        {
            comparer = comparer ?? EqualityComparer<T>.Default;
            var changed = !comparer.Equals(field, value);
            if (changed)
            {
                field = value;
                this.RaisePropertyChanged(name);
            }

            return changed;
        }
    }
}
