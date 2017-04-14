using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct ToolItems
{
    public GameObject obj;
    public Vector3 pos;
    public Quaternion rot;

    public ToolItems(GameObject gameObject, Vector3 position, Quaternion rotation) : this()
    {
        this.obj = gameObject;
        this.pos = position;
        this.rot = rotation;
    }
}
public class OnDroppedTool : MonoBehaviour {
    private List<ToolItems> tools;
    GameObject cl, op;
	// Use this for initialization
	void Start ()
    {
        var inst = GameObject.Find("Instruments");
        tools = inst.GetComponentsInChildren<Transform>().ToList<Transform>()
            .Where(tx => tx.parent == inst.transform)
            .Select(t => new ToolItems(t.gameObject, t.position, t.rotation)).ToList<ToolItems>();
        cl = GameObject.Find("Scissors/Closed");
        op = GameObject.Find("Scissors/Open");
        op.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
		
	}
    private IEnumerator reset_tool(ToolItems tool)
    {
        yield return new WaitForSeconds(1.0f);

        tool.obj.transform.position = tool.pos;
        tool.obj.transform.rotation = tool.rot;
        var rb = tool.obj.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.parent)
            return;
        var dropped_tools = tools.Where(tool => tool.obj == other.transform.parent.gameObject);

        if (dropped_tools.Count() > 0)
        {
            StartCoroutine(reset_tool(dropped_tools.First()));
        }
        else
        {
            var dropped_scissors = tools.Where(
                tool => other.transform.parent && tool.obj == other.transform.parent.parent.gameObject);
            if (dropped_scissors.Count() > 0)
            {
                cl.SetActive(true);
                op.SetActive(false);
                StartCoroutine(reset_tool(dropped_scissors.First()));
            }
        }
    }
}
