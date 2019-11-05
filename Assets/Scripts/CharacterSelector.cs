using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    public Image[] items;
    public Color selectedColor;
    public Color defaultColor;

    [SerializeField]
    int selectedItem = 0;

    public void Select(int i) {
        items[selectedItem].color = defaultColor;
        items[i].color = selectedColor;
        selectedItem = i;
    }

    public void Submit() {
        AppManager.singleton.SetCharacter(selectedItem);
        UIManager.singleton.CloseSelectMenu();
    }
}
