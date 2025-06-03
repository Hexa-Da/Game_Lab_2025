using UnityEngine;
using UnityEngine.Events;
 
public class OpenPannelButton : MonoBehaviour
{
   [SerializeField] private PannelType type;
   [SerializeField] private OpenPannelButton onSwitchBackAction;

   private MenuController controller;
   private MenuInputs inputs;

   void Start()
   {
      controller = FindObjectOfType<MenuController>();
      inputs = controller.GetComponent<MenuInputs>();
   }

   public void OnClick()
   {
      controller.OpenPannel(type);
      if(onSwitchBackAction != null)
      {
         inputs.SetBackListener(onSwitchBackAction.OnClick);
      }
      else
      {
         inputs.SetBackListener();
      }
   }  
} 
