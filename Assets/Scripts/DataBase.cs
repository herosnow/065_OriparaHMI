using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;

public class DataBase : MonoBehaviour
{
    public CSVReader cSVReader;
    public TCPTestClientGUI _tCPTestClientGUI;

    public List<string[]> tableData_501 = new List<string[]>();
    public List<string[]> tableData_502 = new List<string[]>();
    public List<string[]> tableData_503 = new List<string[]>();
    public List<string[]> tableData_504 = new List<string[]>();

    private GameObject[] dataObj_501 = new GameObject[100];
    private GameObject[] dataObj_502 = new GameObject[100];
    private GameObject[] dataObj_503 = new GameObject[100];
    private GameObject[] dataObj_504 = new GameObject[100];

    public string receiveData;

    public Transform parentObj_501;
    public Transform parentObj_502;
    public Transform parentObj_503;
    public Transform parentObj_504;
    public Transform receiveDataTransform;

    public GameObject switchObj501;
    public GameObject switchObj502;
    public GameObject switchObj503;
    public GameObject switchObj504;
    public GameObject switchObjReceiveData;
	
	private float allertTime;

    public Toggle toggle;

    public int maxLogNum;
    private string[] log = new string[10000];

    public Text receiveDataText;
    public List<string> receiveDatas;
    public List<string> logDatas;
    public GameObject prefabObj;

    private bool receive;

    //public string[] scenarioCheck = new string[8];
    public int[] scenarioCheck_Int{get; private set;} = new int[100];
    public int nowMode = -1;
    private int oldMode = -1;
    public int arrowNum;

    public AudioClip[] audioClips;
    public AudioClip[] audioClips_ENG;
    public AudioClip[] audioClipsKAN;

    public bool instantiateText;

    public GameObject[] debugUI;

    [Header("グリッド停止基準")]
    public double multiCoefficient = 0;
    public double stopCriteria = 0;
    public double duration = 0;

    //グリッド停止判断bool
    [NonSerialized]
    public bool isGridStop = false;


    //DSM制御用車速の監視
    List<double> speedForDsmControl = new List<double>();

    // Start is called before the first frame update
    void Awake()
    {
        tableData_501 = cSVReader.CsvRead("501");
        tableData_502 = cSVReader.CsvRead("502");
        tableData_503 = cSVReader.CsvRead("503");
        tableData_504 = cSVReader.CsvRead("504");

        //DebugModeのテキストの初期化
        if (instantiateText)
        {
            InstantiateText(tableData_501, dataObj_501, parentObj_501);
            InstantiateText(tableData_502, dataObj_502, parentObj_502);
            InstantiateText(tableData_503, dataObj_503, parentObj_503);
            InstantiateText(tableData_504, dataObj_504, parentObj_504);
        }

        //for (int i = 0; i < tableData_501.Count; i++)
        //{
        //    for (int j = 0; j < tableData_501[i].Length; j++)
        //    {
        //        Debug.Log("table[" + i + "][" + j + "] = " + tableData_501[i][j]);
        //    }
        //}

        

        //for (int i = 0; i < scenarioCheck.Length; i++)
        //{
        //    scenarioCheck[i] = "0";
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //DSM制御用車速監視メソッド
        //Debug.LogWarning("wowowowowowowo ::: " + tableData_504[0][3]);
        CheckSpeedForDsmControl();


        if (tableData_501.Count > 0)
        {
            UpdateData(tableData_501, dataObj_501);
            UpdateData(tableData_502, dataObj_502);
            UpdateData(tableData_503, dataObj_503);
            UpdateData(tableData_504, dataObj_504);
        }

        var update = toggle.isOn;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            for(int i = 0; i < debugUI.Length; i++)
            {
                if (debugUI[i].activeSelf)
                {
                    debugUI[i].SetActive(false);
                }
                else
                {
                    debugUI[i].SetActive(true);
                }
            }
        }

        if (_tCPTestClientGUI.connect)
        {
            if (nowMode != oldMode)
            {
                if (GetComponent<TCPTestClientGUI>().language==1)
                {
                    GetComponent<AudioSource>().clip = audioClips[nowMode];
                }
                else if (GetComponent<TCPTestClientGUI>().language == 2)
                {
                    GetComponent<AudioSource>().clip = audioClips_ENG[nowMode];
                }
                else
                {
                    GetComponent<AudioSource>().clip = audioClipsKAN[nowMode];
                }
                GetComponent<AudioSource>().Play();
                oldMode = nowMode;
            }

            if (update)
            {
                if (receive)
                {
                    logDatas.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ":" + get_msec().ToString("000") + ":" + receiveDatas[receiveDatas.Count - 1]);
                    log[receiveDatas.Count - 1] = string.Format("<color=green>{0}</color>", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ":" + get_msec().ToString("000") + ": \n") + receiveDatas[receiveDatas.Count - 1] + "\n";

                    if (receiveDatas.Count < maxLogNum)
                    {
                        receiveDataTransform.GetComponent<RectTransform>().sizeDelta = new Vector2(0, receiveDataTransform.GetComponent<RectTransform>().sizeDelta.y + 50);
                        receiveDataText.text += string.Format("<color=green>{0}</color>", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ":" + get_msec().ToString("000") + ": ");
                        receiveDataText.text += receiveDatas[receiveDatas.Count - 1] + "\n";
                       
                        receive = false;
                    }
                    else
                    {
                        receiveDataText.text = "";
                        for (int i = receiveDatas.Count - maxLogNum; i < receiveDatas.Count; i++)
                        {
                            receiveDataText.text += log[i];
                        }
                        receive = false;
                    }
                    receive = false;
                }
            }

        
            //SYSSTAT
            scenarioCheck_Int[0] = DataUpdate(tableData_501[0][3]);
            //SYSSTATVOICE
            scenarioCheck_Int[1] = DataUpdate(tableData_501[1][3]);
            //ACTMODEPL 
            scenarioCheck_Int[2] = DataUpdate(tableData_501[2][3]);
            //XACTMODEPLVOICE
            scenarioCheck_Int[3] = DataUpdate(tableData_501[3][3]);
            //ACTMODEEXE
            scenarioCheck_Int[4] = DataUpdate(tableData_501[4][3]);
            //XACTMODEEXEVOICE
            scenarioCheck_Int[5] = DataUpdate(tableData_501[5][3]);
            //XCROSSROAD
            scenarioCheck_Int[6] = DataUpdate(tableData_501[18][3]);
            //SIGNlALINFO
            scenarioCheck_Int[7] = DataUpdate(tableData_501[8][3]);
            //DISTSTOP
            scenarioCheck_Int[9] = DataUpdate(tableData_502[1][3]);
            //DISTBRANCH
            scenarioCheck_Int[8] = DataUpdate(tableData_502[2][3]);
            //XFORWARDOBJ 
            scenarioCheck_Int[10] = DataUpdate(tableData_501[9][3]);
            //FORWARDOBJTYPE
            scenarioCheck_Int[11] = DataUpdate(tableData_501[10][3]);
            //XREADINESSCAUTION
            scenarioCheck_Int[12] = DataUpdate(tableData_502[3][3]);
            //XREADINESSWARNNING
            scenarioCheck_Int[13] = DataUpdate(tableData_502[4][3]);
            //XDISPTOR
            scenarioCheck_Int[14] = DataUpdate(tableData_502[5][3]);
            //XDRIVESTRHOLD
            scenarioCheck_Int[15] = DataUpdate(tableData_504[14][3]);
            //AUTOSYSFAIL
            scenarioCheck_Int[16] = DataUpdate(tableData_501[23][3]);
            //AUTOSTOP
            scenarioCheck_Int[17] = DataUpdate(tableData_501[24][3]);
            //XDRIVERFAILWARNNING 
            scenarioCheck_Int[18] = DataUpdate(tableData_502[6][3]);
            //XSLEEPWARNNING
            scenarioCheck_Int[19] = DataUpdate(tableData_502[7][3]);
            //XINATTENTIVEWARNNING
            scenarioCheck_Int[20] = DataUpdate(tableData_502[8][3]);
            //AUTOFAIL
            scenarioCheck_Int[21] = DataUpdate(tableData_501[22][3]);
            //AUTOSTOPCODE
            scenarioCheck_Int[22] = DataUpdate(tableData_501[25][3]);
            //XNEARGOAL 追記：2020.01.06
            scenarioCheck_Int[23] = DataUpdate(tableData_501[17][3]);
            //SPPCS(DSM制御用車速) 追記：2020.01.06
            scenarioCheck_Int[24] = DataUpdate(tableData_504[0][3]);
            //XCROSSWALK 追記：2020.01.07
            scenarioCheck_Int[25] = DataUpdate(tableData_501[19][3]);


            if (scenarioCheck_Int[18] == 1)
            {
                allertTime += Time.deltaTime;
            }
            else
            {
                allertTime = 0;
            }

            CheckMode();
            ArrowCheck();
        }


        //scenarioCheck[8] = tableData_501[19][3];
    }

    //アプリケーションが終了した際の処理
    //ファイル書き込みの処理をしている
    private void OnApplicationQuit()
    {
        //string filePath = Application.dataPath + @"\Scripts\File\WriteText1.txt";
        string filePath = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_") + "LogData.txt";
        StreamWriter streamWriter = new StreamWriter(filePath, append: true);
        for (int i = 0; i < logDatas.Count; i++)
        {
            //Debug.Log(receiveDatas[i]);
            streamWriter.WriteLine(logDatas[i]);
            if (i == logDatas.Count - 1)
            {
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

    }

    //表示用のオブジェクトを作成
    public void InstantiateText(List<string[]> datas,GameObject[] dataContent,Transform parent)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            var obj = Instantiate(prefabObj);
            dataContent[i] = obj;
            obj.transform.SetParent(parent);
        }
    }
    //データベースの値（引数）を表示させる
    public void UpdateData(List<string[]> datas,GameObject[] dataContent)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            dataContent[i].GetComponentsInChildren<Text>()[0].text = datas[i][0] + " : " + datas[i][3];
        }
    }

    //TCP/IPのデータを取得した際の処理
    public void ReceiveData()
    {
        receive = true;
    }

    //表示リストの更新(デバッグ用)
    public void ViewList(int num)
    {

        //switchObj501.SetActive(false);
        //switchObj502.SetActive(false);
        //switchObj503.SetActive(false);
        //switchObj504.SetActive(false);
        //switchObjReceiveData.SetActive(false);

        switch (num)
        {
            case 1:
                switchObj501.SetActive(true);
                break;

            case 2:
                switchObj502.SetActive(true);
                break;

            case 3:
                switchObj503.SetActive(true);
                break;

            case 4:
                switchObj504.SetActive(true);
                break;

            case 5:
                switchObjReceiveData.SetActive(true);
                break;
        }
    }

    //ミリ秒の取得（データスタンプ用）
    long get_msec()
    {
        System.DateTime now = System.DateTime.Now;
        System.DateTime nowMsec0 = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        long TotalMsec = (long)((DateTime.Now - nowMsec0).TotalMilliseconds);

        return TotalMsec;
    }

    //どのイベントに対応しているかのチェック（音声関係）--------------------------------------------------------------------------------音声----------------------------------------
    public void CheckMode()
    {
        //自動運転システム　起動中
        if (scenarioCheck_Int[0] == 0 && scenarioCheck_Int[1] == 1)
        {
            nowMode = 0;
        }

        //自動運転　開始できません
        if (scenarioCheck_Int[0] == 1 && scenarioCheck_Int[1] == 1)
        {
            nowMode = 1;
        }

        //自動運転　開始できます　起動スイッチを押してください
        if (scenarioCheck_Int[0] == 2 && scenarioCheck_Int[1] == 1)
        {
            nowMode = 2;
        }

        //自動運転　開始しました
        if (oldMode == 2 && scenarioCheck_Int[0] == 3 && scenarioCheck_Int[1] == 1)
        {
            nowMode = 3;
        }

        //車線維持　定常走行中
        if (scenarioCheck_Int[0] == 3 && 
            (scenarioCheck_Int[2] == 1 || scenarioCheck_Int[2] == 2 || scenarioCheck_Int[2] == 3 || scenarioCheck_Int[2] == 4 || 
            scenarioCheck_Int[2] == 5 || scenarioCheck_Int[2] == 15) &&
            scenarioCheck_Int[23] ==0 && scenarioCheck_Int[8] == 0 && scenarioCheck_Int[9] ==0 && scenarioCheck_Int[3] == 1
         )
        {
            nowMode = 4;
        }

        //青信号　通過します
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[3] == 1 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[7] == 1 && (scenarioCheck_Int[9] == 1 || scenarioCheck_Int[9] == 2 || scenarioCheck_Int[9] == 3) &&
            (scenarioCheck_Int[2] == 1 || scenarioCheck_Int[2] == 2 || scenarioCheck_Int[2] == 3 || scenarioCheck_Int[2] == 4 ||
            scenarioCheck_Int[2] == 5 || scenarioCheck_Int[2] == 15))
        {
            nowMode = 5;
        }

        //青信号　交差点右折します
        //制御廃止
        //if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[3] == 1 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[7] == 1 && scenarioCheck_Int[2] == 9 && (scenarioCheck_Int[9] == 1 || scenarioCheck_Int[9] == 2 || scenarioCheck_Int[9] == 3))
        //{
        //    nowMode = 6;
        //}

        //青信号　右折中
        //制御廃止
        //if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 9 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[7] == 1 && scenarioCheck_Int[5] == 1)
        //{
        //    nowMode = 7;
        //}

        //青信号　交差点左折します
        //制御廃止
        //if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[3] == 1 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[7] == 1 && scenarioCheck_Int[2] == 10 && (scenarioCheck_Int[9] == 1 || scenarioCheck_Int[9] == 2 || scenarioCheck_Int[8] == 3))
        //{
        //    nowMode = 8;
        //}

        //青信号　左折中
        //制御廃止
        //if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 10 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[7] == 1 && scenarioCheck_Int[5] == 1)
        //{
        //    nowMode = 9;
        //}

        ///使用予定なし
        ///矢印信号　直進します　nowMode=10
        ///矢印信号　右折します　nowMode=11
        ///矢印信号　左折します　nowMode=12
        ///

        //赤信号　減速停車します-
        if (scenarioCheck_Int[0] == 3 && (scenarioCheck_Int[2] == 7 || scenarioCheck_Int[2] == 2) && (scenarioCheck_Int[7] == 2 || scenarioCheck_Int[7] == 3) && scenarioCheck_Int[3] == 1 && (scenarioCheck_Int[9] == 1 || scenarioCheck_Int[9] == 2 || scenarioCheck_Int[9] == 3))
        {
            nowMode = 13;
        }

        //信号待ち　停車中-
        if (scenarioCheck_Int[0] == 3 && (scenarioCheck_Int[2] == 7 || scenarioCheck_Int[2] == 2 || scenarioCheck_Int[2] == 9 || scenarioCheck_Int[2] == 10) && scenarioCheck_Int[9] == 3 && scenarioCheck_Int[7] == 2 && isGridStop && scenarioCheck_Int[3] == 1)
        {
            nowMode = 14;
        }

        //青信号　発進します-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[2] == 8 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[9] == 3 && scenarioCheck_Int[7] == 1 && scenarioCheck_Int[3] == 1)
        {
            nowMode = 15;
        }

        //交差点右折します-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[2] == 9 && (scenarioCheck_Int[9] == 1 || scenarioCheck_Int[9] == 2 || scenarioCheck_Int[9] == 3) && scenarioCheck_Int[3] == 1)
        {
            nowMode = 16;
        }

        //交差点左折します-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[2] == 10 && (scenarioCheck_Int[9] == 1 || scenarioCheck_Int[9] == 2 || scenarioCheck_Int[9] == 3) && scenarioCheck_Int[3] == 1)
        {
            nowMode = 17;
        }

        //右折中-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 9 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[5] == 1)
        {
            nowMode = 18;
        }

        //左折中-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 10 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[5] == 1)
        {
            nowMode = 19;
        }

        //分岐を右に進行します-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[2] == 16 && scenarioCheck_Int[3] == 1 && (scenarioCheck_Int[8] == 1 || scenarioCheck_Int[8] == 2 || scenarioCheck_Int[8] == 3))
        {
            nowMode = 20;
        }

        //分岐を右に進行中
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 16 && scenarioCheck_Int[5] == 1)
        {
            nowMode = 21;
        }

        //分岐を左に進行します-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[2] == 17 && scenarioCheck_Int[3] == 1 && (scenarioCheck_Int[8] == 1 || scenarioCheck_Int[8] == 2 || scenarioCheck_Int[8] == 3))
        {
            nowMode = 22;
        }

        //分岐を左に進行中-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 17 && scenarioCheck_Int[5] == 1)
        {
            nowMode = 23;
        }

        ///使用予定なし
        ///この先左折レーン　右へ　車線変更します　nowMode=24
        ///この先バス優先レーン　右へ　車線変更します　nowMode=25
        ///

        //右へ車線変更します-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[2] == 11 && scenarioCheck_Int[3] == 1)
        {
            nowMode = 26;
        }
        
        //右へ車線変更中-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 11 && scenarioCheck_Int[5] == 1)
        {
            nowMode = 27;
        }

        ///使用予定なし
        ///右へ　合流します　nowMode=28
        ///

        //左へ車線変更します-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[2] == 13 && scenarioCheck_Int[3] == 1)
        {
            nowMode = 29;
        }

        //左へ車線変更中-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 13 && scenarioCheck_Int[5] == 1)
        {
            nowMode = 30;
        }

        ///使用予定なし
        ///左へ　合流します　nowMode=31
        ///

        //障害物有り　車線変更中断します-
        if (scenarioCheck_Int[0] == 3 && (scenarioCheck_Int[2] == 12 || scenarioCheck_Int[2] == 14) && scenarioCheck_Int[3] == 1)
        {
            nowMode = 32;
        }

        ///使用予定なし
        ///歩行者なし　横断歩道通過します　nowMode=33
        ///

        //歩行者有り　減速停車します-
        if (scenarioCheck_Int[0] == 3 && (scenarioCheck_Int[2] == 9 || scenarioCheck_Int[2] == 10 || scenarioCheck_Int[4] == 9 || scenarioCheck_Int[4] == 10) && scenarioCheck_Int[25] == 1 && scenarioCheck_Int[10] == 1 && scenarioCheck_Int[11] == 2 && scenarioCheck_Int[3] == 1)
        {
            nowMode = 34;
        }

        //発信します-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[2] == 8 && scenarioCheck_Int[25] == 1 && scenarioCheck_Int[10] == 0 && scenarioCheck_Int[7] == 0 && scenarioCheck_Int[3] == 1)
        {
            nowMode = 35;
        }

        ///使用予定なし
        ///安全監視に　集中してください　nowMode=36
        ///

        //運転復帰可能状態を　保ってください-
        ////警報
        if (scenarioCheck_Int[0] == 3 && (scenarioCheck_Int[13] == 1))
        {
            nowMode = 37;
        }
        ////注意表示フラグ テキストのみ
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[12] == 1)
        {
            nowMode = 51;
        }

        //自動運転解除します　手動運転してください-
        if (scenarioCheck_Int[0] == 3 && (scenarioCheck_Int[14] == 1))
        {
            nowMode = 38;
        }

        ///使用予定なし
        ///中継地点到着　減速します　nowMode=39
        /// 自動運転解除　ブレーキを踏んでください　nowMode=40
        ///

        //目的地到着　減速します-
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[23] == 1 && scenarioCheck_Int[1] == 1)
        {
            nowMode = 41;
        }

        ///使用予定なし
        ///お疲れ様でした　自動運転解除　ブレーキを踏んでください　nowMode=42
        ///

        //運転操作検出　自動運転解除　手動で運転してください-
        if (scenarioCheck_Int[0] == 5 && scenarioCheck_Int[15] == 0 && scenarioCheck_Int[1] == 1)
        {
            nowMode = 43;
        }


        //システム故障　手動で運転してください-
        if (scenarioCheck_Int[0] == 4 && scenarioCheck_Int[16] == 1 && scenarioCheck_Int[1] == 1)
        {
            nowMode = 44;
        }

        if (scenarioCheck_Int[0] == 4 && scenarioCheck_Int[21] == 1 && scenarioCheck_Int[1] == 1)
        {
            nowMode = 52;
        }

        //システム中断　手動で運転してください-
        if (scenarioCheck_Int[0] == 4 && scenarioCheck_Int[17] == 1 && scenarioCheck_Int[1] == 1)
        {
            nowMode = 45;
        }


        //ドライバーの異常を検知しました
        if ((scenarioCheck_Int[0] != 3 || scenarioCheck_Int[0] != 4) && scenarioCheck_Int[18] == 1)
        {
            nowMode = 46;
        }


        //ドライバーの異常を検知　減速停車します
        if ((scenarioCheck_Int[0] != 3 || scenarioCheck_Int[0] != 4) && scenarioCheck_Int[18] == 1 && allertTime >= 4)
        {
            nowMode = 47;
        }


        ///使用予定なし
        ///ドライバーの異常を検知　停車中　nowMode=48
        ///


        //眠気を検知しました(XSLEEPWARNNINGの条件が入っていない)
        if ((scenarioCheck_Int[0] != 3 || scenarioCheck_Int[0] != 4) && scenarioCheck_Int[19] == 1)
        {
            nowMode = 49;
        }


        //安全監視に　集中してください(XINATTENTIVEWARNNINGの条件が入っていない)
        if ((scenarioCheck_Int[0] != 3 || scenarioCheck_Int[0] != 4) && scenarioCheck_Int[20] == 1)
        {
            nowMode = 50;
        }
    }

    //どの矢印に対応しているかのチェック（矢印の確認）-------------------------------------------------------------------------矢印-----------------------------------------------
    public void ArrowCheck()
    {

        //交差点計画経路 交差点右折
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[2] == 9 && (scenarioCheck_Int[9] == 1 || scenarioCheck_Int[9] == 2 || scenarioCheck_Int[9] == 3))
        {
            arrowNum = 0;
        }
        
        //交差点計画経路 交差点左折
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[2] == 10 && (scenarioCheck_Int[9] == 1 || scenarioCheck_Int[9] == 2 || scenarioCheck_Int[9] == 3))
        {
            arrowNum = 1;
        }
        
        //交差点計画経路 交差点直進
        if(scenarioCheck_Int[0] == 3 && scenarioCheck_Int[6] == 1 && 
            (scenarioCheck_Int[2] == 1 || scenarioCheck_Int[2] == 2 || scenarioCheck_Int[2] == 3 || scenarioCheck_Int[2] == 4 || scenarioCheck_Int[2] == 5 || 
            scenarioCheck_Int[2] == 12 || scenarioCheck_Int[2] == 14 || scenarioCheck_Int[2] == 15) &&
            (scenarioCheck_Int[7] == 0 || scenarioCheck_Int[7] == 1) && 
            (scenarioCheck_Int[9] == 1 || scenarioCheck_Int[9] == 2 || scenarioCheck_Int[9] == 3)
        )
        {
            arrowNum = 2;
        }

        //分岐経路計画 右分岐
        if(scenarioCheck_Int[0] == 3 && scenarioCheck_Int[2] == 16 && (scenarioCheck_Int[8] == 1 || scenarioCheck_Int[8] == 2 || scenarioCheck_Int[8] == 3))
        {    
            arrowNum = 3;
        }
        
        //分岐経路計画 左分岐
        if(scenarioCheck_Int[0] == 3  && scenarioCheck_Int[2] == 17 && (scenarioCheck_Int[8] == 1 || scenarioCheck_Int[8] == 2 || scenarioCheck_Int[8] == 3))
        {
            arrowNum = 4;
        }
        
        //単路経路計画 直進
        if (scenarioCheck_Int[0] == 3 && 
            (scenarioCheck_Int[2] == 1 || scenarioCheck_Int[2] == 2 || scenarioCheck_Int[2] == 3 || scenarioCheck_Int[2] == 4 || scenarioCheck_Int[2] == 5 || 
            scenarioCheck_Int[2] == 12 || scenarioCheck_Int[2] == 14 || scenarioCheck_Int[2] == 15) &&
            scenarioCheck_Int[23] == 0 &&
            scenarioCheck_Int[9] == 0 && scenarioCheck_Int[8] == 0
        )
        {
            arrowNum = 5;
        }
        
        //単路経路計画 右レーンチェンジ
        if (scenarioCheck_Int[0] == 3 && scenarioCheck_Int[2] == 11)
        {
            arrowNum = 6;
        }
        
        //単路経路計画 左レーンチェンジ
        if(scenarioCheck_Int[0] == 3 && scenarioCheck_Int[2] == 13)
        {
            arrowNum = 7;
        }
        
        //交差点制御実行中 交差点右折（実行）
        if(scenarioCheck_Int[0] == 3 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[4] == 9)
        {
            arrowNum = 8;
        }
        
        //交差点制御実行中 交差点左折（実行）
        if(scenarioCheck_Int[0] == 3 && scenarioCheck_Int[6] == 1 && scenarioCheck_Int[4] == 10)
        {
            arrowNum = 9;
        }
        
        //交差点制御実行中 交差点直進
        if(scenarioCheck_Int[0] == 3 && 
            (scenarioCheck_Int[2] == 1 || scenarioCheck_Int[2] == 2 || scenarioCheck_Int[2] == 3 || scenarioCheck_Int[2] == 4 || scenarioCheck_Int[2] == 5 || 
            scenarioCheck_Int[2] == 12 || scenarioCheck_Int[2] == 14 || scenarioCheck_Int[2] == 15) &&
            scenarioCheck_Int[23] == 0 &&
            scenarioCheck_Int[9] == 0 && scenarioCheck_Int[8] == 0
        )
        {
            arrowNum = 5;
        }
        
        //分岐制御実行中 右分岐（実行）
        if(scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 16)
        {
            
            arrowNum = 10;
        }
        
        //分岐制御実行中 左分岐（実行）
        if(scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 17)
        {
            
            arrowNum = 11;
        }
        
        //単路制御実行中 右レーンチェンジ（実行）
        if(scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 11)
        {
            arrowNum = 12;
        }
        
        //単路制御実行中 左レーンチェンジ（実行）
        if(scenarioCheck_Int[0] == 3 && scenarioCheck_Int[4] == 13)
        {
            arrowNum = 13;
        }
        
        //単路制御実行中 直進
        if(scenarioCheck_Int[0] == 3 &&
           (scenarioCheck_Int[2] == 1 || scenarioCheck_Int[2] == 2 || scenarioCheck_Int[2] == 3 || scenarioCheck_Int[2] == 4 || scenarioCheck_Int[2] == 5 ||
            scenarioCheck_Int[2] == 12 || scenarioCheck_Int[2] == 14 || scenarioCheck_Int[2] == 15) &&
            scenarioCheck_Int[23] == 0 &&
            scenarioCheck_Int[9] == 0 && scenarioCheck_Int[8] == 0
        )
        {
            arrowNum = 5;
        }

        //SYSSTAT がRUNNINGではないとき
        if (scenarioCheck_Int[0] != 3)
        {
            arrowNum = 15;
        }
    }

    //データの更新＆中身があるかの確認
    public int DataUpdate(string stringData)
    {
        if(stringData != "")
        {
            return Int32.Parse(stringData);
        }
        else
        {
            return 0;
        }
    }


    void CheckSpeedForDsmControl()
    {
        //リストに追加
        speedForDsmControl.Add(scenarioCheck_Int[24] * multiCoefficient);

        //指定数以上になったら古いのを削除
        if(speedForDsmControl.Count > 60 * duration){
            speedForDsmControl.RemoveAt(0);
        }

        //リストの各要素を加算
        var speedForDsmControlSum = speedForDsmControl.Sum();
        //Debug.LogWarning("リストの要素数：" + speedForDsmControl.Count + " & リストの合計：" + speedForDsmControlSum + " & 合計" + speedForDsmControlSum + " & 基準" + stopCriteria * speedForDsmControl.Count);

        //時速xキロがx秒続いたか判断
        if (speedForDsmControlSum >= stopCriteria * speedForDsmControl.Count)
        {
            //Debug.LogWarning("グリッド停止");
            isGridStop = true;
        }

        //Debug.LogWarning("isGridStop : " + isGridStop);

    }

    float FpsCount()
    {
        float fps = 1.0f / Time.deltaTime;

        return fps;
    }
   
}
