//-----------------------------------------------------------------------
// <copyright file="DelegateRegistry.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This class holds default delegates used to move around the UI framework pages.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup.SetupFramework
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Singleton class to hold the delegates used to navigate the pages 
    /// </summary>
    public class DelegateRegistry
    {
        /// <summary>
        /// The unique instance of the DelegateRegistry class 
        /// </summary>
        private static DelegateRegistry instance;

        /// <summary>
        /// Holds the delegates used to navigate the pages 
        /// </summary>
        private Dictionary<string, Delegate> delegates;

        /// <summary>
        /// Prevents a default instance of the DelegateRegistry class from being created
        /// </summary>
        private DelegateRegistry()
        {
            this.delegates = new Dictionary<string, Delegate>();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static DelegateRegistry Instance
        {
            get
            {
                if (DelegateRegistry.instance == null)
                {
                    DelegateRegistry.instance = new DelegateRegistry();
                }

                return DelegateRegistry.instance;
            }
        }

        /// <summary>
        /// Gets the delegate using reflection.
        /// </summary>
        /// <param name="delegateId">The delegate id.</param>
        /// <returns>A PageCallback delegate</returns>
        public static PageNavigation.PageCallback GetDelegate(string delegateId)
        {
            if (string.IsNullOrEmpty(delegateId))
            {
                throw new ArgumentException(delegateId);
            }

            int lastDotPosition = delegateId.LastIndexOf(".", StringComparison.OrdinalIgnoreCase);
            string functionName = delegateId.Substring(lastDotPosition + 1);
            string className = delegateId.Remove(lastDotPosition);
            Type preprocessingType = Assembly.GetEntryAssembly().GetType(className, false);
            MethodInfo preprocessingMethodInfo = preprocessingType.GetMethod(functionName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return
                (PageNavigation.PageCallback)Delegate.CreateDelegate(typeof(PageNavigation.PageCallback), preprocessingMethodInfo);
        }

        /// <summary>
        /// Registers the delegate.
        /// </summary>
        /// <param name="delegateId">The delegate id.</param>
        /// <param name="delegateToRegister">The delegate to register.</param>
        public void RegisterDelegate(string delegateId, Delegate delegateToRegister)
        {
            this.delegates.Add(delegateId, delegateToRegister);
        }
    }
}
