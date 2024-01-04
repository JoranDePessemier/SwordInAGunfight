using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{

    [SerializeField]
    private float _scaleSize = 1.2f;

    [SerializeField]
    private float _scaleTime = 0.5f;

    [SerializeField]
    private UnityEvent _select;

    [SerializeField]
    private UnityEvent _clickedScene;

    [SerializeField]
    private UnityEvent _clickedPrefab;

    [SerializeField]
    private MenuState _selectableState;



    public void OnDeselect(BaseEventData eventData)
    {
        
    }


    public void OnSelect(BaseEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
            _clickedScene.Invoke();
            _clickedPrefab.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, Vector3.one * _scaleSize, _scaleTime).setEase(LeanTweenType.easeInBack).setIgnoreTimeScale(true);
        _select.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.scale(gameObject, Vector3.one, _scaleTime).setEase(LeanTweenType.easeOutBack).setIgnoreTimeScale(true);
    }
}
