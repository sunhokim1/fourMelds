using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class HorizontalWheelScroll : MonoBehaviour, IScrollHandler
{
    [SerializeField] private ScrollRect target;
    [SerializeField] private float wheelSpeed = 0.08f;

    public void Bind(ScrollRect scrollRect)
    {
        target = scrollRect;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (target == null)
            return;

        float delta = eventData.scrollDelta.y;
        if (Mathf.Approximately(delta, 0f))
            delta = -eventData.scrollDelta.x;

        if (Mathf.Approximately(delta, 0f))
            return;

        if (target.vertical)
        {
            target.verticalNormalizedPosition = Mathf.Clamp01(
                target.verticalNormalizedPosition + (delta * wheelSpeed)
            );
            return;
        }

        if (target.horizontal)
        {
            target.horizontalNormalizedPosition = Mathf.Clamp01(
                target.horizontalNormalizedPosition - (delta * wheelSpeed)
            );
        }
    }
}
