using System;
using System.Collections.Generic;

namespace OnePatientLeft.Services
{
    /// <summary>
    /// Serializable container for a player's save data (example / alternative save model).
    /// </summary>
    /// <remarks>
    /// This class is not currently used by the UGS Cloud Save implementation shown in
    /// <see cref="Services.CloudSaveStore"/> (which stores stage + inventory_json).
    /// If you decide to store a richer save payload (e.g., level name + inventory dictionary)
    /// you can serialize this class to JSON and store it under a single Cloud Save key.
    /// </remarks>
    [Serializable]
    public class PlayerSaveData
    {
        /// <summary>Scene name of the saved level.</summary>
        public string levelSceneName;

        /// <summary>Inventory mapping: itemId -> count.</summary>
        public Dictionary<string, int> inventory;
    }
}
