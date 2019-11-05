using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
public class ScheduleGenerator : MonoBehaviour
{
    public GameObject gridHandle;
    public GameObject schedulePlane;


    public void Generate(string data, int day=-1) {
        for (int index = gridHandle.transform.childCount-1; index >= 0; index--)
            Destroy(gridHandle.transform.GetChild(index).gameObject);

        Transform elem0 = schedulePlane.transform.Find("elem0");
        if (elem0 != null) {
            elem0.parent = null;
            Destroy(elem0.gameObject);
            elem0 = schedulePlane.transform.Find("elem0");
            elem0.parent = null;
            Destroy(elem0.gameObject);
            elem0 = schedulePlane.transform.Find("Schedule header");
            elem0.parent = null;
            Destroy(elem0.gameObject);
        }
        JObject o = JObject.Parse(data);
        JArray table = (JArray)o["table"]["table"];

        DateTime now = DateTime.Now;
        int i = (day == -1) ? ((int)now.DayOfWeek+1) : day+2;
        if (i == 0)
            i = 2;
        JArray array = (JArray)table[i];
        GameObject header = new GameObject("Schedule header");
        header.transform.parent = schedulePlane.transform;
        TMPro.TextMeshProUGUI headerText = header.AddComponent<TMPro.TextMeshProUGUI>();
        headerText.text = o["table"].Value<string>("name");
        headerText.fontSize = Screen.height/21;
        RectTransform headerRT = header.GetComponent<RectTransform>();
        headerRT.anchorMin = new Vector2(0, 1);
        headerRT.anchorMax = new Vector2(0, 1);
        headerRT.sizeDelta = new Vector2(Screen.width / 2, 200);
        headerRT.pivot = new Vector2(-0.15f, 1.25f);
        headerRT.anchoredPosition = new Vector2(0, 0);
        CreateColumn((JArray)table[0], 0.2f);
        CreateColumn(array, 0.7f);
        gridHandle.GetComponent<HorizontalLayoutGroup>().padding = new RectOffset((int)(Screen.width * 0.1f), 0, 250, 0);
        //}
    }
    public void CreateColumn(JArray array, float width) {
        GameObject column = new GameObject("row");
        column.transform.parent = gridHandle.transform;
        column.transform.rotation = gridHandle.transform.rotation;
        RectTransform rt = column.AddComponent<RectTransform>();
        rt.pivot = new Vector2(0, 1);
        rt.sizeDelta = new Vector2(Screen.width*width, rt.sizeDelta.y);
        column.transform.localScale = Vector3.one;
        column.transform.localPosition = Vector3.zero;
        VerticalLayoutGroup vlg = column.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = Screen.height / (array.Count);
        for (int j = 0; j < array.Count; j++) {
            GameObject elem = new GameObject("elem" + j);
            elem.transform.parent = column.transform;
            elem.transform.rotation = column.transform.rotation;
            RectTransform elemRT = elem.AddComponent<RectTransform>();
            elemRT.pivot = new Vector2(0.5f, 1);
            TMPro.TextMeshProUGUI text = elem.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = array.Value<string>(j);
            text.fontSize = 36;
            elemRT.sizeDelta = new Vector2(Screen.width * width, elemRT.sizeDelta.y);
            elemRT.anchoredPosition = Vector2.zero;
            elemRT.localPosition = Vector3.zero;
            elemRT.localScale = new Vector3(1, 1, 1);
            if (j == 0) {
                elem.transform.parent = schedulePlane.transform;
                elemRT.anchorMin = new Vector2(0, 1);
                elemRT.anchorMax = new Vector2(0, 1);
                elemRT.pivot = new Vector2(-0.5f, 4.5f);
            }
        }
    }
}
