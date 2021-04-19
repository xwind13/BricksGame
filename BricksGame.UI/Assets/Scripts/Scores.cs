using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    [SerializeField] private Text _scoreUserList;
    [SerializeField] private Text _scoreList;

    // Update is called once per frame
    public void Show()
    {
        var json = PlayerPrefs.GetString("UserStat");
        var value = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(json);

        _scoreUserList.text = value != null ? string.Join("\n\n", value.Select(p => p.Key).ToArray()) : "";
        _scoreList.text = value != null ? string.Join("\n\n", value.Select(p => p.Value).ToArray()) : "";

        this.gameObject.SetActive(true);
    }
}
