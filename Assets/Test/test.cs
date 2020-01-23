using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class test : MonoBehaviour
{
    int count;
    // Start is called before the first frame update
    void Start()
    {
        int[] a = new int[3] { 5, 2, 3};
        int[] b = new int[3] { 5, 2, 3 };

        //Debug.Log(a.SequenceEqual(b));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            System.DateTime now = System.DateTime.Now;
           
            count++;
            //ScreenCapture.CaptureScreenshot("_text_" + count + ".png");
            ScreenCapture.CaptureScreenshot(now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute + "_text_" + count + ".png");
            //Debug.Log(now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute + "_text_" + count + ".png");
        }
    }
}
