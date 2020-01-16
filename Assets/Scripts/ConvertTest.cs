using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConvertTest : MonoBehaviour
{

    public string binStr;
    public string hexStr;

    public int speedLimit;
    public int distStop;
    public int distBrunch;
    public bool xReadInessCation;
    public bool xReadInessWarning;
    public bool xDisptor;
    public bool draiverFailWarning;
    public bool xSleepWarning;
    public bool kinattentiveWarning;

    // Start is called before the first frame update
    void Start()
    {

        // 2進数文字列、16進数文字列を数値へ変換
        long v1 = Convert.ToInt64(binStr, 2);
        long v2 = Convert.ToInt64(hexStr, 16);

        // 数値を2進数文字列、16進数文字列へ変換

        binStr = Convert.ToString(v1, 2);
        hexStr = Convert.ToString(v2, 16);

        //string hexResult = BinToHex(binStr);

        //string binResult = HexToBin(hexStr);
    }

    // 2進数→16進数
    public string BinToHex(string _binStr)
    {
        return Convert.ToString(Convert.ToInt64(_binStr, 2), 16);
    }

    // 16進数→2進数
    public string HexToBin(string _hexStr)
    {
        return Convert.ToString(Convert.ToInt64(_hexStr, 16), 2);
    }

    //指定した文字列の中身を切り取る関数
    //「str」の中身を「firstNum」から「length」の数字分の文字列を返す
    //str:文字列
    //firstNum:開始位置
    //length:開始位置から返す予定の文字の数
    public string StringCuter(string str, int firstNum, int length)
    {
        return str.Substring(firstNum, length);
    }

    //文字列（2進数）を10進数のintに変換して返す
    //「binStr」の中身を10進数に変換して返す
    public long BinToDecIntChanger(string binStr)
    {
        long decStr = Convert.ToInt64(binStr, 2);
        return decStr;
    }

    public bool BinToDecBoolChanger(string binStr)
    {
        long decStr = Convert.ToInt64(binStr, 2);
        if (decStr == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //public long ConvertValue(string str, int firstNum, int length)
    //{
    //    string binResult = HexToBin(str);

    //    return BinToDecIntChanger(StringCuter(binResult, firstNum, length));
    //}

    //public int ConvertValue_2(List<string[]> data, string str,int count)
    //{
    //    string binResult = HexToBin(str);

    //    return BinToDecIntChanger(StringCuter(binResult, Int64.Parse(data[count][1]), Int64.Parse(data[count][2])));
    //}

}
