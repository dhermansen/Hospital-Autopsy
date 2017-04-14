using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class slice_job
{
    public List<int> affected_vertices;
    public Vector3[] cutting_quad;
    public Vector3[] vertices;
    public int[][] indices;
    public int[] triangles;
    public Matrix4x4 transform;
    public bool is_done = false;
    public bool is_processing = false;
}
public static class slice_mesh
{
    public static void find_triangles(slice_job sj)
    {
        var new_indices = new int[sj.indices.Length][];
        sj.affected_vertices = new List<int>();
        for (int m = 0; m < sj.indices.Count(); ++m)
        {
            new_indices[m] = new int[sj.indices[m].Count()];
            for (int i = 0; i < sj.indices[m].Count(); i += 3)
            {
                var p1 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.indices[m][i + 0]]);
                var p2 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.indices[m][i + 1]]);
                var p3 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.indices[m][i + 2]]);
                if (CutFlexUtil.intersects_quad(p1, p2, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]) ||
                    CutFlexUtil.intersects_quad(p2, p3, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]) ||
                    CutFlexUtil.intersects_quad(p3, p1, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]))
                {
                    sj.affected_vertices.Add(sj.indices[m][i + 0]);
                    sj.affected_vertices.Add(sj.indices[m][i + 1]);
                    sj.affected_vertices.Add(sj.indices[m][i + 2]);
                }
                else
                {
                    new_indices[m][i + 0] = sj.indices[m][i + 0];
                    new_indices[m][i + 1] = sj.indices[m][i + 1];
                    new_indices[m][i + 2] = sj.indices[m][i + 2];
                }
            }
        }
        sj.indices = new_indices;
        //var new_triangles = new List<int>();
        //for (int i = 0; i < sj.triangles.Length; i += 3)
        //{
        //    var p1 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.triangles[i + 0]]);
        //    var p2 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.triangles[i + 1]]);
        //    var p3 = sj.transform.MultiplyPoint3x4(sj.vertices[sj.triangles[i + 2]]);
        //    if (!CutFlexUtil.intersects_quad(p1, p2, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]) &&
        //        !CutFlexUtil.intersects_quad(p2, p3, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]) &&
        //        !CutFlexUtil.intersects_quad(p3, p1, sj.cutting_quad[0], sj.cutting_quad[1], sj.cutting_quad[2], sj.cutting_quad[3]))
        //    {
        //        new_triangles.Add(sj.triangles[i + 0]);
        //        new_triangles.Add(sj.triangles[i + 1]);
        //        new_triangles.Add(sj.triangles[i + 2]);
        //    }
        //}
        //sj.triangles = new_triangles.ToArray();
        sj.is_done = true;
    }
}
