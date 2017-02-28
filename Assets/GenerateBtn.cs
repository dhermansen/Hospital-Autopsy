using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateBtn : MonoBehaviour
{
    private bool mode, log;
    void Start()
    {
        var togNames = new List<string> { "Mode_Btn", "Log_Btn" };
        foreach (var name in togNames)
        {
            GameObject toggleObj = GameObject.Find(name);
            Toggle toggle = toggleObj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((x) => Invoke("MyFunction", 1f));
        }
    }
    private void MyFunction()
    {
        Debug.Log("Foo");
    }
}