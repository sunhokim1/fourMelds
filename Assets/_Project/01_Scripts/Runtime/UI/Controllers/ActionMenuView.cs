using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Project.UI.Models;
using System;



public class ActionMenuView : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private Button buttonPrefab;
    public event Action<ActionOption> OnOptionSelected;

    private readonly List<Button> _spawned = new();

    public void Show(ActionMenuModel model)
    {
        Clear();
        panelRoot.SetActive(true);

        foreach (var option in model.Options)
        {
            // 1️⃣ 버튼 생성
            var btn = Instantiate(buttonPrefab, contentRoot);
            _spawned.Add(btn);

            // 2️⃣ 버튼 안에 있는 TilePreview(Text)들 전부 가져오기
            // (PreviewRoot 아래에 있는 Text들)
            var previews = btn.GetComponentsInChildren<Text>(true);

            // 3️⃣ 전부 끄기
            foreach (var p in previews)
                p.gameObject.SetActive(false);

            // 4️⃣ PreviewTiles 개수만큼 켜고 값 채우기
            for (int i = 0; i < option.PreviewTiles.Length && i < previews.Length; i++)
            {
                previews[i].gameObject.SetActive(true);

                // 101 → 1 처럼 숫자만 표시
                previews[i].text = (option.PreviewTiles[i] % 100).ToString();
            }

            // 5️⃣ 버튼 클릭 처리
            btn.onClick.AddListener(() =>
            {
                OnOptionSelected?.Invoke(option);
                Hide();
            });
        }
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
        Clear();
    }

    private void Clear()
    {
        foreach (var b in _spawned)
            if (b != null) Destroy(b.gameObject);

        _spawned.Clear();
    }
}