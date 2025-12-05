using System;
using UnityEngine;

namespace _Scripts.Lecture11
{
    public class PlayerSave : MonoBehaviour
    {
        public static PlayerSave Instance;

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Save(ref PlayerData data)
        {
            data.position = transform.position;
        }

        public void Load(PlayerData data)
        {
            transform.position = data.position;
            Physics.SyncTransforms(); // Force the Character Controller into changing the transform value
        }

        [Serializable]
        public struct PlayerData
        {
            public Vector3 position;
        }
    }
}
