using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   public static GameManager instance {private set; get;}

   [Header("Version Info")]
   public string gameVersion = "1.0.0"; // Version du jeu
   public string buildDate; // Date du build

   private void Awake()
   {
     if(instance != null){
        Destroy(this.gameObject);
        return;
     }  
    
    instance = this;
    DontDestroyOnLoad(this.gameObject);
    buildDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
   }

   public void ChangeScene(string _sceneName)
   {
        SceneManager.LoadScene(_sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;
   }

   private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject obj in interactables)
        {
            obj.SetActive(true);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
   }

    public void Quit()
    {
        Application.Quit();
    }

    // Méthode pour afficher la version
    public string GetVersionInfo()
    {
        return $"Version: {gameVersion}\nBuild Date: {buildDate}";
    }
}
