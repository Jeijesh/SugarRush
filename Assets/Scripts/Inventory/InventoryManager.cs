using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class InventoryButton
    {
        public string fieldName;
        public Button button;
        public TMP_Dropdown targetDropdown;
        public GameObject popupPanel;
    }

    [Header("Inventory Buttons")]
    public InventoryButton[] inventoryButtons;

    private GameObject currentOpenPanel;

    private void Start()
    {
        CloseAll();

        foreach (var ib in inventoryButtons)
        {
            if (ib.button != null)
                ib.button.interactable = false;
        }

        if (PatientUI.Instance != null)
            PatientUI.Instance.OnEmptyDropdownChanged += UpdateButtons;
    }

    private void Update()
    {
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        if (PatientUI.Instance == null)
        {
            Debug.LogWarning("PatientUI.Instance belum siap");
            return;
        }

        Debug.Log($"--- UpdateButtons --- EmptyDropdowns count: {PatientUI.Instance.EmptyDropdowns.Count}");

        foreach (var ib in inventoryButtons)
        {
            if (ib.button == null || ib.targetDropdown == null)
            {
                Debug.LogWarning($"Button atau targetDropdown null: {ib.fieldName}");
                continue;
            }

            bool isEmpty = PatientUI.Instance.EmptyDropdowns.Contains(ib.targetDropdown);
            ib.button.interactable = isEmpty;

            Debug.Log($"Button {ib.fieldName} | Dropdown: {ib.targetDropdown.name} | IsEmpty: {isEmpty} | Interactable: {ib.button.interactable}");
        }
    }

    public void OpenItem(int index)
    {
        CloseAll();

        if (index >= 0 && index < inventoryButtons.Length)
        {
            var ib = inventoryButtons[index];
            if (ib.popupPanel != null)
            {
                ib.popupPanel.SetActive(true);
                currentOpenPanel = ib.popupPanel;
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
        foreach (var ib in inventoryButtons)
            if (ib.popupPanel != null)
                ib.popupPanel.SetActive(false);

        currentOpenPanel = null;
    }
}
