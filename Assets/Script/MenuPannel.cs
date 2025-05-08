using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup))]

public class MenuPannel : MonoBehaviour
{
    [SerializeField] private PannelType type;

    [Header("Animation")]
    [SerializeField] private float animationTime;
    [SerializeField] private AnimationCurve animationCurve = new AnimationCurve();

    [Header("Config")]
    [SerializeField] private GameObject selectedGameObject;
    [SerializeField] private Button rightPannel, leftPannel;
    private MenuController controller;

    private bool state;
    private Canvas canvas;
    private CanvasGroup group;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        canvas = GetComponent<Canvas>();
    }
        
    private void UpdateState(bool  _animate)
    {
        StopAllCoroutines();

        if(_animate) StartCoroutine(Animate(state));
        else
        {
            canvas.enabled = state;
            group.interactable = state;  
            group.blocksRaycasts = state;
        } 

        if(state) controller.SetSelectedGameObject(selectedGameObject, rightPannel, leftPannel);
    }

    public void Init(MenuController _controller)
    {
        controller = _controller;
    }

    private IEnumerator Animate(bool _state) 
    {
        canvas.enabled = true;

        float _t =_state ? 0 : 1;
        float _target = _state ? 1 : 0;
        int _factor = _state ? 1 : -1;

        while(true)
        {
            yield return null;

            _t += Time.deltaTime * _factor / animationTime;

            group.alpha = animationCurve.Evaluate(_t);

            if((state && _t >=_target) || (!state && _t <= _target))
            {
                group.alpha = _target;
                break;
            } 
        }
        group.interactable = _state;  
        group.blocksRaycasts = _state;
        canvas.enabled = _state;
    }

    public void ChangeState(bool  _animate)
    {
        state = !state;
        UpdateState(_animate);
    }

    public void ChangeState(bool  _animate, bool _state)
    {
        state = _state;
        UpdateState(_animate);
    }

    #region Getter

    public PannelType GetPannelType() 
    {
        return type;
    }

    #endregion
}
