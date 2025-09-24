using System.Collections.Generic;
using Saves;
using UnityEngine;

public static class SaveManager
{
    private const string RecordsCountKey = "recordsCount";
    private const string RecordTimePrefixKey = "time_";
    private const string RecordDistancePrefixKey = "distance_";
    private const string RecordWidthPrefixKey = "width_";
    private const string RecordHeightPrefixKey = "height_";
    private const string RecordExitsCountPrefixKey = "exits_";
    
    public static int GetCountOfRecords()
    {
        return PlayerPrefs.GetInt(RecordsCountKey, 0);
    }
    
    public static void IncrementCountOfRecords()
    {
        var value = PlayerPrefs.GetInt(RecordsCountKey, 0);
        PlayerPrefs.SetInt(RecordsCountKey, ++value);
    }

    public static void SaveResult(float time, float distance)
    {
        var index = GetCountOfRecords();
        
        PlayerPrefs.SetFloat($"{RecordTimePrefixKey}{index.ToString()}", time);
        PlayerPrefs.SetFloat($"{RecordDistancePrefixKey}{index.ToString()}", distance);
        PlayerPrefs.SetInt($"{RecordWidthPrefixKey}{index.ToString()}", GameController.Instance.Width);
        PlayerPrefs.SetInt($"{RecordHeightPrefixKey}{index.ToString()}", GameController.Instance.Height);
        PlayerPrefs.SetInt($"{RecordExitsCountPrefixKey}{index.ToString()}", GameController.Instance.ExitCount);
        
        PlayerPrefs.Save();
        
        IncrementCountOfRecords();
    }

    public static List<ResultRecord> GetResultRecords()
    {
        var recordsCount = GetCountOfRecords();

        var result = new List<ResultRecord>();

        if (recordsCount == 0)
        {
            return result;
        }

        for (var i = recordsCount - 1; i >= 0; i--)
        {
            var record = new ResultRecord();
            record.Seconds = PlayerPrefs.GetFloat($"{RecordTimePrefixKey}{i.ToString()}");
            record.Distance = PlayerPrefs.GetFloat($"{RecordDistancePrefixKey}{i.ToString()}");
            record.Width = PlayerPrefs.GetInt($"{RecordWidthPrefixKey}{i.ToString()}");
            record.Height = PlayerPrefs.GetInt($"{RecordHeightPrefixKey}{i.ToString()}");
            record.ExitsCount = PlayerPrefs.GetInt($"{RecordExitsCountPrefixKey}{i.ToString()}");
            result.Add(record);
        }

        return result;
    }
}