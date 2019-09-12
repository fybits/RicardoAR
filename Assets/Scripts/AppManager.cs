using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    public static AppManager singleton;
    public enum RicardoState {FLEX, SCHEDULE, PUSHUP, NOTES};  // State machine states
    public RicardoState state = RicardoState.FLEX;      // State machine state
    bool stateChanged = true;                           // State machine flag
    public Animator ricardoAnimator;                    // Animator component of Ricardo GameObject
    public Renderer ricardoSchedule;                    // Ricardo Schedule Renderer reference
    public GameObject ricardoNotes;

    public ScheduleGenerator[] ricardoScheduleGenerator;
    public ScheduleGenerator scheduleGen;
    public Image schedule;                              // Schedule Renderer reference

    public Texture2D[] scheduleTextures = new Texture2D[4]; // Array that holds schedules textures

    public TMPro.TMP_InputField notes;
    public TMPro.TextMeshProUGUI notesRicardo;

    public Text debugText;              // UI Text for debug info on a screen
    public Dropdown groupDropdown;

    private int group = 0;              // Holds current group

    Dictionary<int, int> mapping = new Dictionary<int, int> {
        {0, 1 }, {1, 6 }, {2, 7}, {3, 8}, {4, 38}, {5, 43}, {6, 44}, {7, 45}
    };

    void Awake() {
        singleton = this;
    }

    private void Start() {
        //LoadSchedulesTextures(true);
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
            ricardoAnimator.SetInteger("State",(int)state);         // Sets animator's variable "State" to the new state
            if (state == RicardoState.SCHEDULE) {                   // If new state is Schedule
                ricardoNotes.SetActive(false);
                ricardoSchedule.gameObject.SetActive(true);         // Enables schedule gameobject on the scene
                for (int i = 0; i < 4; i++)
                    ricardoScheduleGenerator[i].Generate(NetworkManager.DownloadSchedule("" + mapping[group]), i);
            } else if (state == RicardoState.NOTES) {
                ricardoNotes.SetActive(true);
                ricardoSchedule.gameObject.SetActive(false);
            } else {
                ricardoSchedule.gameObject.SetActive(false);        // Disables if it's not
                ricardoNotes.SetActive(false);
            }
            stateChanged = false;
        }
    }

    // The function that calls from outside of the script by the UI
    public void SetState(int newState) {
        if (state == (RicardoState)newState) return;        // If state didn't changed return
        state = (RicardoState)newState;
        stateChanged = true;
        Debug.Log(state.ToString());
    }

    public void RefreshSchedule() {
        scheduleGen.Generate(NetworkManager.DownloadSchedule(""+mapping[group]));
    }

    public void LoadConfig() {
        Debug.LogError(Application.persistentDataPath);
        if (!File.Exists(Path.Combine(Application.persistentDataPath, "config.cfg"))) {
            
            Debug.Log("Config file not found");
            return;
        }
        string raw = File.ReadAllText(Path.Combine(Application.persistentDataPath, "config.cfg"));
        string[] data = raw.Split('\n');
        groupDropdown.value = int.Parse(data[0]);
        groupDropdown.RefreshShownValue();
        SetGroup(int.Parse(data[0]));
        for (int i = 1; i < data.Length-1; i++) {
            UIManager.singleton.showedTips.Add(data[i]);
        }
        Debug.Log("Config loaded");
    }

    private void OnApplicationQuit() {
        string config = group + "\n";
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
        notesRicardo.text = data;
        Debug.Log("Notes loaded");
    }

    public void SaveNotes() {
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "note.txt"), notes.text);
        Debug.Log("Notes saved");
    }


    public void LoadSchedulesTextures(bool force) {
        for (int i = 0; i < 4; i++) {
            byte[] fileData;
            string groupName = "1-"+((i != 0) ? (i + 5) : 1);
            string filePath = groupName+".png";
            if (!File.Exists(Path.Combine(Path.Combine(Application.persistentDataPath, "schedule/"),filePath)) || force) {
                Debug.Log("File not found. Downloading..");
                debugText.text += "File not found.Downloading..\n";
                // Downloading new schedule file if it isn't exist
                // Also must load new schedules if old isn't up to date
                // NetworkManager.DownloadSchedule(groupName);
            }
            // Read of file data to the byte array
            fileData = File.ReadAllBytes(Path.Combine(Path.Combine(Application.persistentDataPath, "schedule/"), filePath));
            // Creating new texture from those bytes
            scheduleTextures[i] = new Texture2D(1, 1);
            scheduleTextures[i].LoadImage(fileData);
            scheduleTextures[i].Apply();
            // Updating current texture
            Rect r = new Rect(0, 0, scheduleTextures[group].width, scheduleTextures[group].height);
            schedule.sprite = Sprite.Create(scheduleTextures[group],r,Vector2.one/2);
            ricardoSchedule.material.mainTexture = scheduleTextures[group];
        }
        debugText.text = "";
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
        //ricardoSchedule.material.mainTexture = scheduleTextures[group];
        //Rect r = new Rect(0, 0, scheduleTextures[group].width, scheduleTextures[group].height);
        //schedule.sprite = Sprite.Create(scheduleTextures[group], r, Vector2.one / 2);
    }
}
