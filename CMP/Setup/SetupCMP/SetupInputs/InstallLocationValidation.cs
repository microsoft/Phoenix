// Copyright (c) Microsoft Corporation.  All rights reserved.
using System;
using System.Globalization;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CMP.Setup
{
    using CMP.Setup.SetupFramework;
    using CMP.Setup.Helpers;

    /// <summary>
    /// Summary description for InstallLocationValidation.
    /// </summary>
    public class InstallLocationValidation
    {
        private InstallLocationValidation()
        {
            
        }

        private static InstallLocationValidation installLocationValidation = new InstallLocationValidation();

        public static InstallLocationValidation Instance
        {
            get
            {
                return installLocationValidation;
            }
        }

        public void ThrowInvalidLocationException(string installLocation)
        {
            throw new Exception(
                String.Format("The selected location {0} is read-only, hidden, a system folder, or a root volume", installLocation));
        }

        public void CheckForRemovableMedia(String path)
        {

            if (!PathHelper.IsPathRooted(path))
            {
                ThrowInvalidLocationException(path);
            }

            String driveName = PathHelper.GetPathRoot(path);

            int driveType = CMP.Setup.Helpers.NativeMethods.GetDriveType(driveName);

            switch (driveType)
            {
                case CMP.Setup.Helpers.NativeMethods.DRIVE_UNKNOWN: // The drive type cannot be determined. 
                case CMP.Setup.Helpers.NativeMethods.DRIVE_NO_ROOT_DIR: // The root path is invalid. For example, no volume is mounted at the path. 
                    {
                        ThrowInvalidLocationException(path);
                        break;
                    }
                case CMP.Setup.Helpers.NativeMethods.DRIVE_REMOVABLE: // The disk can be removed from the drive. 
                case CMP.Setup.Helpers.NativeMethods.DRIVE_REMOTE: // The drive is a remote (network) drive. 
                case CMP.Setup.Helpers.NativeMethods.DRIVE_CDROM: // The drive is a CD-ROM drive. 
                case CMP.Setup.Helpers.NativeMethods.DRIVE_RAMDISK: // The drive is a RAM disk.
                    {
                        throw new Exception("The location cannot be on removable media or a network share");
                    }
                case CMP.Setup.Helpers.NativeMethods.DRIVE_FIXED: // The disk cannot be removed from the drive. 
                    {
                        break;
                    }
                default:
                    {
                        throw new Exception("Switching over drive type returned by GetDriveType");
                        break;
                    }
            }
        }

        public void CheckForDirectoryAttributes(String path)
        {
            CheckForDirectoryAttributes(path, false);
        }

        public void CheckForDirectoryAttributes(String path, bool forDB)
        {
            AppAssert.AssertNotNull(path, "path");
            try
            {
                FileAttributes attributes = File.GetAttributes(path);

                if ((int)attributes == -1)
                {

                    throw new Exception(String.Format("The attributes of directory {0} cannot be acquired", path));
                }

                if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                    (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly ||
                    (attributes & FileAttributes.System) == FileAttributes.System)
                {
                    ThrowInvalidLocationException(path);
                }

                if (forDB)
                {
                    if ((attributes & FileAttributes.Compressed) == FileAttributes.Compressed)
                    {
                        throw new Exception(
                            String.Format("The selected location {0} is on a compressed volume", path));
                    }
                }
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                // Do nothing
            }
            catch (FileNotFoundException fileNotFoundException)
            {

                // Do nothing
            }
            catch (UnauthorizedAccessException unauthorizedAccessException)
            {
                // Setup was denied access to the folder, ask user to choose other location.
                ThrowInvalidLocationException(path);
            }
            catch (IOException ioException)
            {
                // IO error while accessing the install location, ask user to choose other location.
                ThrowInvalidLocationException(path);
            }
        }

        public UInt64 GetFreeDiskSpace(String folderPath)
        {
            if (folderPath == null)
            {
                throw new ArgumentNullException("folderPath");
            }
            SetupLogger.LogInfo("GetFreeDiskSpace");

            SetupLogger.LogData("Folder Path: ", folderPath);
            UInt64 freeBytes = 0;
            UInt64 totalBytes = 0;
            UInt64 totalfreeBytes = 0;

            String validFolderPath = PathHelper.GetExistingAncestor(folderPath);

            InstallLocationValidation.GetDiskFreeSpace(validFolderPath, ref freeBytes, ref totalBytes, ref totalfreeBytes);

            // Convert the bytes in Megabytes
            freeBytes = freeBytes / (UInt64)Math.Pow(2, 20);

            SetupLogger.LogData("FreeSpace: ", freeBytes.ToString());
            return freeBytes;
        }

        /// <summary>
        /// Calls the native GetDiskFreeSpaceEx.
        /// Throws exception if the native call fails.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="freeBytes"></param>
        /// <param name="totalBytes"></param>
        /// <param name="totalfreeBytes"></param>
        public static void GetDiskFreeSpace(string directory, ref UInt64 freeBytes, ref UInt64 totalBytes, ref UInt64 totalfreeBytes)
        {
            bool nativeResult = NativeMethods.GetDiskFreeSpaceEx(directory, ref freeBytes, ref totalBytes, ref totalfreeBytes);

            if (nativeResult == false)
            {
                throw new Exception("Disk space cannot be calculated");
            }
        }
    }
}
