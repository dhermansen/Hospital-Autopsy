#define ACTIVE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFlex;

public class CutFlexUtil
{
    public static Vector3 shape_to_world(int i, FlexShapeMatching fsm)
    {
        var rotated_center = fsm.m_shapeRotations[i] * fsm.m_shapeCenters[i];
        return rotated_center / 100.0f + fsm.m_shapeTranslations[i];
    }
    public static void world_to_shape(Vector3 pt, int i, FlexShapeMatching fsm)
    {
        fsm.m_shapeCenters[i] = Quaternion.Inverse(fsm.m_shapeRotations[i]) * (pt - fsm.m_shapeTranslations[i]) * 100.0f;
    }
    private static bool intersects(Vector3 tri1, Vector3 tri2, Vector3 tri3, Vector3 p1, Vector3 p2, bool debug)
    {
        Vector3 u, v, n;              // triangle vectors
        Vector3 dir, w0, w;           // ray vectors
        float r, a, b;              // params to calc ray-plane intersect

        // get triangle edge vectors and plane normal
        u = tri2 - tri1;
        v = tri3 - tri1;
        n = Vector3.Cross(u, v);              // cross product
        //if (n == (Vector)0)             // triangle is degenerate
        //    return false;                  // do not deal with this case

        dir = p2 - p1;              // ray direction vector
        w0 = p1 - tri1;
        a = -Vector3.Dot(n, w0);
        b = Vector3.Dot(n, dir);
        if (Mathf.Abs(b) < 1e-6)
        {     // ray is  parallel to triangle plane
            //if (a == 0)                 // ray lies in triangle plane
            //    return 2;
            //else return 0;              // ray disjoint from plane
            return false;
        }

        // get intersect point of ray with triangle plane
        r = a / b;
        if (debug)
            Debug.LogFormat("r: {0}", r);
        if (r < 0.0f || r > 1.0f)                    // ray goes away from triangle
            return false;                   // => no intersect
                                        // for a segment, also test if (r > 1.0) => no intersect

        var I = p1 + r * dir;            // intersect point of ray and plane

        // is I inside T?
        float uu, uv, vv, wu, wv, D;
        uu = Vector3.Dot(u, u);
        uv = Vector3.Dot(u, v);
        vv = Vector3.Dot(v, v);
        w = I - tri1;
        wu = Vector3.Dot(w, u);
        wv = Vector3.Dot(w, v);
        D = uv * uv - uu * vv;

        // get and test parametric coords
        float s, t;
        s = (uv * wv - vv * wu) / D;
        if (s < 0.0 || s > 1.0)         // I is outside T
            return false;
        t = (uv * wu - uu * wv) / D;
        if (t < 0.0 || (s + t) > 1.0)  // I is outside T
            return false;

        return true;                       // I is in T
    }
    //{
    //    var rayd = p2 - p1;
    //    // Vectors from p1 to p2/p3 (edges)
    //    var e1 = tri2 - tri1;
    //    var e2 = tri3 - tri1;
    //    var s1 = Vector3.Cross(rayd, e2);
    //    var divisor = Vector3.Dot(s1, e1);
    //    //if determinant is near zero, ray lies in plane of triangle otherwise not
    //    if (Mathf.Abs(divisor) < 1e-6)
    //        return false;
    //    var inv_divisor = 1.0f / divisor;

    //    //calculate distance from p1 to ray origin
    //    var d = p1 - tri1;
    //    var b1 = Vector3.Dot(d, s1) * inv_divisor;
    //    if (b1 < 0.0f || b1 > 1.0f)
    //        return false;
    //    var s2 = Vector3.Cross(d, e1);
    //    var b2 = Vector3.Dot(rayd, s2) * inv_divisor;
    //    if (b2 < 0.0f || b1 + b2 > 1.0f)
    //        return false;

    //    var t = Vector3.Dot(e2, s2) * inv_divisor;
    //    return t >= 0.0f && t <= 1.0f;
    //}
    public static bool intersects_quad(Vector3 p1, Vector3 p2, Vector3 blade1, Vector3 blade2, Vector3 blade3, Vector3 blade4, bool debug = false)
    {
        return intersects(blade1, blade2, blade3, p1, p2, debug) ||
               intersects(blade1, blade3, blade4, p1, p2, debug)/* ||
               intersects(blade2, blade3, blade4, p1, p2) ||
               intersects(blade2, blade3, blade1, p1, p2)*/;
    }
    public static bool CutFlexSoft(Transform target, Vector3 blade1, Vector3 blade2, Vector3 blade3, Vector3 blade4)
    {
        FlexShapeMatching shapes = target.GetComponent<FlexShapeMatching>();
        FlexParticles particles = target.GetComponent<FlexParticles>();

        List<int> indicies = new List<int>();
        List<int> offsets = new List<int>();
        //Debug.Log("Blade: " + blade1.ToString("F4") + blade2.ToString("F4") + blade3.ToString("F4") + blade4.ToString("F4") );
        List <int> otherIndices = new List<int>();
        var centers = Enumerable.Range(0, shapes.m_shapesCount).Select(i => shape_to_world(i, shapes)).ToList();
        int indexBeg = 0;
        int indexEnd = 0;
        var plane = new Plane(blade1, blade2, blade3);
        for (int i = 0; i < shapes.m_shapesCount; ++i)
        {
            indexEnd = shapes.m_shapeOffsets[i];

            Vector3 shapeCenter = shapes.m_shapeCenters[i];
            for (int j = indexBeg; j < indexEnd; ++j)
            {
                int id = shapes.m_shapeIndices[j];
                Vector3 particlePos = particles.m_particles[id].pos;
                var qisect = intersects_quad(particlePos, centers[i], blade1, blade2, blade3, blade4);
                //var cisect = !plane.SameSide(particlePos, centers[i]) && c.bounds.IntersectRay(new Ray(particlePos, centers[i] - particlePos));
                //if (qisect && !cisect)
                //{
                //    Debug.Log("Unwanted: " + particlePos.ToString("F4") + ';' + centers[i].ToString("F4"));
                //    intersects_quad(particlePos, centers[i], blade1, blade2, blade3, blade4, true);
                //}
                //if (!qisect && cisect)
                //{
                //    Debug.Log("Wanted: " + particlePos.ToString("F4") + ';' + centers[i].ToString("F4"));
                //    intersects_quad(particlePos, centers[i], blade1, blade2, blade3, blade4, true);
                //}
                if (qisect)
                {
                    if (!otherIndices.Contains(id))
                    {
                        otherIndices.Add(id);
                    }
                }
                else
                {
                    indicies.Add(id);
                }
            }
            offsets.Add(indicies.Count);
            indexBeg = indexEnd;
        }
        if (otherIndices.Count == 0)
            return false;
        for (int i = 0; i < otherIndices.Count; i++)
        {
            if (!indicies.Contains(otherIndices[i]))
            {
                int index = FindClosedBoneIndexOnSameSide(centers.ToArray(), particles.m_particles[otherIndices[i]].pos, plane);
                int atIndex = offsets[index];
                indicies.Insert(atIndex, otherIndices[i]);
                for (int j = index; j < offsets.Count; j++)
                {
                    offsets[j] += 1;
                }
            }
        }
        if (indicies.Count - shapes.m_shapeIndicesCount != 0)
        {
            Debug.LogFormat("Shape counts difference: {0}",
                indicies.Count - shapes.m_shapeIndicesCount);
            Debug.LogFormat("Shape count: {0}", offsets.Count);
        }
        shapes.m_shapeIndicesCount = indicies.Count;
        shapes.m_shapeIndices = indicies.ToArray();
        shapes.m_shapeOffsets = offsets.ToArray();

        int shapeStart = 0;
        int shapeIndex = 0;
        var rest_positions = new Vector3[indicies.Count];
        for (int s = 0; s < shapes.m_shapesCount; s++)
        {
            shapes.m_shapeTranslations[s] = new Vector3();
            shapes.m_shapeRotations[s] = Quaternion.identity;

            shapeIndex++;

            int shapeEnd = shapes.m_shapeOffsets[s];
            //-----------------------------------------cccc------------------------------------------------//

            Vector3 cen = Vector3.zero;
            for (int i = shapeStart; i < shapeEnd; ++i)
            {
                int p = shapes.m_shapeIndices[i];
                Vector3 pos = particles.m_restParticles[p].pos;
                cen += pos;
            }
            cen /= (shapeEnd - shapeStart);
            shapes.m_shapeCenters[s] = cen;

            //--------------------------------------------cccc---------------------------------------------//
            for (int i = shapeStart; i < shapeEnd; ++i)
            {
                int p = shapes.m_shapeIndices[i];

                // remap indices and create local space positions for each shape
                Vector3 pos = particles.m_restParticles[p].pos;
                rest_positions[i] = pos - shapes.m_shapeCenters[s];
            }

            shapeStart = shapeEnd;
        }
        shapes.m_shapeRestPositions = rest_positions;
        //for (int i = 0, endi = mesh.triangles.Count(); i < endi; i += 3)
        //{
        //    var pt1 = mesh.vertices[i+0];
        //    var pt2 = mesh.vertices[i+1];
        //    var pt3 = mesh.vertices[i+2];
        //    if (sliced_by_shape(pt1, pt3, plane, collider) || sliced_by_shape(pt2, pt3, plane, collider))
        //    {
        //        //Store this triangle.
        //        pts.Add(pt1);
        //        pts.Add(pt2);
        //        pts.Add(pt3);
        //    }
        //}
        //BoneWeight[] boneWeights = new BoneWeight[mesh.vertexCount];
        //for (int i = 0; i < mesh.vertexCount; i++)
        //{
        //    boneWeights[i] = mesh.boneWeights[i];
        //    Vector3 vertexPos = mesh.vertices[i];
        //    mesh.boneWeights[i] = CheckWeight(boneWeights[i], vertexPos, shapes.m_shapeCenters, plane);
        //}
        return true;
    }
    public static BoneWeight CheckWeight(BoneWeight weight, Vector3 vert, Vector3[] bones, Plane plane)
    {
        //return weight;
        Vector3 shapeCenter = bones[weight.boneIndex0];
        float value;

        int flag = 0;
        if (plane.SameSide(shapeCenter, vert) == false)
        {
            value = weight.weight0;
            weight.weight0 = 0;


            int index = FindClosedBoneIndexOnSameSide(new Vector3[] { bones[weight.boneIndex1], bones[weight.boneIndex2], bones[weight.boneIndex3] }, vert, plane);
            if (index == 0) weight.weight1 += value;
            else if (index == 1) weight.weight2 += value;
            else if (index == 2) weight.weight3 += value;

            flag++;
        }

        shapeCenter = bones[weight.boneIndex1];
        if (plane.SameSide(shapeCenter, vert) == false)
        {
            value = weight.weight1;
            weight.weight1 = 0;
            //
            int index = FindClosedBoneIndexOnSameSide(new Vector3[] { bones[weight.boneIndex0], bones[weight.boneIndex2], bones[weight.boneIndex3] }, vert, plane);
            if (index == 0) weight.weight0 += value;
            else if (index == 1) weight.weight2 += value;
            else if (index == 2) weight.weight3 += value;
            flag++;
        }
        shapeCenter = bones[weight.boneIndex2];
        if (plane.SameSide(shapeCenter, vert) == false)
        {
            value = weight.weight2;
            weight.weight2 = 0;

            int index = FindClosedBoneIndexOnSameSide(new Vector3[] { bones[weight.boneIndex0], bones[weight.boneIndex1], bones[weight.boneIndex3] }, vert, plane);
            if (index == 0) weight.weight0 += value;
            else if (index == 1) weight.weight1 += value;
            else if (index == 2) weight.weight3 += value;

            flag++;
        }
        shapeCenter = bones[weight.boneIndex3];
        if (plane.SameSide(shapeCenter, vert) == false)
        {
            value = weight.weight3;
            weight.weight3 = 0;

            int index = FindClosedBoneIndexOnSameSide(new Vector3[] { bones[weight.boneIndex0], bones[weight.boneIndex1], bones[weight.boneIndex2] }, vert, plane);
            if (index == 0) weight.weight0 += value;
            else if (index == 1) weight.weight1 += value;
            else if (index == 2) weight.weight2 += value;

            flag++;
        }
        if (flag > 3)
        {
            Debug.Log(weight.weight0 + "------" + weight.weight1 + "------" + weight.weight2 + "------" + weight.weight3);

            weight.boneIndex0 = FindClosedBoneIndexOnSameSide(bones, vert, plane);
            weight.weight0 = 1;
        }

        return weight;

    }

    public static int FindClosedBoneIndexOnSameSide(Vector3[] BonePos, Vector3 vert, Plane plane)
    {
        int index = -1;
        float dis = float.MaxValue;
        for (int i = 0; i < BonePos.Length; i++)
        {
            if (plane.SameSide(BonePos[i], vert))
            {
                if (Vector3.Distance(BonePos[i], vert) < dis)
                {
                    index = i;
                    dis = Vector3.Distance(BonePos[i], vert);
                }
            }
        }
        return index;
    }
}
