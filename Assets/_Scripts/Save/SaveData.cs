using UnityEngine;

[System.Serializable]
public class SaveData
{
    public float playerHealth;
    public float[] playerPosition = new float[3];
    public float[] playerRotation = new float[3];

    public int collectibleCount;
}