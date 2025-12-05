using UnityEngine;

public class PlayerSaveHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private int collectibleCount;

    private void Start()
    {
        // Check if we must load saved data when the scene starts
        LoadFromMenu();
    }

    // Called only when loading from main menu
    private void LoadFromMenu()
    {
        if (PlayerPrefs.GetInt("LoadGameFlag", 0) == 1)
        {
            PlayerPrefs.SetInt("LoadGameFlag", 0);

            SaveData data = SaveManager.LoadGame();
            if (data != null)
            {
                ApplyLoadedData(data);
                NotificationManager.Instance.ShowMessage("Game Loaded!", Color.green);
            }
            else
            {
                NotificationManager.Instance.ShowMessage("No Save Found!", Color.red);
            }
        }
    }

    public SaveData CreateSaveData()
    {
        SaveData data = new SaveData();

        // Save health
        data.playerHealth = playerStats.currentHealth;

        // Save position
        Vector3 pos = transform.position;
        data.playerPosition[0] = pos.x;
        data.playerPosition[1] = pos.y;
        data.playerPosition[2] = pos.z;

        // Save rotation
        Vector3 rot = transform.eulerAngles;
        data.playerRotation[0] = rot.x;
        data.playerRotation[1] = rot.y;
        data.playerRotation[2] = rot.z;

        // Save collectibles
        data.collectibleCount = collectibleCount;

        return data;
    }

    public void ApplyLoadedData(SaveData data)
    {
        if (data == null) return;

        // Restore health
        playerStats.currentHealth = Mathf.Clamp(data.playerHealth, 0f, playerStats.maxHealth);
        playerStats.Heal(0);

        // Saved position & rotation
        Vector3 savedPos = new Vector3(
            data.playerPosition[0],
            data.playerPosition[1],
            data.playerPosition[2]
        );

        Vector3 savedRot = new Vector3(
            data.playerRotation[0],
            data.playerRotation[1],
            data.playerRotation[2]
        );

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            transform.position = savedPos;    // apply position
            transform.eulerAngles = savedRot; // apply rotation
            cc.enabled = true;
        }
        else
        {
            transform.position = savedPos;
            transform.eulerAngles = savedRot;
        }

        // Restore collectibles
        collectibleCount = data.collectibleCount;
    }


    public void SavePlayer()
    {
        SaveData data = CreateSaveData();
        SaveManager.SaveGame(data);
        NotificationManager.Instance.ShowMessage("Game Saved!", Color.green);
    }
}
