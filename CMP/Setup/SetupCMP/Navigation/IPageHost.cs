//-----------------------------------------------------------------------
// <copyright file="IPageHost.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary> Host interface for the pages.
// </summary>
//-----------------------------------------------------------------------
namespace CMP.Setup.SetupFramework
{
    using System;

    /// <summary>
    /// The Interface host pages must implement.
    /// </summary>
    public interface IPageHost
    {
        /// <summary>
        /// The Host Page must handle the Closed Event 
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Sets the page.
        /// </summary>
        /// <param name="page">The page.</param>
        void SetPage(IPageUI page);

        /// <summary>
        /// Shows this instance.
        /// </summary>
        void Show();

        /// <summary>
        /// Closes this instance.
        /// </summary>
        void Close();

        /// <summary>
        /// Shows the help.
        /// </summary>
        void ShowHelp();

        /// <summary>
        /// Sets the state of the previous button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        void SetPreviousButtonState(bool visibleState, bool enabledState);

        /// <summary>
        /// Sets the state of the previous button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        /// <param name="buttonText">The button text.</param>
        void SetPreviousButtonState(bool visibleState, bool enabledState, string buttonText);

        /// <summary>
        /// Sets the state of the next button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        void SetNextButtonState(bool visibleState, bool enabledState);

        /// <summary>
        /// Sets the state of the next button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        /// <param name="buttonText">The button text.</param>
        void SetNextButtonState(bool visibleState, bool enabledState, string buttonText);

        /// <summary>
        /// Sets the state of the cancel button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        void SetCancelButtonState(bool visibleState, bool enabledState);

        /// <summary>
        /// Sets the state of the cancel button.
        /// </summary>
        /// <param name="visibleState">if set to <c>true</c> [visible state].</param>
        /// <param name="enabledState">if set to <c>true</c> [enabled state].</param>
        /// <param name="buttonText">The button text.</param>
        void SetCancelButtonState(bool visibleState, bool enabledState, string buttonText);
    }
}
