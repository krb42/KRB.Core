using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace KRB.Core.Toolset
{
   /// <summary>
   /// Provides a thread-safe dictionary for use with data binding.
   /// </summary>
   /// <typeparam name="TKey">Specifies the type of the keys in this collection.</typeparam>
   /// <typeparam name="TValue">Specifies the type of the values in this collection.</typeparam>
   /// <remarks>
   /// This class is an updated version  of the ObservableConcurrentDictionary class from the Parallel Extensions Extras project
   /// Microsoft provides as part of their Samples for Parallel Programming with the .NET Framework (https://code.msdn.microsoft.com/ParExtSamples)
   /// under MS-LPL license.
   /// The original notification to the observer sent a NotifyCollectionChangedEventArgs with the Action set to NotifyCollectionChangedAction.Reset
   /// The update class aims to pass the updated information via the other properties of the NotifyCollectionChangedEventArgs object.
   ///</remarks>
   [DebuggerDisplay("Count={Count}")]
   public class ObservableConcurrentDictionary<TKey, TValue> :
        ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>,
        INotifyCollectionChanged, INotifyPropertyChanged
   {
      private readonly SynchronizationContext _context;
      private readonly ConcurrentDictionary<TKey, TValue> _dictionary;

      private void BaseItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (sender != null)
         {
            NotifyObserversOfChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender), managePropertyChangedEvents: false);
         }
      }

      /// <summary>
      /// Attempts to manage the PropertyChanged event for the items in the dictionary
      /// </summary>
      /// <param name="collectionChangedArgs">
      /// The NotifyCollectionChangedEventArgs class
      /// </param>
      private void ManagePropertyChangedEvent(NotifyCollectionChangedEventArgs collectionChangedArgs)
      {
         ManagePropertyChangedEvent(collectionChangedArgs.Action, collectionChangedArgs.NewItems, collectionChangedArgs.OldItems);
      }

      /// <summary>
      /// Attempts to manage the PropertyChanged event for the items in the dictionary
      /// </summary>
      /// <param name="action">
      /// The NotifyCollectionChanged action
      /// </param>
      /// <param name="newItems">
      /// Items which are being or have been added
      /// </param>
      /// <param name="oldItems">
      /// Items which are being or have been removed
      /// </param>
      private void ManagePropertyChangedEvent(NotifyCollectionChangedAction action, IList newItems, IList oldItems)
      {
         void RegisterPropertyChanged(INotifyPropertyChanged item)
         {
            if (item != null)
            {
               item.PropertyChanged += new PropertyChangedEventHandler(BaseItem_PropertyChanged);
            }
         }

         void UnRegisterPropertyChanged(INotifyPropertyChanged item)
         {
            if (item != null)
            {
               item.PropertyChanged -= new PropertyChangedEventHandler(BaseItem_PropertyChanged);
            }
         }

         if (action == NotifyCollectionChangedAction.Add && newItems != null)
         {
            foreach (INotifyPropertyChanged item in newItems)
            {
               RegisterPropertyChanged(item);
            }
         }
         else if ((action == NotifyCollectionChangedAction.Remove || action == NotifyCollectionChangedAction.Reset) && oldItems != null)
         {
            foreach (INotifyPropertyChanged item in oldItems)
            {
               UnRegisterPropertyChanged(item);
            }
         }
         else if (action == NotifyCollectionChangedAction.Replace)
         {
            if (newItems != null)
            {
               foreach (INotifyPropertyChanged item in oldItems)
               {
                  UnRegisterPropertyChanged(item);
               }
            }
            if (oldItems != null)
            {
               foreach (INotifyPropertyChanged item in newItems)
               {
                  RegisterPropertyChanged(item);
               }
            }
         }
      }

      /// <summary>
      /// Notifies observers of CollectionChanged or PropertyChanged of an update to the dictionary.
      /// </summary>
      /// <param name="collectionChangedArgs">
      /// </param>
      private void NotifyObserversOfChange(NotifyCollectionChangedEventArgs collectionChangedArgs, bool managePropertyChangedEvents)
      {
         var collectionHandler = CollectionChanged;
         var propertyHandler = PropertyChanged;
         if (collectionHandler != null || propertyHandler != null)
         {
            _context.Post(s =>
            {
               if (collectionHandler != null && collectionChangedArgs != null)
               {
                  if (managePropertyChangedEvents)
                  {
                     ManagePropertyChangedEvent(collectionChangedArgs);
                  }
                  collectionHandler(this, collectionChangedArgs);
               }
               if (propertyHandler != null)
               {
                  if (collectionChangedArgs.Action != NotifyCollectionChangedAction.Replace)
                  {
                     propertyHandler(this, new PropertyChangedEventArgs("Count"));
                     propertyHandler(this, new PropertyChangedEventArgs("Keys"));
                  }
                  propertyHandler(this, new PropertyChangedEventArgs("Values"));
               }
            }, null);
         }
      }

      /// <summary>
      /// Attempts to add an item to the dictionary, notifying observers of any changes.
      /// </summary>
      /// <param name="item">
      /// The item to be added.
      /// </param>
      /// <returns>
      /// Whether the add was successful.
      /// </returns>
      private bool TryAddWithNotification(KeyValuePair<TKey, TValue> item)
      {
         return TryAddWithNotification(item.Key, item.Value);
      }

      /// <summary>
      /// Attempts to add an item to the dictionary, notifying observers of any changes.
      /// </summary>
      /// <param name="key">
      /// The key of the item to be added.
      /// </param>
      /// <param name="value">
      /// The value of the item to be added.
      /// </param>
      /// <returns>
      /// Whether the add was successful.
      /// </returns>
      private bool TryAddWithNotification(TKey key, TValue value)
      {
         bool result = _dictionary.TryAdd(key, value);
         var index = _dictionary.Keys.ToList().IndexOf(key);
         if (result) NotifyObserversOfChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index), managePropertyChangedEvents: true);
         return result;
      }

      /// <summary>
      /// Attempts to remove an item from the dictionary, notifying observers of any changes.
      /// </summary>
      /// <param name="key">
      /// The key of the item to be removed.
      /// </param>
      /// <param name="value">
      /// The value of the item removed.
      /// </param>
      /// <returns>
      /// Whether the removal was successful.
      /// </returns>
      private bool TryRemoveWithNotification(TKey key, out TValue value)
      {
         int index = -1;
         if (_dictionary.ContainsKey(key))
         {
            index = _dictionary.Keys.ToList().IndexOf(key);
         }
         bool result = _dictionary.TryRemove(key, out value);
         if (result) NotifyObserversOfChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index), managePropertyChangedEvents: true);
         return result;
      }

      /// <summary>
      /// Attempts to add or update an item in the dictionary, notifying observers of any changes.
      /// </summary>
      /// <param name="key">
      /// The key of the item to be updated.
      /// </param>
      /// <param name="value">
      /// The new value to set for the item.
      /// </param>
      /// <returns>
      /// Whether the update was successful.
      /// </returns>
      private void UpdateWithNotification(TKey key, TValue value)
      {
         int index = -1;
         if (!_dictionary.ContainsKey(key))
         {
            TryAddWithNotification(key, value);
            return;
         }

         index = _dictionary.Keys.ToList().IndexOf(key);
         var oldvalue = _dictionary[key];
         _dictionary[key] = value;
         NotifyObserversOfChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldvalue, index), managePropertyChangedEvents: true);
      }

      /// <summary>
      /// Initializes an instance of the ObservableConcurrentDictionary class.
      /// </summary>
      public ObservableConcurrentDictionary()
      {
         _context = AsyncOperationManager.SynchronizationContext;
         _dictionary = new ConcurrentDictionary<TKey, TValue>();
      }

      /// <summary>
      /// Event raised when the collection changes.
      /// </summary>
      public event NotifyCollectionChangedEventHandler CollectionChanged;

      /// <summary>
      /// Event raised when a property on the collection changes.
      /// </summary>
      public event PropertyChangedEventHandler PropertyChanged;

      #region ICollection<KeyValuePair<TKey,TValue>> Members

      int ICollection<KeyValuePair<TKey, TValue>>.Count
      {
         get { return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count; }
      }

      bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
      {
         get { return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly; }
      }

      void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
      {
         TryAddWithNotification(item);
      }

      void ICollection<KeyValuePair<TKey, TValue>>.Clear()
      {
         ManagePropertyChangedEvent(NotifyCollectionChangedAction.Reset, null, _dictionary.Values.ToList());
         ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
         NotifyObserversOfChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), managePropertyChangedEvents: false);
      }

      bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
      {
         return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
      }

      void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
      {
         ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
      }

      bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
      {
         return TryRemoveWithNotification(item.Key, out TValue temp);
      }

      #endregion ICollection<KeyValuePair<TKey,TValue>> Members

      #region IEnumerable<KeyValuePair<TKey,TValue>> Members

      IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
      {
         return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
      }

      #endregion IEnumerable<KeyValuePair<TKey,TValue>> Members

      #region IDictionary<TKey,TValue> Members

      public ICollection<TKey> Keys
      {
         get { return _dictionary.Keys; }
      }

      public ICollection<TValue> Values
      {
         get { return _dictionary.Values; }
      }

      public TValue this[TKey key]
      {
         get { return _dictionary[key]; }
         set { UpdateWithNotification(key, value); }
      }

      public void Add(TKey key, TValue value)
      {
         TryAddWithNotification(key, value);
      }

      public bool ContainsKey(TKey key)
      {
         return _dictionary.ContainsKey(key);
      }

      public bool Remove(TKey key)
      {
         return TryRemoveWithNotification(key, out TValue temp);
      }

      public bool TryGetValue(TKey key, out TValue value)
      {
         return _dictionary.TryGetValue(key, out value);
      }

      #endregion IDictionary<TKey,TValue> Members
   }
}
