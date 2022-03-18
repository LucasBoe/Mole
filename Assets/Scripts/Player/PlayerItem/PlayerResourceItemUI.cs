using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerResourceItemUI : MonoBehaviour
{
    [SerializeField] GameObject resourceItemUIElement;
    Dictionary<PlayerItem, GameObject> resourceItemUIInstances = new Dictionary<PlayerItem, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        resourceItemUIElement.SetActive(false);
        PlayerItemHolder.AddedItem += OnAddedItem;
        PlayerItemHolder.RemovedItem += OnRemovedItem;
        PlayerItemHolder.Changedtem += OnChangedItem;
    }

    private void OnChangedItem(PlayerItem item)
    {
        if (item.IsUseable)
            return;

        UpdateInstance(resourceItemUIInstances[item], item);
    }

    private void OnRemovedItem(PlayerItem item)
    {
        if (item.IsUseable)
            return;

        Destroy(resourceItemUIInstances[item]);
        resourceItemUIInstances.Remove(item);
    }

    private void OnInitItems(Dictionary<PlayerItem, int>.KeyCollection keys)
    {
        foreach (var item in keys)
        {
            if (!item.IsUseable)
                OnAddedItem(item);
        }
    }

    private void OnAddedItem(PlayerItem item)
    {
        if (item.IsUseable)
            return;

        GameObject instance = Instantiate(resourceItemUIElement, transform);
        instance.SetActive(true);
        UpdateInstance(instance, item);
        resourceItemUIInstances.Add(item, instance);
    }

    private void UpdateInstance(GameObject instance, PlayerItem item)
    {
        instance.GetComponentInChildren<Image>().sprite = item.Sprite;
        instance.GetComponentInChildren<TMP_Text>().text = PlayerItemHolder.Instance.GetAmount(item).ToString();
    }
}
