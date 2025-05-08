using UnityEngine;

public class ToDelete : MonoBehaviour
{
    // Singleton instance
    public static ToDelete Instance;

    // Public fields you can edit in Inspector
    public Sprite S1;
    public Sprite S2;
    public GameObject Horse;
    public TextAsset Data;

    void Awake()
    {
        // Set the singleton instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
}