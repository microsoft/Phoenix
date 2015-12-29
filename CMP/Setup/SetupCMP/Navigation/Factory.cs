//-----------------------------------------------------------------------
// <copyright file="Factory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Factory class for Pages
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup.SetupFramework
{
    using System;

    /// <summary>
    /// Abstract class for creating host and UI pages
    /// </summary>
    public abstract class Factory
    {
        /// <summary>
        /// Creates a window that will host pages 
        /// </summary>
        /// <returns>a IPageHost compliant window</returns>
        public abstract IPageHost CreateHost();

        /// <summary>
        /// Creates the page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="type">The type.</param>
        /// <returns>an IPageUI compliant wizard page</returns>
        public abstract IPageUI CreatePage(Page page, Type type);
    }
}
