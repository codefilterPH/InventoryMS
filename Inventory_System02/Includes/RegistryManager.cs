using System;
using System.IO;
using Microsoft.Win32;

namespace Inventory_System02.Includes
{
    internal class RegistryManager
    {
        public static void VerifyRegistryEntries()
        {
            string registryPath = @"SOFTWARE\codefilterPH\InventoryMS";
            string serverNameValueName = "ServerName";
            string importDirValueName = "ImportsDir";
            string importDirValueData = @"\CommonSql\Imports";

            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey softwareKey = currentUserKey.OpenSubKey(registryPath, true);

            // If the SOFTWARE\codefilterPH\InventoryMS key doesn't exist then create it.
            if (softwareKey == null)
            {
                softwareKey = currentUserKey.CreateSubKey(registryPath);
            }

            string serverName = (string)softwareKey.GetValue(serverNameValueName);

            // Verify the ImportsDir
            if (softwareKey.GetValue(importDirValueName) == null || softwareKey.GetValue(importDirValueName).ToString() != importDirValueData)
            {
                softwareKey.SetValue(importDirValueName, importDirValueData, RegistryValueKind.String);
            }

            // Construct the path
            string path = $@"\\{serverName}{importDirValueData}";

            // Check and create directory if it does not exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // It's a good practice to close the keys after use.
            softwareKey.Close();
            currentUserKey.Close();
        }
    }
}
