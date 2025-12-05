using System;
using UnityEngine;
using System.IO;

namespace _Scripts.Lecture11
{
    public class SaveSystem : MonoBehaviour
    {
        #region Save/Load
        private static SavedData savedData = new SavedData();

        [System.Serializable]
        public struct SavedData
        {
            public PlayerSave.PlayerData PlayerSaveData;
        }

        public static string SaveFileName()
        {
            var saveFile = Application.persistentDataPath + "/save" + ".sav";
            return saveFile;
        }

        public static void Save()
        {
            HandleSaveData();

            // Without Encryption version
            File.WriteAllText(SaveFileName(), JsonUtility.ToJson(savedData, true));
            
        }

        private static void HandleSaveData()
        {
            PlayerSave.Instance.Save(ref savedData.PlayerSaveData);
        }

        public static void Load()
        {
            var saveContent = File.ReadAllText(SaveFileName());

            // Without Encryption version
            savedData = JsonUtility.FromJson<SavedData>(saveContent);
            
            // With Encryption version
            // var decryptedContent = EncryptionUtility.DecryptString(saveContent);
            // savedData = JsonUtility.FromJson<SavedData>(decryptedContent);
            
            HandleLoadData();
        }

        private static void HandleLoadData()
        {
            PlayerSave.Instance.Load(savedData.PlayerSaveData);
        }
        
        #endregion

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Save();
            } else if (Input.GetKeyDown(KeyCode.P))
            {
                Load();
            }
        }
    }
}