using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public static class Utils
{
    /// <summary>
    /// kiểm tra xem chuột có đang nằm trên UI hay không?
    /// UI có tick chọn RaycastTarget
    /// </summary>
    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;

    public static bool IsMouseOverUI()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        _eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }


    /// <summary>
    /// lấy vị trí chuột theo đơn vị World
    /// </summary>
    private static Camera _mainCam;

    public static Vector3 GetMouseWorldPosition()
    {
        if (_mainCam == null)
            _mainCam = Camera.main;

        var mousePos = Input.mousePosition;
        var mouseWorldPos = _mainCam!.ScreenToWorldPoint(mousePos); // z = -10
        return new Vector3(mouseWorldPos.x, mouseWorldPos.y);
    }


    public static Vector3 WorldToScreenPoint(Vector3 worldPos)
    {
        if (_mainCam == null)
            _mainCam = Camera.main;

        return _mainCam!.WorldToScreenPoint(worldPos);
    }

    public static int Rand(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    #region ===> DELAY & WAIT

    public static async void DelayAction(int millisecond, Action action)
    {
        await UniTask.Delay(millisecond);
        action?.Invoke();
    }

    public static async void DelayFrameAction(int frame, Action action)
    {
        await UniTask.DelayFrame(frame);
        action?.Invoke();
    }

    public static UniTask WaitAction(int millisecond, Action action)
    {
        action?.Invoke();
        return UniTask.Delay(millisecond);
    }

    public static UniTask WaitFrameAction(int frame, Action action)
    {
        action?.Invoke();
        return UniTask.DelayFrame(frame);
    }

    #endregion
}