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
    [SerializeField] private List<ItemSlot> itemSlots;
    [SerializeField] private int selectedItemSlotIndex = 0;

    [SerializeField] RectTransform parent;
    [SerializeField] Text itemNameText;

    [SerializeField] RectTransform itemSlotPrefab;
    [SerializeField] Image itemSlotModePrefab;

    private void Start()
    {
        UIHandler.Instance.Show(this);
    }

    private void CreateUIElementsForAllItems()
    {
        foreach (ItemSlot slot in itemSlots)
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
    }

    private void UpdateSelectedItem()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            ItemSlot slot = itemSlots[i];
            int index = i < selectedItemSlotIndex ? selectedItemSlotIndex + i : i - selectedItemSlotIndex;
            slot.RectInstance.SetSiblingIndex(index);

            float alpha = Mathf.Lerp(1f, 0f, index / 3f);

            for (int j = 0; j < slot.Modes.Length; j++)
            {
                ItemMode mode = slot.Modes[j];
                bool isSelected = i == selectedItemSlotIndex && j == slot.SelectedModeIndex;
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
}

[System.Serializable]
public class ItemSlot
{
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
