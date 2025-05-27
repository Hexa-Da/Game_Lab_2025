using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public Transform inventoryGrid;
    public GameObject inventorySlotPrefab;
    public int slotCount = 10;

    private List<Image> slotImages = new List<Image>();
    public List<Sprite> collectedItemSprites = new List<Sprite>();

    void Start()
    {
        InitializeSlots();
    }

    void InitializeSlots()
    {
        foreach (Transform child in inventoryGrid)
            Destroy(child.gameObject);

        slotImages.Clear();
        collectedItemSprites.Clear();

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryGrid);
            Image img = slot.GetComponent<Image>();
            slotImages.Add(img);
            collectedItemSprites.Add(null);
            img.sprite = null;
            img.color = new Color(1,1,1,0.5f); // slot vide visible
        }
    }

    public void AddItem(Sprite itemSprite)
    {
        for (int i = 0; i < collectedItemSprites.Count; i++)
        {
            if (collectedItemSprites[i] == null)
            {
                collectedItemSprites[i] = itemSprite;
                slotImages[i].sprite = itemSprite;
                slotImages[i].color = Color.white;
                break;
            }
        }
    }
}