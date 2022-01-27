using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IUpdateMeWithInput
{
    void UpdateUI(PlayerInput input);
}

public class PlayerItemUI : UIBehaviour
{
    [SerializeField] private List<ItemSlot> itemSlots = new List<ItemSlot>();
    [SerializeField] private int selectedItemSlotIndex = 0;
    private Image currentSelectionImage;

    [SerializeField] RectTransform parent;
    [SerializeField] Text itemNameText;

    [SerializeField] RectTransform itemSlotPrefab;
    [SerializeField] Image itemSlotModePrefab;


    private void Start()
    {
        UIHandler.Instance.Show(this);
        PlayerItemHolder.OnAddNewItem += OnAddItem;
        PlayerItemHolder.OnRemoveItem += OnRemoveItem;
        PlayerItemUser.OnStartUsingItem += ShowSelectionIndicator;
        PlayerItemUser.OnEndUsingItem += HideSelectionIndicator;
    }

    private void OnAddItem(PlayerItem item, bool forceSelection)
    {
        ItemSlot newSlot = new ItemSlot() { Item = item, Modes = item.GetItemModes() };
        itemSlots.Add(newSlot);
        CreateInstancesForSlot(newSlot);

        if (forceSelection)
            selectedItemSlotIndex = itemSlots.IndexOf(newSlot);

        UpdateSelectedItem();
    }
    private void OnRemoveItem(PlayerItem item)
    {
        for (int i = itemSlots.Count -1; i >= 0; i--)
        {
            ItemSlot slot = itemSlots[i];

            if (slot.Item == item)
            {
                Destroy(slot.RectInstance.gameObject);
                itemSlots.RemoveAt(i);
            }
        }

        UpdateSelectedItem();
    }
    private void SetSelectionIndicatorAlpha(float alpha)
    {
        Debug.Log(alpha);

        if (currentSelectionImage != null)
            currentSelectionImage.color = new Color(255, 255, 255, alpha);
    }

    private void CreateUIElementsForAllItems()
    {
        foreach (ItemSlot slot in itemSlots)
        {
            CreateInstancesForSlot(slot);
        }
    }

    private void CreateInstancesForSlot(ItemSlot slot)
    {
        slot.RectInstance = Instantiate(itemSlotPrefab, parent);

        foreach (ItemMode mode in slot.Modes)
        {
            Image selectedImage = Instantiate(itemSlotModePrefab, slot.RectInstance);
            Image iconImage = selectedImage.GetComponentInChildrenExcludeOwn<Image>();
            mode.IconImage = iconImage;
            mode.SelectedImage = selectedImage;
            iconImage.sprite = mode.Icon;
        }
    }

    private void UpdateSelectedItem()
    {
        if (itemSlots.Count == 0)
        {
            //clear selected item
            PlayerItemUser.Instance.OverrideSelectedItem(null, drop:false);
            return;
        } else
        {
            //prevent overflow when old index is outside range
            selectedItemSlotIndex = Mathf.Min(selectedItemSlotIndex, itemSlots.Count - 1);
        }

        PlayerItemUser.Instance.OverrideSelectedItem(itemSlots[selectedItemSlotIndex].Item);

        //visuals
        for (int i = 0; i < itemSlots.Count; i++)
        {
            ItemSlot slot = itemSlots[i];


            int index = i < selectedItemSlotIndex ? selectedItemSlotIndex + i : i - selectedItemSlotIndex;
            Debug.Log(index + " / " + itemSlots.Count);
            slot.RectInstance.SetSiblingIndex(index);

            float alpha = Mathf.Lerp(1f, 0f, index / 3f);

            for (int j = 0; j < slot.Modes.Length; j++)
            {
                ItemMode mode = slot.Modes[j];
                bool isSelected = i == selectedItemSlotIndex && j == slot.SelectedModeIndex;
                if (isSelected) currentSelectionImage = mode.SelectedImage;
                mode.SelectedImage.enabled = isSelected;
                mode.IconImage.color = new Color(1, 1, 1, alpha * (isSelected ? 1f : 0.6f));

                if (isSelected)
                    itemNameText.text = mode.Name;
            }
        }
    }

    public override void Show()
    {
        base.Show();

        CreateUIElementsForAllItems();
        UpdateSelectedItem();
    }

    public override void Hide()
    {
        //do never hide
    }

    public override void UpdateUI(PlayerInput input)
    {
        if (itemSlots.Count == 0)
            return;

        bool update = false;
        ItemSlot slot = itemSlots[selectedItemSlotIndex];

        if (input.ItemMenuUp)
        {
            if (selectedItemSlotIndex == 0)
                selectedItemSlotIndex = itemSlots.Count - 1;
            else
                selectedItemSlotIndex--;

            update = true;
        }
        else if (input.ItemMenuDown)
        {
            if (selectedItemSlotIndex == itemSlots.Count - 1)
                selectedItemSlotIndex = 0;
            else
                selectedItemSlotIndex++;

            update = true;
        }
        else if (slot.Modes.Length > 1)
        {
            if (input.ItemMenuLeft)
            {
                if (slot.SelectedModeIndex == 0)
                    slot.SelectedModeIndex = slot.Modes.Length - 1;
                else
                    slot.SelectedModeIndex--;

                update = true;
            }
            else if (input.ItemMenuRight)
            {
                if (slot.SelectedModeIndex == slot.Modes.Length - 1)
                    slot.SelectedModeIndex = 0;
                else
                    slot.SelectedModeIndex++;

                update = true;
            }
        }

        if (update)
            UpdateSelectedItem();
    }

    private void HideSelectionIndicator()
    {
        SetSelectionIndicatorAlpha(0.1f);
    }
    private void ShowSelectionIndicator()
    {
        SetSelectionIndicatorAlpha(1f);
    }
}

[System.Serializable]
public class ItemSlot
{
    public PlayerItem Item;
    public ItemMode[] Modes;
    public int SelectedModeIndex;
    public RectTransform RectInstance;
}

[System.Serializable]
public class ItemMode
{
    public Sprite Icon;
    public string Name;
    public Image IconImage;
    public Image SelectedImage;
}
