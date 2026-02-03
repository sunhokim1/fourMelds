using UnityEngine;
using UnityEngine.UI;
using Project.Core.Melds;

public class MeldSlotView : MonoBehaviour
{
    [SerializeField] private Text infoText;
    [SerializeField] private Text[] tileTexts; // TilePreview 4개를 여기에 연결

    public void Bind(MeldState meld)
    {
        infoText.text = $"{meld.Type}  id={meld.MeldId}" + (meld.IsFixed ? " [FIX]" : "");

        for (int i = 0; i < tileTexts.Length; i++)
        {
            if (i < meld.Tiles.Length)
            {
                tileTexts[i].gameObject.SetActive(true);
                tileTexts[i].text = (meld.Tiles[i] % 100).ToString();
            }
            else
            {
                tileTexts[i].gameObject.SetActive(false);
            }
        }
    }

    public void Clear()
    {
        infoText.text = "(empty)";
        for (int i = 0; i < tileTexts.Length; i++)
            tileTexts[i].gameObject.SetActive(false);
    }
}