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
    [SerializeField] private Color specialBorderColor = new Color(0.96f, 0.78f, 0.22f, 0.98f);
    [SerializeField] private Vector2 specialBorderDistance = new Vector2(2f, -2f);
    private Outline _specialOutline;
    private void Awake()
    {
        if (visual == null)
            visual = GetComponentInChildren<TileVisual>(true);

        if (inputSource == null)
            inputSource = FindFirstObjectByType<MouseActionRequestSource>();

        _specialOutline = GetComponent<Outline>();
        if (_specialOutline == null)
            _specialOutline = gameObject.AddComponent<Outline>();
        _specialOutline.enabled = false;
        _specialOutline.effectColor = specialBorderColor;
        _specialOutline.effectDistance = specialBorderDistance;
    }

    public void Bind(int tileId)
    {
        id = tileId;
        if (visual != null) visual.SetTile(tileId);
        SetSpecialHighlight(false);
    }

    public void SetSpecialHighlight(bool enabled)
    {
        if (_specialOutline == null)
            return;
        _specialOutline.enabled = enabled;
        if (enabled)
        {
            _specialOutline.effectColor = specialBorderColor;
            _specialOutline.effectDistance = specialBorderDistance;
        }
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
