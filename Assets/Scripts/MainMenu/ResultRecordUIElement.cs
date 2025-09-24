using System;
using Saves;
using TMPro;
using UnityEngine;

public class ResultRecordUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text sizeText;
    [SerializeField] private TMP_Text numOfExitstText;

    public void Init(ResultRecord record)
    {
        timeText.text = $"time:{TimeSpan.FromSeconds(record.Seconds).ToString(@"mm\:ss")}";
        distanceText.text = $"distance:{record.Distance.ToString("F2")}";
        sizeText.text = $"W:{record.Width} / H:{record.Height}";
        numOfExitstText.text = $"exits count:{record.ExitsCount}";
    } 
}
