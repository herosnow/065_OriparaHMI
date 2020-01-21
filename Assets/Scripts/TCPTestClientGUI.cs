using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TCPTestClientGUI : MonoBehaviour
{
    private List<TCPTestClient> clients = new List<TCPTestClient>();
    private TCPTestServer _server;
    private TCPTestClient _client;

    public InputField IPInputField;
    public InputField PortInputField;
    public InputField MessageInputField;
    public Text TextWindow;
    private string text;

    public bool resever;
    public string[][] strings = new string[50][];
    public InputField[] sendInputFields;
    private float timer;


    private object cacheLock = new object();
    private string cache;

    public GameObject uiCanvas;

    public ConvertTest convertTest;
    public DataBase dataBase;

    List<string[]> _dataBase501;
    List<string[]> _dataBase502;
    List<string[]> _dataBase503;
    List<string[]> _dataBase504;

    public string sendMessage;

    private float readnesUpdateTime;

    private string saveMessage;
    private int messageCount;

    //デバッグ用
    float startTime;

    public bool debug = true;
    public bool connect;
    private int count;

    public string testCan501;
    public string testCan502;
    public string testCan503;
    public string testCan504;
    //

    float reseiveTime;
    public GameObject errorImage;

    private void Awake()
    {
        _server = GetComponent<TCPTestServer>();
        _server.OnLog += OnServerReceivedMessage;
        _client = GetComponent<TCPTestClient>();
        _client.OnConnected += OnClientConnected;
        _client.OnDisconnected += OnClientDisconnected;
        _client.OnMessageReceived += OnClientReceivedMessage;
        _client.OnLog += OnClientLog;

        ConnectClient();
    }

    private void Update()
    {
        readnesUpdateTime += Time.deltaTime;
        reseiveTime += Time.deltaTime;

        if (reseiveTime > 1f)
        {
            Debug.Log("消える");
            errorImage.SetActive(true);
        }
        else
        {
            Debug.Log("消えない");
            errorImage.SetActive(false);
            //count = 0;
        }

        lock (cacheLock)
        {
            if (!string.IsNullOrEmpty(cache))
            {
                TextWindow.text += string.Format("{0}", cache);
                cache = null;
            }
        }

        //チェックサムテスト用
        //OnClientReceivedMessage("M 1 CSD 501 " + testCan501 + "\n" + "M 1 CSD 502 " + testCan502 + "\n" + "M 1 CSD 503 " + testCan503 + "\n" + "M 1 CSD 504 " + testCan504 + "\n");

        //OnClientReceivedMessage("M 1 CSD 501 20 00 04 80 40 00 00 F2\n" +
        //    "M 1 CSD 502 28 00 00 00 00 00 00 37\n" +
        //    "M 1 CSD 503 04 4E 00 00 00 00 08 6A\n" +
        //    "M 1 CSD 504 00 00 00 59 00 08 00 72\n");

        if (resever)
        {
            timer += Time.deltaTime;
            //if (timer > 1)
            {
                _dataBase501 = dataBase.tableData_501;
                _dataBase502 = dataBase.tableData_502;
                _dataBase503 = dataBase.tableData_503;
                _dataBase504 = dataBase.tableData_504;

                if (_client.IsConnected)
                {
                    ChangeImage();
                    if (readnesUpdateTime > 1f)
                    {
                        ReadnessUpdate();
                        readnesUpdateTime = 0f;
                    }
                }
                timer = 0;
            }
        }    
       

        if (_client.IsConnected)
        {
            startTime += Time.deltaTime;
            if (startTime > 1f)
            {
                startTime = 0;
                sendMessage = "M 1 CSD 505 00 00 00 00 00 00 00 12";
                //SendMessageToServer_sting();
            }
        }
        connect = _client.IsConnected;
    }

    public void StartServer()
    {
        if (!_server.IsConnected)
        {
            _server.IPAddress = IPInputField.text;
            int.TryParse(PortInputField.text, out _server.Port);
            _server.StartServer();
        }
    }

    public void ConnectClient()
    {
        if (!_client.IsConnected)
        {        
            _client.IPAddress = IPInputField.text;
            int.TryParse(PortInputField.text, out _client.Port);
            _client.ConnectToTcpServer();

            StartCoroutine("DelayMethod");
        }
    }

    private IEnumerator DelayMethod()
    {
        Debug.Log("送信");
        yield return new WaitForSeconds(0.3f);
        MessageInputField.text = "CAN 1 INIT STD 500";
        SendMessageToServer();
        yield return new WaitForSeconds(0.3f);
        MessageInputField.text = "CAN 1 FILTER CLEAR";
        SendMessageToServer();
        yield return new WaitForSeconds(0.3f);
        MessageInputField.text = "CAN 1 FILTER ADD STD 0 0";
        SendMessageToServer();
        yield return new WaitForSeconds(0.3f);
        MessageInputField.text = "CAN 1 START";
        SendMessageToServer();
    }

    public void DisconnectClient()
    {
        if (_client.IsConnected)
        {
            MessageInputField.text = "CAN 1 STOP";
            SendMessageToServer();

            _client.CloseConnection();
        }
    }

    private void OnApplicationQuit()
    {
        if (_client.IsConnected)
        {
            MessageInputField.text = "CAN 1 STOP";
            SendMessageToServer();

            _client.CloseConnection();
        }
    }


    //メッセージを送る関数 ボタンのイベント
    public void SendMessageToServer()
    {
        if (_client.IsConnected)
        {
            string message = MessageInputField.text;
            //for (int i=0;i<sendInputFields.Length;i++)
            //{
            //    message += sendInputFields[i].text + " ";
            //}
            //if (message.StartsWith("!ping"))
            //{
            //    message += " " + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            //}
            
            if (!string.IsNullOrEmpty(message))
            {
                if (_client.SendMessage(message))
                {
                    MessageInputField.text = string.Empty;
                }
            }
        }
    }

    //メッセージを送る関数
    public void SendMessageToServer_sting()
    {
        if (_client.IsConnected)
        {
            //for (int i=0;i<sendInputFields.Length;i++)
            //{
            //    message += sendInputFields[i].text + " ";
            //}
            //if (message.StartsWith("!ping"))
            //{
            //    message += " " + (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            //}

                if (_client.SendMessage(sendMessage))
                {
                    MessageInputField.text = string.Empty;
                }
        }
    }

    //追記 2020.01.06
    string previousMessage = "";

    private void OnClientReceivedMessage(string message)
    {
        string finalMessage = ProcessServerMessage(message);

        //lock (cacheLock)
        //{
        //    if (string.IsNullOrEmpty(cache))
        //    {
        //        cache = string.Format("<color=green>{0}</color>\n", finalMessage);
        //    }
        //    else
        //    {
        //        cache += string.Format("<color=green>{0}</color>\n", finalMessage);
        //    }
        //}
    }
    
    private void OnClientLog(string message)
    {
        lock (cacheLock)
        {
            if (string.IsNullOrEmpty(cache))
            {
                cache = string.Format("<color=grey>{0}</color>\n", message);
            }
            else
            {
                cache += string.Format("<color=grey>{0}</color>\n", message);
            }
        }
    }

    private void OnServerReceivedMessage(string message)
    {
        lock (cacheLock)
        {
            if (string.IsNullOrEmpty(cache))
            {
                cache = string.Format("<color=red>{0}</color>\n", message);
            }
            else
            {
                cache += string.Format("<color=red>{0}</color>\n", message);
            }
        }
    }

    //TCP/IPでデータを受け取った時の処理
    private string ProcessServerMessage(string message)
    {

        reseiveTime = 0;
        //Debug.Log(message == saveMessage);
        //string data = message.Data;
        //if (message.Contains("User1"))
        {
            //dataBase.ReceiveData();
            ////var m = message.Substring(44);
            //dataBase.receiveDatas.Add(message);
        }


        string[] kaigyou = new string[10];

        if (debug)
        {
            //message = message.Substring(41);
            kaigyou = message.Split('|');
        }
        else
        {
            kaigyou = message.Split('\n');
        }

        Debug.Log(kaigyou[0]);
        if (kaigyou[0].IndexOf("4") != 59 && kaigyou[0].IndexOf("4") != 18)
        {
            if (message == saveMessage)
            {
                if (messageCount < 1)
                {
                    messageCount++;
                    return "";
                }
            }
            else
            {
                saveMessage = message;
                messageCount = 0;
                return "";
            }
        }

        string[] astr_ID = new string[kaigyou.Length];
        string[][] astr_Data = new string[kaigyou.Length][];

        for (int i = 0; i < kaigyou.Length; i++)
        {
            astr_Data[i] = new string[8];
        }

        for (int i = 0; i < kaigyou.Length; i++)
        {
            strings[i] = kaigyou[i].Split(' ');
            if (strings[i].Length < 12) { break; }
            if (strings[i].Length > 3)
            {
                astr_ID[i] = strings[i][3];
                for (int j = 0; j < 8; j++)
                {
                    astr_Data[i][j] = strings[i][4 + j];
                }
            }
        }

        //各IDを判別し、格納する
        for (int j = 0; j < kaigyou.Length - 1; j++)
        {
            List<string[]> _dataBase = null;

            if (astr_ID[j]=="501")
            {
               // if (CheckSum(0x0501, 0x08, astr_Data[j]))
                {
                    _dataBase = dataBase.tableData_501;
                }
            }
            else if (astr_ID[j] == "502")
            {
               // if (CheckSum(0x0502, 0x08, astr_Data[j]))
                {
                    _dataBase = dataBase.tableData_502;
                }
            }
            else if (astr_ID[j] == "503")
            {
               // if (CheckSum(0x0503, 0x08, astr_Data[j]))
                {
                    _dataBase = dataBase.tableData_503;
                }
            }
            else if (astr_ID[j] == "504")
            {
               // if (CheckSum(0x0504, 0x08, astr_Data[j]))
                {
                    _dataBase = dataBase.tableData_504;
                }
            }

            message = "";
            if (strings != null)
            {
                if (strings[j].Length < 7) { break; }
                for (int i = 4; i < strings[j].Length; i++)
                {

                    if (strings[j][i] == "0")
                    {
                        strings[j][i] = strings[j][i].PadLeft(2, '0');
                    }
                    message += strings[j][i];
                }
            }

            if (_dataBase != null)
            {
                long v2 = Convert.ToInt64(message, 16);

                var hexStr = Convert.ToString(v2, 16);

                //2進数で直した後右詰めで0を表示する
                string a = convertTest.HexToBin(hexStr).PadLeft(64, '0');


                /////////////////////////////////////////////////
                //文字を反転する処理（必要なら）
                //char[] tst = new char[a.Length];
                //tst = stringToChar(a, tst);
                ////tst = rev_string(tst);
                //a = tst.ToString();
                //////////////////////////////////////////////////

                Debug.Log("処理");

                for (int i = 0; i < _dataBase.Count; i++)
                {
                    //Debug.Log("ID : "+ astr_ID[j]+" "+ _dataBase[j] + " " + (_dataBase== dataBase.tableData_501) + " : "+convertTest.BinToDecIntChanger(convertTest.StringCuter(a, Int32.Parse(_dataBase[i][1]), Int32.Parse(_dataBase[i][2]))).ToString());
                    _dataBase[i][3] = convertTest.BinToDecIntChanger(convertTest.StringCuter(a, Int32.Parse(_dataBase[i][1]), Int32.Parse(_dataBase[i][2]))).ToString();
                }

            }
        }
        return string.Format("{0}: {1}", message, message);
    }

    char[] stringToChar(string text, char[] ch)
    {
        for (int i = 0; i < text.Length; i++)
        {
            ch[i] = text[i];
        }
        return ch;
    }

    char[] rev_string(char[] str)
    {
        int i;
        int len = str.Length;
        for (i = 0; i < len / 2; i++)
        {
            char temp = str[i];
            str[i] = str[len - i - 1];
            str[len - i - 1] = temp;
        }

        return str;
    }

    private void OnClientConnected(TCPTestClient client)
    {
        clients.Add(client);
    }
    
    private void OnClientDisconnected(TCPTestClient client)
    {
        clients.Remove(client);
    }

    //GUI関連の変数
    [Header("ワイヤーフレームのアニメーション")]
    public Animator roadGrid;

    [Header("方向指示矢印")]
    public Image arrow;
    public Sprite[] arrowSprites;

    [Header("方向指示矢印 残り距離表示")]
    public Image distBranch;
    public Sprite[] distBranchSprites;

    [Header("DSM 注意力散漫検知")]
    public GameObject[] radar;

    public Image readniss;
    public Image readnessText;
    public Sprite[] readnissSprites;
    public Sprite readnissTextSprites_JPN;
    public Sprite readnissTextSprites_ENG;

    [Header("信号機")]
    public Image signal;
    public Sprite[] signalSprites;

    [Header("制限速度表示")]
    public Image speedLimit;
    public Sprite[] speedLimitSprites;

    [Header("車本体")]
    public Image car;

    public Sprite stplamp;
    public Sprite normalCar;

    [Header("ウィンカーアニメーション")]
    public Animator trnlAnimation;

    // public Image crossRoad;

    [Header("横断歩道検知表示")]
    public Image crossWalk;
    public Sprite[] crossWalkSprites;

    [Header("手動・自動運転 表示")]
    private bool manualDrive;
    public Image drive;
    public Sprite[] driveSprites;
    public Sprite[] driveSprites_ENG;

    [Header("HMI上のインフォ表示（テキスト）")]
    public Image info;
    public Sprite[] infoSprites;
    public Sprite[] infoSprite_ENG;
    public Sprite[] infoSprite_KAN;

    [Header("システム不具合時 表示")]
    public Image attetionImage;
    public Image codeNum_Ten;
    public Image codeNum_One;
    public Sprite[] codeNumSprites;

    [Header("言語")]
    public int language;
    public Animator languageAnimator;

    //GUIの表示の判定
    public void ChangeImage()
    {
        //ACTMODEEXE 実行中の動作(1:定速走行中、4:減速、6:一旦停止、10:交差点左折、11:右レーンチェンジ、17:左分岐)
        if (_dataBase501[0][3] == "3")
        {
                arrow.sprite = arrowSprites[dataBase.arrowNum];

            if (_dataBase501[4][3] == "9")
            {
                if (!roadGrid.GetCurrentAnimatorStateInfo(0).IsName("Wire_RightTurn"))
                {
                    roadGrid.Play("Wire_RightTurn", 0, 0);
                }
            }
            else if (_dataBase501[4][3] == "10")
            {
                if (!roadGrid.GetCurrentAnimatorStateInfo(0).IsName("Wire_LeftTurn"))
                {
                    roadGrid.Play("Wire_LeftTurn", 0, 0);
                }
            }
            else if (dataBase.isGridStop)
            {
                roadGrid.Play("New State", 0, 0);
                dataBase.isGridStop = false;
            }
            else
            {
                if (!roadGrid.GetCurrentAnimatorStateInfo(0).IsName("Wire_Drive"))
                {
                    roadGrid.Play("Wire_Drive", 0, 0);
                }
            }
        }
        else
        {
            roadGrid.Play("New State", 0, 0);
        }

        //左右分岐で使用 DISTBRANCH 分岐までの距離（0：なし、1：90ｍ先、2：60ｍ先、3；30ｍ先）
        if (arrow.sprite.name != "null")
        {
            if (dataBase.arrowNum == 3 || dataBase.arrowNum == 4)
            {
                //if (_dataBase502[2][3] == "0")
                if (_dataBase502[2][3] == "0")
                {
                    distBranch.sprite = distBranchSprites[0];
                }
                else if (_dataBase502[2][3] == "1")
                {
                    distBranch.sprite = distBranchSprites[1];
                }
                else if (_dataBase502[2][3] == "2")
                {
                    distBranch.sprite = distBranchSprites[2];
                }
                else if (_dataBase502[2][3] == "3")
                {
                    distBranch.sprite = distBranchSprites[3];
                }
                else
                {
                    distBranch.sprite = distBranchSprites[0];
                }
                
            }
            else if(dataBase.arrowNum == 10 || dataBase.arrowNum == 11) //制御実行中
            {
                distBranch.sprite = distBranchSprites[4];
            }
            else if(dataBase.arrowNum == 5 || dataBase.arrowNum == 6 || dataBase.arrowNum == 7 || dataBase.arrowNum == 12 ||dataBase.arrowNum == 13) //Nullにするよ
            {
                distBranch.sprite = distBranchSprites[0];
            }
        }
        else
        {
            distBranch.sprite = distBranchSprites[0];
        }


        //交差点の直進・右折・左折で使用 DISTSTOP 停止線までの距離（0：なし、1：90ｍ先、2：60ｍ先、3；30ｍ先）
        if (arrow.sprite.name != "null")
        {
            if (dataBase.arrowNum == 0 || dataBase.arrowNum == 1 || dataBase.arrowNum == 2)
            // dataBase.arrowNum != 8 && dataBase.arrowNum != 9 && dataBase.arrowNum != 10 && dataBase.arrowNum != 11)
            {
                //if (_dataBase502[1][3] == "0")
                
                if (_dataBase502[1][3] == "0")
                {
                    distBranch.sprite = distBranchSprites[0];
                }
                else if (_dataBase502[1][3] == "1")
                {
                    distBranch.sprite = distBranchSprites[1];
                }
                else if (_dataBase502[1][3] == "2")
                {
                    distBranch.sprite = distBranchSprites[2];
                }
                else if (_dataBase502[1][3] == "3")
                {
                    distBranch.sprite = distBranchSprites[3];
                }
                else
                {
                    distBranch.sprite = distBranchSprites[0];
                }
                               
            }
            else if(dataBase.arrowNum == 8 || dataBase.arrowNum == 9) //制御実行中
            {
                distBranch.sprite = distBranchSprites[4];
            }
            else if(dataBase.arrowNum == 5 || dataBase.arrowNum == 6 || dataBase.arrowNum == 7 || dataBase.arrowNum == 12 ||dataBase.arrowNum == 13) //Nullにするよ
            {
                distBranch.sprite = distBranchSprites[0];
            }
        }
        else
        {
            distBranch.sprite = distBranchSprites[0];
        }

        //SYSSTAT システム状態(1:SYSTEM NOT READY、2:SYSTEM READY、3：SYSTEM RUNNING、7:SYSTEM ATGOUAL)
        if (_dataBase501[0][3] == "0")
        {
            arrow.enabled = false;
            distBranch.enabled = false;
            readniss.sprite = readnissSprites[0];
            speedLimit.sprite = speedLimitSprites[0];
            manualDrive = true;
            if (language==1)
            {
                drive.sprite = driveSprites[1];
            }
            else if(language==2)
            {
                drive.sprite = driveSprites_ENG[1];
            }
            else
            {
                drive.sprite = driveSprites[1];
            }
        }
        else if (_dataBase501[0][3] == "1")
        {
            arrow.enabled = false;
            distBranch.enabled = false;
            readniss.sprite = readnissSprites[0];
            speedLimit.sprite = speedLimitSprites[0];
            manualDrive = true;
            if (language==1)
            {
                drive.sprite = driveSprites[1];
            }
            else if(language==2)
            {
                drive.sprite = driveSprites_ENG[1];
            }
            else
            {
                drive.sprite = driveSprites[1];
            }
        }
        else if (_dataBase501[0][3] == "2")
        {
            arrow.enabled = false;
            distBranch.enabled = false;
            readniss.sprite = readnissSprites[0];
            speedLimit.sprite = speedLimitSprites[0];
            if (language==1)
            {
                drive.sprite = driveSprites[1];
            }
            else if(language==2)
            {
                drive.sprite = driveSprites_ENG[1];
            }
            else
            {
                drive.sprite = driveSprites[1];
            }
        }
        else if (_dataBase501[0][3] == "3")
        {
            if (manualDrive)
            {

            }
            arrow.enabled = true;
            distBranch.enabled = true;
            readniss.enabled = true;
            speedLimit.enabled = true;
            if (language==1)
            {
                drive.sprite = driveSprites[2];
            }
            else if(language==2)
            {
                drive.sprite = driveSprites_ENG[2];
            }
            else
            {
                drive.sprite = driveSprites[2];
            }
        }
        else if (_dataBase501[0][3] == "4" || _dataBase501[0][3] == "5" || _dataBase501[0][3] == "6")
        {
            arrow.enabled = false;
            distBranch.enabled = false;
            readniss.sprite = readnissSprites[0];
            speedLimit.sprite = speedLimitSprites[0];
            manualDrive = true;
            if (_dataBase501[0][3] == "4")
            {
                if (language == 1)
                {
                    drive.sprite = driveSprites[0];
                }
                else if (language == 2)
                {
                    drive.sprite = driveSprites_ENG[0];
                }
                else
                {
                    drive.sprite = driveSprites[0];
                }
            }
            else
            {
                if (language == 1)
                {
                    drive.sprite = driveSprites[1];
                }
                else if (language == 2)
                {
                    drive.sprite = driveSprites_ENG[1];
                }
                else
                {
                    drive.sprite = driveSprites[1];
                }
            }

        }
        else if (_dataBase501[0][3] == "7")
        {
            arrow.enabled = false;
            distBranch.enabled = false;
            if (language==1)
            {
                drive.sprite = driveSprites[2];
            }
            else if(language==2)
            {
                drive.sprite = driveSprites_ENG[2];
            }
            else
            {
                drive.sprite = driveSprites[2];
            }
        }
        else
        {
            arrow.enabled = false;
            distBranch.enabled = false;
        }


        if (_dataBase501[0][3] == "3")
        {
            //TRNL 左ターンSW信号(0 or 1)
            if (_dataBase504[8][3] == "0")
            {
                if (_dataBase504[9][3] == "0")
                {
                    trnlAnimation.Play("Winker_Stop");
                }
                else if (_dataBase504[9][3] == "1")
                {
                    {
                        trnlAnimation.speed = 1;
                        trnlAnimation.Play("Winker_L");
                    }
                }
            }

            //TRNR 右ターンSW信号(0 or 1)
            if (_dataBase504[9][3] == "0")
            {
                if (_dataBase504[8][3] == "0")
                {
                    trnlAnimation.Play("Winker_Stop");
                }
                else if (_dataBase504[8][3] == "1")
                {
                    {
                        trnlAnimation.speed = 1;
                        trnlAnimation.Play("Winker_R");
                    }
                }
            }
        }

        //XCROSSROAD 前方の交差点有無(0 or 1)
        if (_dataBase501[18][3] == "0")
        {
            //crossRoad.enabled = false;
        }
        else
        {
            //crossRoad.enabled = true;
        }

        if (readniss.sprite.name == "null")
        {
            readnessText.enabled = false;
        }
        else
        {
            readnessText.enabled = true;
        }


        if (_dataBase501[0][3] == "3" || _dataBase501[0][3] == "7")
        {
            //READINESS Readniss規定値（0～100）
            if (_dataBase501[26][3] != "")
            {
                if (readnesUpdateTime > 1f)
                {
                    readniss.sprite = readnissSprites[1 + Int32.Parse(_dataBase501[26][3])];
                    if (language == 1)
                    {
                        readnessText.sprite = readnissTextSprites_JPN;
                    }
                    else if (language == 2)
                    {

                        readnessText.sprite = readnissTextSprites_ENG;
                    }
                    else
                    {
                        readnessText.sprite = readnissTextSprites_JPN;
                    }
                    readnesUpdateTime = 0f;
                }
            }

            //SPEEDLIMIT 制限速度（40,50,60）
            if (_dataBase502[0][3] == "40")
            {
                speedLimit.sprite = speedLimitSprites[1];
            }
            else if (_dataBase502[0][3] == "50")
            {
                speedLimit.sprite = speedLimitSprites[2];
            }
            else if (_dataBase502[0][3] == "60")
            {
                speedLimit.sprite = speedLimitSprites[3];
            }
            else
            {
                speedLimit.sprite = speedLimitSprites[0];
            }
            Debug.Log(_dataBase503[4][3] == "1");
            //STPLAMP ブレーキランプ（0 or 1）
            if (_dataBase503[4][3] == "1")
            {
                car.sprite = stplamp;
            }
            else
            {
                car.sprite = normalCar;
            }

            //前方に止まれ標識有無(0 or 1)
            if (_dataBase501[20][3] == "1")
            {
                signal.sprite = signalSprites[4];
            }
            else if (_dataBase501[20][3] == "0")
            {
                //SIGNALINFO 信号情報(0:信号なし、1:青、2:赤、3:黄)
                if (_dataBase501[8][3] == "0")
                {
                    signal.sprite = signalSprites[0];
                }
                else if (_dataBase501[8][3] == "1")
                {
                    signal.sprite = signalSprites[1];
                }
                else if (_dataBase501[8][3] == "2")
                {
                    signal.sprite = signalSprites[2];
                }
                else if (_dataBase501[8][3] == "3")
                {
                    signal.sprite = signalSprites[3];
                }
            }

            //前方に横断歩道標識有無(0 or 1)          
            if (_dataBase501[20][3] == "0")
            {
                if (_dataBase501[19][3] == "1")
                {
                    crossWalk.sprite = crossWalkSprites[1];
                }
                else
                {
                    crossWalk.sprite = crossWalkSprites[0];
                }
            }

            //XLEFTLANEOBJF 左前物標有無(0 or 1)
            if (_dataBase501[9][3] == "1")
            {
                radar[3].GetComponent<Animator>().Play("Radar_5");
            }
            else
            {
                radar[3].GetComponent<Animator>().Play("Default");
            }
            //XLEFTLANEOBJF 左前物標有無(0 or 1)
            if (_dataBase501[11][3] == "1")
            {
                radar[2].GetComponent<Animator>().Play("Radar_4");
            }
            else
            {
                radar[2].GetComponent<Animator>().Play("Default");
            }
            //XLEFTLANEOBJS 左横物標有無(0 or 1)
            if (_dataBase501[12][3] == "1")
            {
                radar[1].GetComponent<Animator>().Play("Radar_3");
            }
            else
            {
                radar[1].GetComponent<Animator>().Play("Default");
            }
            //XLEFTLANEOBJR 左横物標有無(0 or 1)
            if (_dataBase501[13][3] == "1")
            {
                radar[0].GetComponent<Animator>().Play("Radar_2");
            }
            else
            {
                radar[0].GetComponent<Animator>().Play("Default");
            }
            //XRIGHTLANEOBJF 左横物標有無(0 or 1)
            if (_dataBase501[14][3] == "1")
            {
                radar[4].GetComponent<Animator>().Play("Radar_6");
            }
            else
            {
                radar[4].GetComponent<Animator>().Play("Default");
            }
            //XRIGHTLANEOBJS 左横物標有無(0 or 1)
            if (_dataBase501[15][3] == "1")
            {
                radar[5].GetComponent<Animator>().Play("Radar_7");
            }
            else
            {
                radar[5].GetComponent<Animator>().Play("Default");
            }
            //XRIGHTLANEOBJR 左横物標有無(0 or 1)
            if (_dataBase501[16][3] == "1")
            {
                radar[6].GetComponent<Animator>().Play("Radar_8");
            }
            else
            {
                radar[6].GetComponent<Animator>().Play("Default");
            }
        }

        Debug.Log(dataBase.nowMode);

        //警告表示
        if (dataBase.nowMode >= 0)
        {

            if (dataBase.nowMode == 44 || dataBase.nowMode == 52)
            {
                attetionImage.enabled = true;
                if (language == 1)
                {
                    attetionImage.sprite = infoSprites[dataBase.nowMode];
                }
                else if (language == 2)
                {
                    attetionImage.sprite = infoSprite_ENG[dataBase.nowMode];
                }
                else
                {
                    attetionImage.sprite = infoSprite_KAN[dataBase.nowMode];
                }
                codeNum_One.enabled = false;
                codeNum_Ten.enabled = false;
            }
            else if (dataBase.nowMode == 45)
            {
                attetionImage.enabled = true;
                if (language == 1)
                {
                    attetionImage.sprite = infoSprites[dataBase.nowMode];
                }
                else if (language == 2)
                {
                    attetionImage.sprite = infoSprite_ENG[dataBase.nowMode];
                }
                else
                {
                    attetionImage.sprite = infoSprite_KAN[dataBase.nowMode];
                }

                codeNum_One.enabled = true;
                codeNum_Ten.enabled = true;
                codeNum_One.sprite = codeNumSprites[dataBase.scenarioCheck_Int[22] % 10];
                codeNum_Ten.sprite = codeNumSprites[dataBase.scenarioCheck_Int[22] / 10];
            }
            else
            {
                if (language == 1)
                {
                    info.sprite = infoSprites[dataBase.nowMode];
                }
                else if (language == 2)
                {
                    info.sprite = infoSprite_ENG[dataBase.nowMode];
                }
                else
                {
                    info.sprite = infoSprite_KAN[dataBase.nowMode];
                }
                codeNum_One.enabled = false;
                codeNum_Ten.enabled = false;
                attetionImage.enabled = false;
            }
            readnessText.sprite = readnissTextSprites_JPN;       
        }
    }

    //言語の変更
    //整数0：日本語　1：英語　2：関西弁
    public void ChangeLanguage(int changeNum)
    {
        language = changeNum;
        languageAnimator.SetInteger("Language", language);
        //language = (language + 1) % 3;
    }


    //チェックサム　受信
    private bool CheckSum(int id, int dlc, string[] str_Data)
    {
        int s4_check_sum = 0;

        int[] as4_data = new int[dlc];

        s4_check_sum += (id & 0xff00) >> 8;
        s4_check_sum += (id & 0x00ff) >> 0;

        s4_check_sum += dlc;

        //DLCの長さだけデータを見るかつCheckSumに加算する
        for (int s4_i = 0; s4_i < 8; s4_i++)
        {      
            //バイトデータの保存
            //16進数の文字列データより読み込む文字を切り出してParseする
            as4_data[s4_i] = int.Parse(str_Data[s4_i], System.Globalization.NumberStyles.HexNumber);
            if (s4_i < dlc - 1)
            {
                //読み込んだバイトデータを足し上げる
                s4_check_sum += as4_data[s4_i];
            }
            else
            {
                //チェックサムの値と足し上げた値を比較する
                //桁上がり分は削除する（下位1byteのみ）
                if ((s4_check_sum & 0x00ff) != as4_data[s4_i])
                {
                    //Debug.Log(false + " : チェックサムは" + (s4_check_sum & 0x00ff));
                    //エラー　チェックサムが違うため　処理
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void ReadnessUpdate()
    {
        if (_dataBase501[0][3] == "3" || _dataBase501[0][3] == "7")
        {
            //READINESS Readniss規定値（0～100）
            if (_dataBase501[26][3] != "")
            {
                readniss.sprite = readnissSprites[1 + Int32.Parse(_dataBase501[26][3])];
                if (language == 1)
                {
                    readnessText.sprite = readnissTextSprites_JPN;
                }
                else if (language == 2)
                {

                    readnessText.sprite = readnissTextSprites_ENG;
                }
                else
                {
                    readnessText.sprite = readnissTextSprites_JPN;
                }
            }
        }
    }
}
