using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSwitcher : MonoBehaviour
{
    public Animator animator;               // Reference to Switcher Animator
    public GameObject[] options;            // GameObject array with all options
    
    private int m_selectedOption = 0;
    public bool opened = false;

    // Calls when one of the ModeSwitcher buttons pressed
    public void SelectOption (int option) {
        // If new option equals selected option closing the ModeSwitcher
        if (m_selectedOption == option) {
            opened = !opened;
            animator.SetBool("opened", opened);
            return;
        }
        // Otherwise 
        opened = false;
        animator.SetBool("opened", opened);
        // Swapping sprites of buttons
        Sprite tempSprite = options[m_selectedOption].GetComponent<Image>().sprite;
        options[m_selectedOption].GetComponent<Image>().sprite = options[option].GetComponent<Image>().sprite;
        options[option].GetComponent<Image>().sprite = tempSprite;
        // Swapping buttons events
        Button.ButtonClickedEvent tempEvent = options[m_selectedOption].GetComponent<Button>().onClick;
        options[m_selectedOption].GetComponent<Button>().onClick = options[option].GetComponent<Button>().onClick;
        options[option].GetComponent<Button>().onClick = tempEvent;
        // Swapping options in GameObject array
        GameObject tempGO = options[m_selectedOption];
        options[m_selectedOption] = options[option];
        options[option] = tempGO;

        m_selectedOption = option;
        AppManager.singleton.SetState(m_selectedOption);
    }

    // Getter for selected option
    public int GetValue() {
        return m_selectedOption;
    }
}
