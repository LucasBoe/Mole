using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : SingletonBehaviour<UIHandler>
{
    public static UITemporarySpawner Temporary;

    [SerializeField] PlayerItemUI itemUI;
    [SerializeField] UITemporarySpawner temporarySpawner;

    PlayerInput input;
    List<IUpdateMeWithInput> toUpdate = new List<IUpdateMeWithInput>();

    private void OnEnable()
    {
        Temporary = temporarySpawner;
        input = PlayerInputHandler.PlayerInput;
    }

    private void Update()
    {
        for (int i = toUpdate.Count - 1; i >= 0; i--)
        {
            IUpdateMeWithInput element = toUpdate[i];
            if (element != null)
                element.UpdateUI(input);
        }
    }

    public bool isVisible(UIBehaviour behaviour)
    {
        return toUpdate.Contains(behaviour);
    }

    public void Show(UIBehaviour behaviour)
    {
        if (!toUpdate.Contains(behaviour))
            toUpdate.Add(behaviour);

        behaviour.Show();
    }
    public void Hide(UIBehaviour behaviour)
    {
        if (toUpdate.Contains(behaviour))
            toUpdate.Remove(behaviour);

        behaviour.Hide();
    }
}
