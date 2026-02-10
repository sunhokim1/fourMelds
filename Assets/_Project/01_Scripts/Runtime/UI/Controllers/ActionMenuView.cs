using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Project.UI.Models;

public class ActionMenuView : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private Button buttonPrefab;

    public event Action<ActionOption> OnOptionSelected;

    private readonly List<Button> _activeButtons = new();
    private readonly List<Button> _buttonPool = new();

    public void Show(ActionMenuModel model)
    {
        ReleaseAllActiveButtons();

        if (panelRoot == null)
        {
            Debug.LogError("[ActionMenuView] panelRoot is null (Inspector?)");
            return;
        }
        if (contentRoot == null)
        {
            Debug.LogError("[ActionMenuView] contentRoot is null (Inspector?)");
            return;
        }
        if (buttonPrefab == null)
        {
            Debug.LogError("[ActionMenuView] buttonPrefab is null (Inspector?)");
            return;
        }
        if (model == null || model.Options == null)
        {
            Debug.LogError("[ActionMenuView] model or model.Options is null");
            return;
        }

        panelRoot.SetActive(true);

        foreach (var option in model.Options)
        {
            var btn = AcquireButton();
            if (btn == null)
                continue;

            var tiles = option.PreviewTiles ?? Array.Empty<int>();
            var previews = btn.GetComponentsInChildren<Text>(true);

            foreach (var p in previews)
                p.gameObject.SetActive(false);

            for (int i = 0; i < tiles.Length && i < previews.Length; i++)
            {
                previews[i].gameObject.SetActive(true);
                previews[i].text = (tiles[i] % 100).ToString();
            }

            btn.onClick.AddListener(() =>
            {
                OnOptionSelected?.Invoke(option);
                Hide();
            });
        }
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        ReleaseAllActiveButtons();
    }

    private Button AcquireButton()
    {
        Button btn;
        var last = _buttonPool.Count - 1;
        if (last >= 0)
        {
            btn = _buttonPool[last];
            _buttonPool.RemoveAt(last);
            if (btn != null)
            {
                btn.transform.SetParent(contentRoot, false);
                btn.gameObject.SetActive(true);
            }
        }
        else
        {
            btn = Instantiate(buttonPrefab, contentRoot);
        }

        if (btn == null)
            return null;

        btn.onClick.RemoveAllListeners();
        _activeButtons.Add(btn);
        return btn;
    }

    private void ReleaseAllActiveButtons()
    {
        for (int i = 0; i < _activeButtons.Count; i++)
        {
            var btn = _activeButtons[i];
            if (btn == null)
                continue;

            btn.onClick.RemoveAllListeners();
            btn.gameObject.SetActive(false);
            _buttonPool.Add(btn);
        }

        _activeButtons.Clear();
    }
}
