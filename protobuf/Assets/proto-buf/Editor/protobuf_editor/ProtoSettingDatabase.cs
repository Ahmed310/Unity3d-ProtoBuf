

using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace BnBCustomEditor.Utils.Proto
{
    public class ProtoSettingDatabase : ScriptableObject
    {
        [SerializeField]
        private SettingData database;

        void OnEnable()
        {
            if (database == null) database = new SettingData();
        }

        public SettingData GetDatabase()
        {
            return database;
        }
    }


    [System.Serializable]
    public class SettingData
    {
        public string ToolPath;
        public string OutputPath;
        public string ProtoFilesPath;
        public string Command;
        public List<string> ProtoFiles;

    }

}
