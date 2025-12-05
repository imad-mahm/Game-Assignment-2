using UnityEngine;

public class PlayerSaveHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private int collectibleCount;

    private void Start()
    {
        // OPTIONAL: auto-load on start
        // LoadPlayer();
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

        playerStats.currentHealth = Mathf.Clamp(data.playerHealth, 0f, 100);

        playerStats.Heal(0);

        // Load position
        transform.position = new Vector3(
            data.playerPosition[0],
            data.playerPosition[1],
            data.playerPosition[2]
        );

        // Load rotation
        transform.eulerAngles = new Vector3(
            data.playerRotation[0],
            data.playerRotation[1],
            data.playerRotation[2]
        );

        // Load collectibles
        collectibleCount = data.collectibleCount;
    }



    public void SavePlayer()
    {
        SaveData data = CreateSaveData();
        SaveManager.SaveGame(data);
        
        NotificationManager.Instance.ShowMessage("Game Saved!", Color.green);
    }

    public void LoadPlayer()
    {
        SaveData data = SaveManager.LoadGame();

        if (data == null)
        {
            NotificationManager.Instance.ShowMessage("No Save Found!", Color.red);
            return;
        }

        ApplyLoadedData(data);

        // Show notification
        NotificationManager.Instance.ShowMessage("Game Loaded!", Color.green);

        // Start game automatically
        MenuManager menu = FindObjectOfType<MenuManager>();
        if (menu != null)
        {
            menu.StartGame(); 
        }

        // Ensure unpaused
        PauseManager pause = FindObjectOfType<PauseManager>();
        pause?.ResumeGame();
    }

}