using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Inventory_System02.Includes
{
    internal class RegistryManager
    {
        public static void VerifyRegistryEntry()
        {
            string registryPath = @"SOFTWARE\codefilterPH\InventoryMS";
            string valueName = "ImportsDir";
            string valueData = @"\CommonSql\Imports";

            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey softwareKey = currentUserKey.OpenSubKey(registryPath, true);

            // If the SOFTWARE\codefilterPH\InventoryMS key doesn't exist then create it.
            if (softwareKey == null)
            {
                softwareKey = currentUserKey.CreateSubKey(registryPath);
            }

            // If the ImportsDir value doesn't exist or it exists but has different data then create/update it.
            if (softwareKey.GetValue(valueName) == null || softwareKey.GetValue(valueName).ToString() != valueData)
            {
                softwareKey.SetValue(valueName, valueData, RegistryValueKind.String);
            }

            // It's a good practice to close the keys after use.
            softwareKey.Close();
            currentUserKey.Close();
        }
    }
}
