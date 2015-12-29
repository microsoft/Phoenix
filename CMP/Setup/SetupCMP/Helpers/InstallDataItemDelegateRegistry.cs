//-----------------------------------------------------------------------
// <copyright file="InstallDataItemDelegateRegistry.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>This class holds default delegates.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup.SetupFramework
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class InstallDataItemDelegateRegistry
    {
        // This will hold the list of delegates we can use
        Dictionary<String, Delegate> preprocessDelegates = new Dictionary<String, Delegate>();
        Dictionary<String, Delegate> postProcessDelegates = new Dictionary<String, Delegate>();
        Dictionary<String, Delegate> customActions = new Dictionary<String, Delegate>();
        Dictionary<String, Delegate> prerequisiteDelegates = new Dictionary<String, Delegate>();

        private static InstallDataItemDelegateRegistry instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallDataItemDelegateRegistry"/> class.
        /// </summary>
        private InstallDataItemDelegateRegistry()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static InstallDataItemDelegateRegistry Instance
        {
            get
            {
                if (InstallDataItemDelegateRegistry.instance == null)
                {
                    InstallDataItemDelegateRegistry.instance = new InstallDataItemDelegateRegistry();
                }
                return InstallDataItemDelegateRegistry.instance;
            }
        }

        /// <summary>
        /// Registers the preprocess delegate.
        /// </summary>
        /// <param name="delegateId">The delegate id.</param>
        /// <param name="delegateToRegister">The delegate to register.</param>
        public void RegisterPreprocessDelegate(String delegateId, Delegate delegateToRegister)
        {
            preprocessDelegates.Add(delegateId, delegateToRegister);
        }

        /// <summary>
        /// Registers the post process delegate.
        /// </summary>
        /// <param name="delegateId">The delegate id.</param>
        /// <param name="delegateToRegister">The delegate to register.</param>
        public void RegisterPostProcessDelegate(String delegateId, Delegate delegateToRegister)
        {
            postProcessDelegates.Add(delegateId, delegateToRegister);
        }

        /// <summary>
        /// Registers the delegate installer
        /// </summary>
        /// <param name="delegateId">The delegate id.</param>
        /// <param name="delegateToRegister">The delegate to register.</param>
        public void RegisterCustomAction(String delegateId, Delegate delegateToRegister)
        {
            this.customActions.Add(delegateId, delegateToRegister);
        }

        /// <summary>
        /// Registers the prerequisite delegate.
        /// </summary>
        /// <param name="delegateId">The delegate id.</param>
        /// <param name="delegateToRegister">The delegate to register.</param>
        public void RegisterPrerequisiteDelegate(String delegateId, Delegate delegateToRegister)
        {
            prerequisiteDelegates.Add(delegateId, delegateToRegister);
        }

        /// <summary>
        /// Gets the preprocess delegate.
        /// </summary>
        /// <param name="delegateId">The delegate id.</param>
        /// <returns></returns>
        public Delegate GetPreprocessDelegate(String delegateId)
        {
            return preprocessDelegates[delegateId];
        }

        /// <summary>
        /// Gets the post process delegate.
        /// </summary>
        /// <param name="delegateId">The delegate id.</param>
        /// <returns></returns>
        public Delegate GetPostProcessDelegate(String delegateId)
        {
            return postProcessDelegates[delegateId];
        }

        /// <summary>
        /// Gets the delegate installer.
        /// </summary>
        /// <param name="delegateId">The delegate id.</param>
        /// <returns></returns>
        public Delegate GetCustomAction(String delegateId)
        {
            return this.customActions[delegateId];
        }

        /// <summary>
        /// Gets the prerequisite delegate.
        /// </summary>
        /// <param name="delegateId">The delegate id.</param>
        /// <returns></returns>
        public Delegate GetPrerequisiteDelegate(String delegateId)
        {
            return prerequisiteDelegates[delegateId];
        }
    }
}