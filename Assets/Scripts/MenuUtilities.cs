using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class MenuUtilities
{
    private static readonly float _standardMovingTime = 1f;

    #region Fading
    public static void FadeOut(RectTransform rectTransform, float time, Action onComplete)
    {
        LeanTween.alpha(rectTransform, 1, 0);
        ChangeAlpha(rectTransform,0,time,LeanTweenType.easeOutSine,() => { onComplete?.Invoke(); rectTransform.gameObject.SetActive(false); });
    }
    public static void FadeOut(RectTransform rectTransform, Action onComplete) => FadeOut(rectTransform, _standardMovingTime, onComplete);


    public static void FadeIn(RectTransform rectTransform, float time, Action onComplete)
    {
        LeanTween.alpha(rectTransform, 0, 1);
        rectTransform.gameObject.SetActive(true);
        ChangeAlpha(rectTransform, 1f, time, LeanTweenType.easeInSine, onComplete);
    }
    public static void FadeIn(RectTransform rectTransform,Action onComplete) => FadeIn(rectTransform, _standardMovingTime, onComplete);


    private static void ChangeAlpha(RectTransform rectTransform,float alpha, float time,LeanTweenType easingMode, Action onComplete)
    {
        LeanTween.alpha(rectTransform, alpha, time)
            .setEase(easingMode)
            .setOnComplete(onComplete)
            .setIgnoreTimeScale(true);
    }

    #endregion

    #region Popin

    public static void PopIn(RectTransform rectTransform, float time, Action onComplete)
    {
        rectTransform.localScale = Vector3.zero;
        rectTransform.gameObject.SetActive(true);
        ChangeScale(rectTransform, Vector3.one, time, LeanTweenType.easeOutBack, onComplete);
    }
    public static void PopIn(RectTransform rectTransform, Action onComplete) => PopIn(rectTransform, _standardMovingTime,onComplete);


    public static void PopOut(RectTransform rectTransform, float time, Action onComplete)
    {
        rectTransform.localScale = Vector3.one;
        ChangeScale(rectTransform, Vector3.zero, time, LeanTweenType.easeInBack, () => { onComplete?.Invoke(); rectTransform.gameObject.SetActive(false); });
    }
    public static void PopOut(RectTransform rectTransform, Action onComplete) => PopOut(rectTransform, _standardMovingTime, onComplete);


    private static void ChangeScale(RectTransform rectTransform, Vector3 scale,float time, LeanTweenType easingMode, Action onComplete)
    {
        LeanTween.scale(rectTransform, scale,time)
            .setEase(easingMode)
            .setOnComplete(onComplete)
            .setIgnoreTimeScale(true);
    }

    #endregion

    #region MoveIn

    public static void MoveIn(RectTransform rectTransform, float time, LeanTweenType easeType, Action onComplete)
    {
        Move(rectTransform, Vector3.zero, time, easeType, onComplete);
    }
    public static void MoveIn(RectTransform rectTransform, Action onComplete) => MoveIn(rectTransform, _standardMovingTime, LeanTweenType.easeOutSine,onComplete);


    public static void MoveOut(RectTransform rectTransform, Vector3 moveOutPosition, float time, LeanTweenType easeType, Action onComplete)
    {
        Move(rectTransform,moveOutPosition,time, easeType, onComplete);
    }

    public static void MoveOutLeft(RectTransform rectTransform, float time, LeanTweenType easeType, Action onComplete)
        => MoveOut(rectTransform, new Vector3(-2500,0,0), time, easeType, onComplete);
    public static void MoveOutLeft(RectTransform rectTransform, Action onComplete)
        => MoveOutLeft(rectTransform,_standardMovingTime,LeanTweenType.easeOutSine,onComplete);

    public static void MoveOutRight(RectTransform rectTransform, float time, LeanTweenType easeType, Action onComplete)
        => MoveOut(rectTransform, new Vector3(2500, 0, 0), time, easeType, onComplete);
    public static void MoveOutRight(RectTransform rectTransform, Action onComplete)
        => MoveOutRight(rectTransform, _standardMovingTime, LeanTweenType.easeOutSine, onComplete);

    public static void MoveOutUp(RectTransform rectTransform, float time, LeanTweenType easeType, Action onComplete)
        => MoveOut(rectTransform, new Vector3(0, 1800, 0), time, easeType, onComplete);
    public static void MoveOutUp(RectTransform rectTransform, Action onComplete)
        => MoveOutUp(rectTransform, _standardMovingTime, LeanTweenType.easeOutSine, onComplete);

    public static void MoveOutDown(RectTransform rectTransform, float time, LeanTweenType easeType, Action onComplete)
        => MoveOut(rectTransform, new Vector3(0, -1800, 0), time, easeType, onComplete);
    public static void MoveOutDown(RectTransform rectTransform, Action onComplete)
        => MoveOutDown(rectTransform, _standardMovingTime, LeanTweenType.easeOutSine, onComplete);

    private static void Move(RectTransform rectTransform, Vector3 toPosition, float time, LeanTweenType easeType, Action onComplete)
    {
        LeanTween.move(rectTransform, toPosition, time)
            .setEase(easeType)
            .setOnComplete(onComplete)
            .setIgnoreTimeScale(true);
    }

    #endregion
}
