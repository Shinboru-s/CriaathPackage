using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public static class Helper
{

    public static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    /// <summary>
    /// Returns a WaitForSeconds instance for the specified duration. If an instance for
    /// this duration already exists, it reuses the existing instance; otherwise, it creates
    /// a new instance and stores it.
    /// </summary>
    /// <param name="time">The duration to wait (in seconds).</param>
    /// <returns>A WaitForSeconds instance for the specified duration.</returns>
    public static WaitForSeconds GetWait(float time)
    {
        if (WaitDictionary.TryGetValue(time, out var wait)) return wait;

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }

    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;
    /// <summary>
    /// Determines whether the mouse pointer is currently over a UI element.
    /// </summary>
    /// <returns>True if the mouse pointer is over a UI element; otherwise, false.</returns>
    public static bool IsPointerOverUi()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }

    /// <summary>
    /// Converts the screen position of a UI element to its world position.
    /// </summary>
    /// <param name="element">The RectTransform of the UI element.</param>
    /// <returns>The world position of the UI element as a Vector2.</returns>
    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera.main, out var result);
        return result;
    }
}
