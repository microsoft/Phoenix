//-----------------------------------------------------------------------
// <copyright file="PropertyBag.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Class used store global application variables.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup.SetupFramework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Xml;

    /// <summary>
    /// Class for the property bag
    /// </summary>
    public class PropertyBagDictionary : IDictionary<string, object>
    {
        // List the properties here so there are no typeo's
        public const string UseDebugMode = "usedebugmode";
        public const string Silent = "silent";
        public const string SetupExePath = "setupexepath";
        public const string ProtectedProperties = "ProtectedProperties";

        public const string Rollback = "rollback";
        public const string LastFailureLog = "lastfailurelog";
        public const string RebootRequired = "rebootrequired";
        public const string VitalFailure = "vitalfailure";
        public const string NonVitalFailure = "nonvitalfailure";

        // General install tracking
        public const string ExecutableInstallDone = "executableinstalldone";
        public const string MicrosoftInstallerInstallDone = "microsoftinstallerinstalldone";

        private static PropertyBagDictionary instance;

        /// <summary>
        /// Used to hold the propertyBag refs.
        /// </summary>
        private Dictionary<string, object> propertyBag = new Dictionary<string, object>();

        /// <summary>
        /// Prevents a default instance of the PropertyBagDictionary class from being created.
        /// </summary>
        private PropertyBagDictionary()
        {
            // Do nothing
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static PropertyBagDictionary Instance
        {
            get
            {
                if (PropertyBagDictionary.instance == null)
                {
                    PropertyBagDictionary.instance = new PropertyBagDictionary();
                }

                return PropertyBagDictionary.instance;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>the key</value>
        public object this[string key]
        {
            get { return this.propertyBag[key]; }
            set { this.propertyBag[key] = value; }
        }

        /// <summary>
        /// Adds to protected property list.
        /// </summary>
        /// <param name="property">The property.</param>
        public static void AddToProtectedPropertyList(string property)
        {
            ArrayList protectedProperties;

            if (!PropertyBagDictionary.instance.PropertyExists(PropertyBagDictionary.ProtectedProperties))
            {
                protectedProperties = new ArrayList();
            }
            else
            {
                protectedProperties = PropertyBagDictionary.instance.GetProperty<ArrayList>(PropertyBagDictionary.ProtectedProperties);
            }

            protectedProperties.Add(property);
            PropertyBagDictionary.Instance.SafeAdd(PropertyBagDictionary.ProtectedProperties, protectedProperties);
        }

        /// <summary>
        /// Determines whether [is protected property] [the specified property].
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        ///     <c>true</c> if [is protected property] [the specified property]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsProtectedProperty(string property)
        {
            if (!PropertyBagDictionary.instance.PropertyExists(PropertyBagDictionary.ProtectedProperties))
            {
                return false;
            }

            ArrayList protectedProperties = PropertyBagDictionary.instance.GetProperty<ArrayList>(PropertyBagDictionary.ProtectedProperties);

            if (protectedProperties.Contains(property))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Properties the exists.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>bool</returns>
        public bool PropertyExists(string property)
        {
            return this.propertyBag.ContainsKey(property);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="T">T</typeparam>
        /// <param name="property">The property.</param>
        /// <returns>the property value</returns>
        public T GetProperty<T>(string property)
        {
            T value = default(T);
            object temp;
            if (this.propertyBag.TryGetValue(property, out temp))
            {
                try
                {
                    if (temp is IConvertible)
                    {
                        value = (T)Convert.ChangeType(temp, typeof(T), CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        value = (T)temp;
                    }
                }
                catch (InvalidCastException)
                {
                    value = (T)temp;
                }
            }

            return value;
        }

        /// <summary>
        /// Safely adds a key/value pair to the property bag.
        /// If key is present, updates the property; otherwise adds the key/value pair.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SafeAdd(string key, object value)
        {
            if (this.propertyBag.ContainsKey(key))
            {
                this.propertyBag[key] = value;
            }
            else
            {
                this.Add(key, value);
            }
        }

        /// <summary>
        /// Removes the key/value pair from the property bag if the specified key 
        /// is present. Otherwise, does nothing. 
        /// </summary>
        /// <param name="key">The key that should be removed, if present</param>
        public void SafeRemove(string key)
        {
            if (this.propertyBag.ContainsKey(key))
            {
                this.Remove(key);
            }
        }

        #region IDictionary<string,object> Members

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public void Add(string key, object value)
        {
            this.propertyBag.Add(key, value);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.</exception>
        public bool ContainsKey(string key)
        {
            return this.propertyBag.ContainsKey(key);
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.</returns>
        public ICollection<string> Keys
        {
            get { return this.propertyBag.Keys; }
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key"/> is null.</exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public bool Remove(string key)
        {
            return this.propertyBag.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key"/> is null.</exception>
        public bool TryGetValue(string key, out object value)
        {
            return this.propertyBag.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.</returns>
        public ICollection<object> Values
        {
            get { return this.propertyBag.Values; }
        }

        #endregion

        #region ICollection<KeyValuePair<string,object>> Members

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(KeyValuePair<string, object> item)
        {
            this.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear()
        {
            this.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return this.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array"/> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <paramref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            this.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
        public int Count
        {
            get { return this.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get { return this.IsReadOnly; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(KeyValuePair<string, object> item)
        {
            return this.Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.propertyBag.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
