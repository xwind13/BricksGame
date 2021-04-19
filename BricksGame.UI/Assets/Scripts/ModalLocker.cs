using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ModalLocker : MonoBehaviour
{
    Selectable[] selectables;
    GameField gameField;

    void OnEnable()
    {
        selectables = FindObjectsOfType<Selectable>().Where(s => s.interactable && !s.transform.IsChildOf(transform)).ToArray();
        foreach (var selectable in selectables)
        {
            selectable.interactable = false;
            //Debug.Log($"{selectable} disabled");
        }

        gameField = FindObjectOfType<GameField>();
        gameField.Block();
    }

    void OnDisable()
    {
        if (selectables == null)
            return;

        foreach (var selectable in selectables)
            selectable.interactable = true;
        selectables = null;

        gameField.Unblock();
    }
}
