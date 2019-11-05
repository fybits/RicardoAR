using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager singleton;
    public GameObject accountWindow;    // References to UI elements
    public GameObject infoWindow;
    public GameObject scheduleWindow;
    public GameObject notesWindow;
    public GameObject exercisesWindow;
    public GameObject btnHolder;
    public GameObject btnBack;
    public GameObject btnInfo;
    public GameObject tip;

    public ModeSwitcher modeSwitcher;

    public GameObject characterSelectorWindow;

    public GameObject planeFinder;      // Reference to GameObject with PlaneFinder script

    public List<string> showedTips = new List<string>();

    // State(Time) machine stuff        // Rick and Morty reference
    public enum WindowState { DEFAULT, ACCOUNT, INFO, SCHEDULE, NOTES, EXERCISES };
    WindowState state = WindowState.DEFAULT;
    bool stateChanged = true;
    private bool isTipVisible = false;

    private void Awake() {
        singleton = this;
    }

    // Update is called once per frame
    void Update() {
        if (stateChanged) {
            switch (state) {
                case WindowState.DEFAULT:
                    ShowTip("Добро пожаловать.");
                    accountWindow.SetActive(false);
                    infoWindow.SetActive(false);
                    scheduleWindow.SetActive(false);
                    notesWindow.SetActive(false);
                    exercisesWindow.SetActive(false);
                    btnHolder.SetActive(true);
                    btnBack.SetActive(false);
                    btnInfo.SetActive(false);
                    planeFinder.SetActive(true);
                    break;
                case WindowState.ACCOUNT:
                    ShowTip("Добрый день, славянин, здесь ты можешь настроить свой профиль.");
                    accountWindow.SetActive(true);
                    infoWindow.SetActive(false);
                    scheduleWindow.SetActive(false);
                    notesWindow.SetActive(false);
                    exercisesWindow.SetActive(false);
                    btnHolder.SetActive(false);
                    btnBack.SetActive(true);
                    btnInfo.SetActive(true);
                    planeFinder.SetActive(false);   // Disabling planefinder so its doesn't triggers when you on the account screen
                    break;
                case WindowState.INFO:
                    ShowTip("Здесь находится информация об приложении.");
                    accountWindow.SetActive(false);
                    scheduleWindow.SetActive(false);
                    infoWindow.SetActive(true);
                    notesWindow.SetActive(false);
                    exercisesWindow.SetActive(false);
                    btnInfo.SetActive(false);
                    planeFinder.SetActive(false);
                    break;
                case WindowState.SCHEDULE:
                    ShowTip("Расписание твоих занятий.");
                    accountWindow.SetActive(false);
                    infoWindow.SetActive(false);
                    scheduleWindow.SetActive(true);
                    notesWindow.SetActive(false);
                    exercisesWindow.SetActive(false);
                    btnHolder.SetActive(false);
                    btnBack.SetActive(true);
                    btnInfo.SetActive(false);
                    planeFinder.SetActive(false);
                    AppManager.singleton.RefreshSchedule();
                    break;
                case WindowState.NOTES:
                    ShowTip("Твои заметки.");
                    accountWindow.SetActive(false);
                    infoWindow.SetActive(false);
                    scheduleWindow.SetActive(false);
                    notesWindow.SetActive(true);
                    exercisesWindow.SetActive(false);
                    btnHolder.SetActive(false);
                    btnBack.SetActive(true);
                    btnInfo.SetActive(false);
                    planeFinder.SetActive(false);
                    break;
                case WindowState.EXERCISES:
                    ShowTip("Упражняйся со своим ассистентом.");
                    accountWindow.SetActive(false);
                    infoWindow.SetActive(false);
                    scheduleWindow.SetActive(false);
                    notesWindow.SetActive(false);
                    exercisesWindow.SetActive(true);
                    btnHolder.SetActive(false);
                    btnBack.SetActive(true);
                    btnInfo.SetActive(false);
                    planeFinder.SetActive(false);
                    break;
            }
            stateChanged = false;
        }
    }

    // Calls from outside (UI)
    public void OnBackPressed() {
        switch (state) {
            case WindowState.ACCOUNT:
                state = WindowState.DEFAULT;
                break;
            case WindowState.INFO:
                state = WindowState.ACCOUNT;
                break;
            case WindowState.SCHEDULE:
                state = WindowState.DEFAULT;
                break;
            case WindowState.NOTES:
                state = WindowState.DEFAULT;
                AppManager.singleton.SaveNotes();
                break;
            case WindowState.EXERCISES:
                state = WindowState.DEFAULT;
                modeSwitcher.SelectOption(modeSwitcher.lastMode);
                break;
        }
        stateChanged = true;
    }

    public void ChangeState(int newState) {
        state = (WindowState)newState;
        stateChanged = true;
    }

    // Calls when dropdown value changes
    public void OnGroupChanged(Dropdown dropDown) {
        AppManager.singleton.SetGroup(dropDown.value);
    }

    public void OnModelChanged(Dropdown dropDown) {
        //AppManager.singleton.isShronk = dropDown.value == 1;
    }

    public void OpenSelectMenu() {
        characterSelectorWindow.SetActive(true);
    }

    public void CloseSelectMenu() {
        characterSelectorWindow.SetActive(false);
    }

    public void SelectExercise(int exer) {
        exercisesWindow.SetActive(false);
        AppManager.singleton.SetExercisesState(exer);
    }


    public void ClearTips() {
        showedTips.Clear();
    }

    public void ShowTip(string message) {
        if (showedTips.Contains(message))
            return;
        showedTips.Add(message);
        tip.GetComponentInChildren<TextMeshProUGUI>().text = message;
        isTipVisible = true;
        tip.SetActive(true);
    }

    public void HideTip() {
        tip.SetActive(false);
        isTipVisible = false;
    }

    public bool IsTipVisible() { return isTipVisible; }
}