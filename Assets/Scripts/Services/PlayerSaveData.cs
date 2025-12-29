using System;
using System.Collections.Generic;

namespace OnePatientLeft.Services
{
    [Serializable]
    public class PlayerSaveData
    {
        public string levelSceneName;
        public Dictionary<string, int> inventory; // itemId -> count
    }
}
