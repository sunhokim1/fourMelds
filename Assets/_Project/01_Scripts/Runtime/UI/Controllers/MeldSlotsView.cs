using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Project.Core.Melds;

public class MeldSlotsView : MonoBehaviour
{
    [SerializeField] private Transform slotsRoot;   // MeldSlotsPanel
    [SerializeField] private GameObject slotPrefab; // MeldSlot prefab

    private readonly List<GameObject> _slots = new();

    public void Render(IReadOnlyList<MeldState> melds)
    {
        EnsureSlots(4);

        // 0~3 ½½·Ô¿¡ melds¸¦ ¸ÅÇÎ, ¾øÀ¸¸é ºó ½½·ÔÀ¸·Î
        for (int i = 0; i < 4; i++)
        {
            var slotGo = _slots[i];
            var slot = slotGo.GetComponent<MeldSlotView>();

            if (i < melds.Count)
                slot.Bind(melds[i]);
            else
                slot.Clear();
        }
    }

    private void EnsureSlots(int count)
    {
        while (_slots.Count < count)
        {
            var go = Instantiate(slotPrefab, slotsRoot);
            _slots.Add(go);
        }
    }
}