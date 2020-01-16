using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;


public class Debug_Mode : MonoBehaviour
{
    public InputField[] texts1;
    public InputField[] texts2;
    public InputField[] texts3;
    public InputField[] texts4;

    public string group1;
    public string group2;
    public string group3;
    public string group4;

    public string resultGroup1;
    public string resultGroup2;
    public string resultGroup3;
    public string resultGroup4;

    public DataBase _dataBase;
    public ConvertTest _convertTest;
    public TCPTestClientGUI _tCPTestClientGUI;

    public InputField message;

    // Start is called before the first frame update
    void Start()
    {
        WriteName(texts1, 501);
        WriteName(texts2, 502);
        WriteName(texts3, 503);
        WriteName(texts4, 504);
    }

    // Update is called once per frame
    void Update()
    {
        resultGroup1 = HexChanger(texts1, group1, 501);
        resultGroup1=ChangeString(resultGroup1, "501");

        resultGroup2 = HexChanger(texts2, group2, 502);
        resultGroup2 = ChangeString(resultGroup2, "502");

        resultGroup3 = HexChanger(texts3, group3, 503);
        resultGroup3 = ChangeString(resultGroup3, "503");

        resultGroup4 = HexChanger(texts4, group4, 504);
        resultGroup4 = ChangeString(resultGroup4, "504");

        //_tCPTestClientGUI.sendMessage=(resultGroup1 + resultGroup2 + resultGroup3 + resultGroup4);

        message.text = (resultGroup1 + resultGroup2 + resultGroup3 + resultGroup4);
    }

    public string HexChanger(InputField[] texts,string res,int num)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            if (i == 0)
            {
                res = "";
            }
            if (texts[i].text != "")
            {
                long a = Int64.Parse(texts[i].text);
                switch (num)
                {
                    case 501:
                        res += Convert.ToString(a, 2).PadLeft(Int32.Parse(_dataBase.tableData_501[i][2]), '0');
                        break;
                    case 502:
                        res += Convert.ToString(a, 2).PadLeft(Int32.Parse(_dataBase.tableData_502[i][2]), '0');
                        break;
                    case 503:
                        res += Convert.ToString(a, 2).PadLeft(Int32.Parse(_dataBase.tableData_503[i][2]), '0');
                        break;
                    case 504:
                        res += Convert.ToString(a, 2).PadLeft(Int32.Parse(_dataBase.tableData_504[i][2]), '0');
                        break;
                }
            }

        }

        if (res != ""&&res!="0")
        {
            //Debug.Log(res);
            res = Convert.ToString(Convert.ToInt64(res, 2), 16);
        }

        return res;
    }

    public string ChangeString(string str, string num)
    {
        str = str.PadLeft(16, '0');

        for (int i = 0; i < str.Length; i++)
        {
            if (i % 3 == 0)
            {
                str = str.Insert(i, " ");
            }
        }
        str = "M 1 CSD " + num + " " + str + "|";

        return str;
    }

    public void WriteName(InputField[] texts,int num)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            switch (num)
            {
                case 501:
                    texts[i].gameObject.GetComponentsInChildren<Text>()[2].text = _dataBase.tableData_501[i][0];
                    break;
                case 502:
                    texts[i].gameObject.GetComponentsInChildren<Text>()[2].text =_dataBase.tableData_502[i][0];
                    break;
                case 503:
                    texts[i].gameObject.GetComponentsInChildren<Text>()[2].text = _dataBase.tableData_503[i][0];
                    break;
                case 504:
                    texts[i].gameObject.GetComponentsInChildren<Text>()[2].text = _dataBase.tableData_504[i][0];
                    break;
            }
            
        }

    }
}
