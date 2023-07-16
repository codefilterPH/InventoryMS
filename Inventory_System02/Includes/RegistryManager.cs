using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Inventory_System02.Includes
{
    internal class RegistryManager
    {
        public static void VerifyRegistryEntries()
        {
            string registryPath = @"SOFTWARE\codefilterPH\InventoryMS";
            string serverValueName = "ServerName";
            string importDirValueName = "ImportsDir";

            string serverValueData = "YourServerName"; // replace this with your actual server name
            string importDirValueData = @"\CommonSql\Imports";

            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey softwareKey = currentUserKey.OpenSubKey(registryPath, true);

            // If the SOFTWARE\codefilterPH\InventoryMS key doesn't exist then create it.
            if (softwareKey == null)
            {
                softwareKey = currentUserKey.CreateSubKey(registryPath);
            }

            // Verify the ServerName
            if (softwareKey.GetValue(serverValueName) == null || softwareKey.GetValue(serverValueName).ToString() != serverValueData)
            {
                softwareKey.SetValue(serverValueName, serverValueData, RegistryValueKind.String);
            }

            // Verify the ImportsDir
            if (softwareKey.GetValue(importDirValueName) == null || softwareKey.GetValue(importDirValueName).ToString() != importDirValueData)
            {
                softwareKey.SetValue(importDirValueName, importDirValueData, RegistryValueKind.String);
            }

            // Construct the path
            string path = $@"\\{serverValueData}{importDirValueData}";

            // Check and create directory if it does not exist
            CheckAndCreateDir(path);

            // It's a good practice to close the keys after use.
            softwareKey.Close();
            currentUserKey.Close();
        }

        public static void CheckAndCreateDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
