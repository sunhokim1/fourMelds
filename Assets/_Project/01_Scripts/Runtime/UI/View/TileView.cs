// Assets/_Project/01_Scripts/Runtime/UI/View/TileView.cs

using UnityEngine;
using UnityEngine.EventSystems;
using Project.InputSystem;
using Project.UI;
using System;

public class TileView : MonoBehaviour, IIdentifiable, IPointerClickHandler
{
    public static event Action<TileView, PointerEventData.InputButton> OnTileClicked;

    [SerializeField] private int id;
    public int Id => id;

    [SerializeField] private MouseActionRequestSource inputSource;
    [SerializeField] private TileVisual visual;
    private void Awake()
    {
        if (visual == null)
            visual = GetComponentInChildren<TileVisual>(true);

        if (inputSource == null)
            inputSource = FindFirstObjectByType<MouseActionRequestSource>();
    }

    public void Bind(int tileId)
    {
        id = tileId;
        if (visual != null) visual.SetTile(tileId);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnTileClicked?.Invoke(this, eventData.button);

        if (inputSource == null)
        {
            Debug.LogError("[TileView] MouseActionRequestSource not found");
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            inputSource.Raise(new ActionRequest(
                ActionRequestType.SelectTile,
                ActionTargetType.Tile,
                Id
            ));
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            inputSource.Raise(new ActionRequest(
                ActionRequestType.OpenActionMenu,
                ActionTargetType.Tile,
                Id
            ));
        }
    }
}
