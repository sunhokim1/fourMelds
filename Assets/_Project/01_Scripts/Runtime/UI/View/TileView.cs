using UnityEngine;
using UnityEngine.EventSystems;
using Project.InputSystem;
using Project.UI;

public class TileView : MonoBehaviour, IIdentifiable, IPointerClickHandler
{
    [SerializeField] private int id;
    public int Id => id;

    [SerializeField] private MouseActionRequestSource inputSource;

    private void Awake()
    {
        // 인스펙터에서 안 꽂았으면 자동 탐색(테스트용)
        if (inputSource == null)
            inputSource = FindFirstObjectByType<MouseActionRequestSource>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (inputSource == null)
            {
                Debug.LogError("[TileView] MouseActionRequestSource not found");
                return;
            }

            inputSource.Raise(new ActionRequest(
                ActionRequestType.OpenActionMenu,
                ActionTargetType.Tile,
                Id
            ));
        }
    }
}