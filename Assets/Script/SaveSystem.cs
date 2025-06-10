using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    private static SaveSystem instance;
    public static SaveSystem Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("SaveSystem");
                instance = go.AddComponent<SaveSystem>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.dat");

    [System.Serializable]
    public class SaveData
    {
        public int stickCount;
        public int fragmentCount;
        public string currentScene;
        public List<string> collectedObjects = new List<string>(); // Liste des objets collectés
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        
        // Récupérer les données actuelles du jeu
        if (MenuInGame.instance != null)
        {
            // Récupérer les compteurs depuis Interaction
            data.stickCount = Interaction.GetStickCount();
            data.fragmentCount = Interaction.GetFragmentCount();
            Debug.Log($"Saving game state - Sticks: {data.stickCount}, Fragments: {data.fragmentCount}");
        }
        
        // Sauvegarder la liste des objets collectés
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject obj in collectibles)
        {
            Interaction interaction = obj.GetComponent<Interaction>();
            if (interaction != null && (interaction.isStick || interaction.isFragment))
            {
                // Si l'objet n'est pas actif, c'est qu'il a été collecté
                if (!obj.activeSelf)
                {
                    data.collectedObjects.Add(obj.name);
                }
            }
        }
        
        data.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"Saving scene: {data.currentScene}");

        // Sauvegarder les données
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(SavePath, FileMode.Create))
        {
            formatter.Serialize(stream, data);
        }
        
        Debug.Log($"Save file created at: {SavePath}");
    }

    public bool LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogError($"No save file found at: {SavePath}");
            return false;
        }

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(SavePath, FileMode.Open))
            {
                SaveData data = (SaveData)formatter.Deserialize(stream);
                Debug.Log($"Loading save file - Sticks: {data.stickCount}, Fragments: {data.fragmentCount}, Scene: {data.currentScene}");
                
                // Charger les données
                if (MenuInGame.instance != null)
                {
                    // Mettre à jour les compteurs
                    Interaction.SetStickCount(data.stickCount);
                    Interaction.SetFragmentCount(data.fragmentCount);
                    MenuInGame.instance.UpdateUI(data.stickCount, data.fragmentCount);
                }

                // Désactiver les objets déjà collectés
                GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Interactable");
                foreach (GameObject obj in collectibles)
                {
                    if (data.collectedObjects.Contains(obj.name))
                    {
                        obj.SetActive(false);
                    }
                }

                // Charger la scène sauvegardée
                GameManager.instance.ChangeScene(data.currentScene);
                
                return true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading save file: {e.Message}");
            return false;
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }
} 