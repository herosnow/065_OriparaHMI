using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CSVReader : MonoBehaviour
{
    public List<string[]> scenario = new List<string[]>();

    private void Start()
    {
        //scenario[0] = (new ArraySegment<string>(ScenarioCsvRead("Scenario")[0], 1, 9));
        scenario = ScenarioCsvRead("Scenario");
    }

    public List<string[]> CsvRead(string csvName)
    {

        List<string[]> csvDatas = new List<string[]>();

        // csvをロード
        TextAsset csv = Resources.Load("CSV/" + csvName) as TextAsset;
        //Debug.Log(csvName);
        StringReader reader = new StringReader(csv.text);
        reader.ReadLine();
        while (reader.Peek() > -1)
        {
            // ','ごとに区切って配列へ格納
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
        }

        //for (int i = 0; i < csvDatas.Count; i++)
        //{
        //    for (int j = 0; j < csvDatas[i].Length; j++)
        //    {
        //        Debug.Log(csvName+" : "+"csvDatas[" + i + "][" + j + "] = " + csvDatas[i][j]);
        //    }
        //}
        return csvDatas;
    }

    public List<string[]> ScenarioCsvRead(string csvName)
    {

        List<string[]> csvDatas = new List<string[]>();

        // csvをロード
        TextAsset csv = Resources.Load("CSV/" + csvName) as TextAsset;
        //Debug.Log(csvName);
        StringReader reader = new StringReader(csv.text);
        reader.ReadLine();
        reader.ReadLine();
        while (reader.Peek() > -1)
        {
            // ','ごとに区切って配列へ格納
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(','));
        }

        //for (int i = 0; i < csvDatas.Count; i++)
        //{
        //    for (int j = 0; j < csvDatas[i].Length; j++)
        //    {
        //        Debug.Log(csvName + " : " + "csvDatas[" + i + "][" + j + "] = " + csvDatas[i][j]);
        //    }
        //}

        return csvDatas;
    }
}