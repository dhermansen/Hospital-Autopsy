#define ACTIVE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFlex;
 
public class CutFlexUtil
{    public static Vector3 shape_to_world(int i, FlexShapeMatching fsm)
    {
        return fsm.m_shapeRotations[i] * fsm.m_shapeCenters[i] / 100.0f + fsm.m_shapeTranslations[i];
    }
    public static void world_to_shape(Vector3 pt, int i, FlexShapeMatching fsm)
    {
        fsm.m_shapeCenters[i] = Quaternion.Inverse(fsm.m_shapeRotations[i]) * (pt - fsm.m_shapeTranslations[i]) * 100.0f;
    }
    private static bool sliced_by_shape(Vector3 particle_pos, Vector3 shape_center, Plane plane, Collider collider)
    {
        if (plane.SameSide(shape_center, particle_pos))
            return false;
        return collider.bounds.IntersectRay(new Ray(shape_center, particle_pos - shape_center));
    }
    public static void CutFlexSoft(Transform target, Vector3 blade1, Vector3 blade2, Vector3 blade3, Collider collider)
    {
        FlexShapeMatching shapes = target.GetComponent<FlexShapeMatching>();
        FlexParticles particles = target.GetComponent<FlexParticles>();

        List<int> indicies = new List<int>();
        List<int> offsets = new List<int>();

        List<int> otherIndices = new List<int>();
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
                if (sliced_by_shape(particlePos, centers[i], plane, collider))
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
            return;
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
        int shapeIndexOffset = 0;
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
                shapes.m_shapeRestPositions[shapeIndexOffset] = pos - shapes.m_shapeCenters[s];

                shapeIndexOffset++;
            }

            shapeStart = shapeEnd;
        }

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
