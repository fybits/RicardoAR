using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterData {
        public Animator characterAnimator;
        public GameObject schedule;
        public GameObject notes;
        public TMPro.TMP_Text notesText;
        public ScheduleGenerator[] scheduleGenerators;
    }


    public CharacterData[] characters;

    public int currentCharacter;
    
    public CharacterData GetCurrentCharacter() {
        return characters[currentCharacter];
    }

    public void SelectCharacter(int newC) {
        currentCharacter = newC;
        for (int i = 0; i < characters.Length; i++) {
            characters[i].characterAnimator.gameObject.SetActive(i == newC);
        }
    }


}
