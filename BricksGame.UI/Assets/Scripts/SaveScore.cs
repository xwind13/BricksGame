using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SaveScore : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private InputField _userNameText;

    public void Save()
    {
        var json = PlayerPrefs.GetString("UserStat");
        var value = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(json);
        var playerName = string.IsNullOrEmpty(_userNameText.text) ? "__anonym__" : _userNameText.text;

        if (value != null && value.Any())
        {
            value.Add(new KeyValuePair<string, string>(playerName, _scoreText.text));
            value.Sort((p1, p2) => int.Parse(p2.Value) - int.Parse(p1.Value));
            value = value.Take(10).ToList();
            SaveResult(value);
            return;
        }

        value = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>(playerName, _scoreText.text)
        };

        SaveResult(value);
    }

    private void SaveResult(List<KeyValuePair<string, string>> value)
    {
        _userNameText.text = "";
        PlayerPrefs.SetString("UserStat", Newtonsoft.Json.JsonConvert.SerializeObject(value));
        PlayerPrefs.Save();
    }
}
