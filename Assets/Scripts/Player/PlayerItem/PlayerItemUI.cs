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
    [SerializeField] private int selectedItemSlotIndex;

    [SerializeField] RectTransform itemSlotPrefab;
    [SerializeField] Image itemSlotModePrefab;

    [SerializeField] private PlayerItem[] defaultItems;
    [SerializeField] private Button prefab;

    int selectedId = 0;
    Button[] buttons;

    private void Start()
    {
        foreach (ItemSlot slot in itemSlots)
        {
            slot.RectInstance = Instantiate(itemSlotPrefab, transform);

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

    public override void Show()
    {
        buttons = new Button[defaultItems.Length + 1];

        for (int i = 0; i <= defaultItems.Length; i++)
        {
            bool iForNone = i == defaultItems.Length;
            PlayerItem item = iForNone ? null : defaultItems[i];
            Button instance = Instantiate(prefab, transform);
            Image image = instance.GetComponentInChildrenExcludeOwn<Image>();

            if (!iForNone)
                image.sprite = item.Sprite;
            else
                Destroy(image);

            string text = iForNone ? "None" : item.name;
            instance.GetComponentInChildren<Text>().text = text;

            int index = i;
            instance.onClick.AddListener(delegate { Select(index); });

            buttons[i] = instance;
        }

        Select(selectedId);

        base.Show();
    }

    public override void Hide()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        base.Hide();
    }

    private void Select(int selectedId)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            bool shouldBeSelected = i == selectedId;
            Button Button = buttons[i];
            Transform trans = Button.transform;
            Image button = Button.GetComponent<Image>();

            trans.localScale = Vector3.one * (shouldBeSelected ? 1.1f : 1);
            button.color = shouldBeSelected ? Color.green : Color.white;
        }

        PlayerItem playerItem = null;

        if (selectedId < defaultItems.Length)
            playerItem = defaultItems[selectedId];

        PlayerItemUser.Instance.TryOverrideActiveItem(playerItem);
    }

    public override void UpdateUI(PlayerInput input)
    {
        if (input.DPadUp)
        {
            if (selectedId == 0)
                selectedId = defaultItems.Length;
            else
                selectedId--;

            Select(selectedId);
        }
        else if (input.DPadDown)
        {
            if (selectedId == defaultItems.Length)
                selectedId = 0;
            else
                selectedId++;

            Select(selectedId);
        }


        if (input.Back)
            HideSelf();
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
