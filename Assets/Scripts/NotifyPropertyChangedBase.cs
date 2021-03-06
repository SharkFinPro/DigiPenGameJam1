﻿/**************************
 * File: NotifyPropertyChangedBase
 * Author: Flynn Duniho
 * Description: Provides a base for implementing events when properties are changed
**************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class NotifyPropertyChangedBase : MonoBehaviour, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void UpdateField<T>(ref T field, T newValue,
            Action<T> onChanged = null,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return;
            }

            T oldValue = field;
            field = newValue;
            onChanged?.Invoke(oldValue);
            OnPropertyChanged(propertyName);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
