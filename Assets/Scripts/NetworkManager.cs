using System.Net;
using System.IO;
using UnityEngine;
using System;
using System.Text;

class NetworkManager {

    // Downloads html of given urlW
    public static string DownloadSchedule(string group) {
        Debug.Log("Loading " + group);
        string html = string.Empty;
        string url = "http://165.22.28.187/schedule-api/?group="+group+".htm";
        // Creates request to given url
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        string data = "";
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream()) {
            using (StreamReader reader = new StreamReader(stream)) {
                data = reader.ReadToEnd();
                reader.Close();
            }
            stream.Close();
        }
        return data;
    }
}
