using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    public static AppManager singleton;
    public enum RicardoState {FLEX, SCHEDULE, EXERCISES, NOTES};  // State machine states
    public enum ExercisesState {PUSHUPS, SQUATS, CAPOEIRA};

    public RicardoState state = RicardoState.FLEX;      // State machine state
    bool stateChanged = true;                           // State machine flag

    public Text debugText;              // UI Text for debug info on a screen
    public Dropdown groupDropdown;

    private CharacterManager characters;

    public ScheduleGenerator scheduleGen;
    public TMPro.TMP_InputField notes;

    private int group = 0;              // Holds current group

    Dictionary<int, int> mapping = new Dictionary<int, int> {
        {0, 1 }, {1, 6 }, {2, 7}, {3, 8}, {4, 38}, {5, 43}, {6, 44}, {7, 45}
    };

    void Awake() {
        singleton = this;
    }

    private void Start() {
        Debug.Log(Application.persistentDataPath);
        //LoadSchedulesTextures(true);
        characters = GetComponent<CharacterManager>();
        characters.SelectCharacter(0);
        SetGroup(group);            // Sets group to default 0 (КТбо1-1)
        LoadConfig();
        LoadNotes();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
            return;
        }
        // Default state machine routine
        if (stateChanged) {
            CharacterManager.CharacterData curCharacter = characters.GetCurrentCharacter();
              curCharacter.characterAnimator.SetInteger("State", (int)state);         // Sets animator's variable "State" to the new state
            if (state == RicardoState.EXERCISES) {
                UIManager.singleton.ChangeState((int)UIManager.WindowState.EXERCISES);
                curCharacter.schedule.SetActive(false);
                curCharacter.notes.SetActive(false);
            } else {
                if (state == RicardoState.SCHEDULE) {                   // If new state is Schedule
                    curCharacter.notes.SetActive(false);
                    curCharacter.schedule.SetActive(true);
                    for (int i = 0; i < 4; i++)
                        curCharacter.scheduleGenerators[i].Generate(NetworkManager.DownloadSchedule("" + mapping[group]), i);
                } else if (state == RicardoState.NOTES) {
                    curCharacter.notes.SetActive(true);
                    curCharacter.schedule.SetActive(false);
                } else {
                    curCharacter.schedule.SetActive(false);
                    curCharacter.notes.SetActive(false);
                }
            }
            stateChanged = false;
        }
    }

    // The function that calls from outside of the script by the UI
    public void SetState(int newState) {
        //if (state == (RicardoState)newState) return;        // If state didn't changed return
        state = (RicardoState)newState;
        stateChanged = true;
        Debug.Log(state.ToString());
    }

    public void SetExercisesState(int exer) {
        characters.GetCurrentCharacter().characterAnimator.SetInteger("Exercise", exer);
        Debug.Log("Exer: " + exer);
    }

    public void RefreshSchedule() {
        scheduleGen.Generate(NetworkManager.DownloadSchedule(""+mapping[group]));
    }

    public void SetCharacter(int i) {
        characters.SelectCharacter(i);
        SetState((int)state);
    }


    public void LoadConfig() {
        Debug.LogError(Application.persistentDataPath);
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "config.cfg"))) {
            UIManager.singleton.OpenSelectMenu();
            Debug.Log("Config file not found");
            return;
        }
        string raw = File.ReadAllText(Path.Combine(Application.persistentDataPath, "config.cfg"));
        string[] data = raw.Split('\n');
        groupDropdown.value = int.Parse(data[0]);
        groupDropdown.RefreshShownValue();
        SetGroup(int.Parse(data[0]));
        SetCharacter(int.Parse(data[1]));
        for (int i = 2; i < data.Length-1; i++) {
            UIManager.singleton.showedTips.Add(data[i]);
        }
        Debug.Log("Config loaded");
    }


    private void OnApplicationQuit() {
        string config = group + "\n" + characters.currentCharacter+"\n";
        for (int i = 0; i < UIManager.singleton.showedTips.Count; i++) {
            config += UIManager.singleton.showedTips[i] + "\n";
        }
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "config.cfg"), config);
    }

    public void LoadNotes() {
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "note.txt"))) {
            notes.text = "Hello world!";
            Debug.Log("Notes file not found");
        }
        string data = File.ReadAllText(Path.Combine(Application.persistentDataPath, "note.txt"));
        notes.text = data;
        characters.GetCurrentCharacter().notesText.text = data;
        Debug.Log("Notes loaded");
    }

    public void SaveNotes() {
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "note.txt"), notes.text);
        characters.GetCurrentCharacter().notesText.text = notes.text;
        Debug.Log("Notes saved");
    }


    public void RotateToCamera(GameObject go) {
        // This shit works wrong.
        // It's meant to rotate Ricardo at the moment he appears on the screen to the camera.
        Vector3 flatPosition = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 flatGOPosition = new Vector3(go.transform.forward.x, 0, go.transform.forward.z);
        go.transform.Rotate(Vector3.up, Vector3.Angle(flatPosition, flatGOPosition));
    }

    // The function that calls from outside of the script by the UI
    public void SetGroup(int groupID) {
        group = groupID;
    }
}
