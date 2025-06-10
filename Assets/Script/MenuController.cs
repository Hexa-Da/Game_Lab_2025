using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum PannelType
{
    None,
    Main,
    Play,
    Parametres,
    Credits,
}

public class MenuController : MonoBehaviour
{
    [Header("Pannels")]
    [SerializeField] private List<MenuPannel> pannelsList = new List<MenuPannel>();
    private Dictionary<PannelType, MenuPannel> pannelsDict = new Dictionary<PannelType, MenuPannel>();

    [SerializeField] private EventSystem eventController;

    private GameManager manager;

    private MenuInputs inputs;

    private void Start()
    {
        manager = GameManager.instance; 
        inputs = GetComponent<MenuInputs>();

        foreach(var _pannel in pannelsList)
        {
            if (_pannel) 
            { 
                pannelsDict.Add(_pannel.GetPannelType(), _pannel);
                _pannel.Init(this);
            }
        }
        OpenOnePannel(PannelType.Main, false);
    }

    private void OpenOnePannel(PannelType _type, bool  _animate)
    {
        foreach(var _pannel in pannelsList)
        {
            _pannel.ChangeState(false, false);
        }

        if(_type != PannelType.None && pannelsDict.ContainsKey(_type))
        {
            pannelsDict[_type].ChangeState(_animate, true);
        }
    }

    public void OpenPannel(PannelType _type)
    {
        OpenOnePannel(_type, true);
    }
    
    public void ChangeScene(string _sceneName)
    {
       manager.ChangeScene(_sceneName);
    }

    public void Quit()
    {
       manager.Quit();
    }

    public void SetSelectedGameObject(GameObject _element, Button _rightPannel, Button _leftPannel)
    {
        eventController.SetSelectedGameObject(_element);

        if(_rightPannel != null)
        {
            inputs.SetShoulderListener(MenuInputs.Side.Right, _rightPannel.onClick.Invoke, _rightPannel.Select);
        }

        if(_leftPannel != null)
        {
            inputs.SetShoulderListener(MenuInputs.Side.Left, _leftPannel.onClick.Invoke, _leftPannel.Select);
        }
    }

    public void StartNewGame()
    {
        // Réinitialiser les compteurs
        Interaction.SetStickCount(0);
        Interaction.SetFragmentCount(0);
        
        // Charger la scène de jeu
        ChangeScene("Scene"); // Remplacez "Game" par le nom de votre scène de jeu
    }

    public void LoadSaveGame()
    {
        if (SaveSystem.Instance.HasSaveFile())
        {
            bool success = SaveSystem.Instance.LoadGame();
            if (!success)
            {
                Debug.LogError("Failed to load save game. Starting new game instead.");
                StartNewGame();
            }
        }
        else
        {
            Debug.LogWarning("No save file found. Starting new game instead.");
            StartNewGame();
        }
    }
}
