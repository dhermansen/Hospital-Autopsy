using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

struct ToolItems
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
	// Use this for initialization
	void Start () {
        var inst = GameObject.Find("Instruments");
        tools = inst.GetComponentsInChildren<Transform>().ToList<Transform>()
            .Where(tx => tx.parent == inst.transform)
            .Select(t => new ToolItems(t.gameObject, t.position, t.rotation)).ToList<ToolItems>();
        foreach (var item in tools)
        {
            Debug.Log("Item " + item.obj.name);
            Debug.Log(item.pos.ToString());
        }
      }

        // Update is called once per frame
        void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        var dropped_tools = tools.Where(tool => tool.obj == other.transform.parent.gameObject);
        if (dropped_tools.Count() > 0)
        {
            var dropped = dropped_tools.First();
            Debug.Log("Trigger entered " + dropped.obj.name);
            Debug.Log(dropped.pos.ToString());
            Debug.Log(dropped.obj.transform.position.ToString());
            //dropped.obj.
            dropped.obj.transform.position = dropped.pos;
            dropped.obj.transform.rotation = dropped.rot;
            var rb = dropped.obj.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        //else
        //{
        //    Debug.Log(other.transform.parent.name);
        //}
    }
}
