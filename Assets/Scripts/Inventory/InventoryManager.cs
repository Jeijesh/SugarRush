using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class InventoryItem
    {
        public string name;
        public GameObject popupPanel; // minigame panel yg akan muncul
    }

    [Header("Inventory Items")]
    public InventoryItem[] items;

    private GameObject currentOpenPanel;

    private void Start()
    {
        CloseAll();
    }

    private void Update()
    {
        // cek tombol Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseCurrent();
        }
    }

    // panggil dari Button OnClick
    public void OpenItem(int index)
    {
        CloseAll();

        if (index >= 0 && index < items.Length)
        {
            if (items[index].popupPanel != null)
            {
                items[index].popupPanel.SetActive(true);
                currentOpenPanel = items[index].popupPanel;
            }
        }
    }

    private void CloseCurrent()
    {
        if (currentOpenPanel != null)
        {
            currentOpenPanel.SetActive(false);
            currentOpenPanel = null;
        }
    }

    public void CloseAll()
    {
        foreach (var item in items)
        {
            if (item.popupPanel != null)
                item.popupPanel.SetActive(false);
        }
        currentOpenPanel = null;
    }
}
