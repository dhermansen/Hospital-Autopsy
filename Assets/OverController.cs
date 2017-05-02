using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using uFlex;

public struct MeshValues
{
    public Vector3[] shapeRestPositions;
    public Vector3[] shapeCenters;
    public Vector3[] shapeTranslations;
    public Quaternion[] shapeRotations;
    public int[] shapeIndices;
    public int[] shapeOffsets;
    public int[][] mesh_indices;
    public Vector3[] mesh_vertices;
    public int[] mesh_triangles;
    public BoneWeight[] mesh_weights;
    public Vector2[] mesh_uv;
    public Vector3[] mesh_normals;
    public Matrix4x4[] bind_poses;
    public ToolItems[] shape_transforms;
    public uFlex.Particle[] flex_particles;
    //public Mesh mesh;
}

public class OverController : MonoBehaviour {
    private GameObject renal;
    private MeshValues backups;
    private GameObject ppanel;
    private GameObject fimgpanel;
    private GameObject mimgpanel;
    bool female_pressed;

    private static MeshValues save_mesh(GameObject obj)
    {
        MeshValues mv;

        var smr = obj.GetComponent<SkinnedMeshRenderer>();
        var fsm = obj.GetComponent<FlexShapeMatching>();
        var fp = obj.GetComponent<FlexParticles>();
        smr.sharedMesh.MarkDynamic();

        mv.shapeRestPositions = fsm.m_shapeRestPositions;
        mv.shapeCenters = fsm.m_shapeCenters;
        mv.shapeTranslations = fsm.m_shapeTranslations;
        mv.shapeRotations = fsm.m_shapeRotations;
        mv.shapeIndices = fsm.m_shapeIndices;
        mv.shapeOffsets = fsm.m_shapeOffsets;
        //mv.mesh = Instantiate(smr.sharedMesh);
        mv.mesh_indices = new int[smr.sharedMesh.subMeshCount][];
        for (int i = 0; i < smr.sharedMesh.subMeshCount; i++)
            mv.mesh_indices[i] = smr.sharedMesh.GetIndices(i);
        mv.mesh_vertices = smr.sharedMesh.vertices;
        mv.mesh_triangles = smr.sharedMesh.triangles;
        mv.mesh_weights = smr.sharedMesh.boneWeights;
        mv.mesh_uv = smr.sharedMesh.uv;
        mv.mesh_normals = smr.sharedMesh.normals;
        mv.flex_particles = fp.m_particles;
        mv.bind_poses = smr.sharedMesh.bindposes;
        var txs = obj.GetComponentsInChildren<Transform>();
        mv.shape_transforms = new ToolItems[txs.Length];
        for (int i = 0; i < txs.Length; ++i)
        {
            mv.shape_transforms[i].obj = txs[i].gameObject;
            mv.shape_transforms[i].pos = txs[i].position;
            mv.shape_transforms[i].rot = txs[i].rotation;
        }
        return mv;
    }
    private static void restore_mesh(GameObject obj, MeshValues mv)
    {
        var smr = obj.GetComponent<SkinnedMeshRenderer>();
        var fsm = obj.GetComponent<FlexShapeMatching>();
        var fp = obj.GetComponent<FlexParticles>();
        fsm.m_shapeRestPositions = mv.shapeRestPositions;
        fsm.m_shapeCenters = mv.shapeCenters;
        fsm.m_shapeTranslations = mv.shapeTranslations;
        fsm.m_shapeRotations = mv.shapeRotations;
        fsm.m_shapeIndices = mv.shapeIndices;
        fsm.m_shapeOffsets = mv.shapeOffsets;
        fsm.m_shapeIndicesCount = mv.shapeIndices.Length;
        //smr.sharedMesh = Instantiate(mv.mesh);
        var mesh = Instantiate(smr.sharedMesh) as Mesh;
        smr.sharedMesh.MarkDynamic();
        smr.sharedMesh.Clear();

        smr.sharedMesh.vertices = mv.mesh_vertices;
        smr.sharedMesh.subMeshCount = mv.mesh_indices.Length;
        for (int i = 0; i < mv.mesh_indices.Length; i++)
            smr.sharedMesh.SetIndices(mv.mesh_indices[i], MeshTopology.Triangles, i);
        //smr.sharedMesh.SetTriangles(mv.mesh_triangles.ToList(), 0);
        smr.sharedMesh.boneWeights = mv.mesh_weights;
        smr.sharedMesh.SetNormals(mv.mesh_normals.ToList());
        smr.sharedMesh.SetUVs(0, mv.mesh_uv.ToList());
        smr.sharedMesh.bindposes = mv.bind_poses;
        //smr.sharedMesh = mesh;
        //smr.sharedMesh.UploadMeshData(true);
        smr.sharedMesh.UploadMeshData(false);
        fp.m_particles = mv.flex_particles;
        for (int i = 0; i < mv.shape_transforms.Length; ++i)
        {
           mv.shape_transforms[i].obj.transform.position = mv.shape_transforms[i].pos;
           mv.shape_transforms[i].obj.transform.rotation = mv.shape_transforms[i].rot;
        }
    }
    // Use this for initialization
    void Start () {
        renal = GameObject.Find("RenalSystemColor");
        backups = save_mesh(renal);
        //renal.GetComponent<Renderer>().enabled = false;

        ppanel = GameObject.Find("PracticePanel");
        ppanel.SetActive(false);
        fimgpanel = GameObject.Find("FemaleImagePanel");
        mimgpanel = GameObject.Find("MaleImagePanel");
    }

    // Update is called once per frame
    void Update () {
        //if (Time.time > 4)
        //    current_copy.SetActive(true);
    }
    void GeneratePressed(Button btn)
    {
        var gbt = btn.gameObject.GetComponentInChildren<Text>();
        if (gbt.text.Contains("G"))
        {
            //Move current system back to original location.
            // (May need to reset hinges, etc.)
            restore_mesh(renal, backups);
            renal.GetComponent<Renderer>().enabled = true;
            ppanel.SetActive(true);
            female_pressed = fimgpanel.activeSelf;
            fimgpanel.SetActive(false);
            mimgpanel.SetActive(false);
            gbt.text = "  RESET";

        }
        else
        {
            //Reset to original system
            restore_mesh(renal, backups);
        }
    }
    public void restore_mesh_public()
    {
        restore_mesh(renal, backups);
    }

}
