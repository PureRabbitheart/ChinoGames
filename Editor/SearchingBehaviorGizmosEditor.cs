﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class SearchingBehaviorGizmosEditor
{
    private static readonly int TRIANGLE_COUNT = 12;
    private static readonly Color MESH_COLOR = new Color(1.0f, 1.0f, 0.0f, 0.7f);


    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawPointGizmos(SearchingBehavior Searching, GizmoType gizmoType)
    {
        if (Searching.Radius <= 0.0f)
        {
            return;
        }

        Gizmos.color = MESH_COLOR;

        Transform transform = Searching.transform;
        Vector3 pos = transform.position + Vector3.up * 0.01f; // 0.01fは地面と高さだと見づらいので調整用。
        Quaternion rot = transform.rotation;
        Vector3 scale = Vector3.one * Searching.Radius;


        if (Searching.SearchAngle > 0.0f)
        {
            Mesh fanMesh = CreateFanMesh(Searching.SearchAngle, TRIANGLE_COUNT);

            Gizmos.DrawMesh(fanMesh, pos, rot, scale);
        }
    }

    private static Mesh CreateFanMesh(float angle, int triangleCount)
    {
        var mesh = new Mesh();

        var vertices = CreateFanVertices(angle, triangleCount);

        var triangleIndexes = new List<int>(triangleCount * 3);

        for (int i = 0; i < triangleCount; ++i)
        {
            triangleIndexes.Add(0);
            triangleIndexes.Add(i + 1);
            triangleIndexes.Add(i + 2);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangleIndexes.ToArray();

        mesh.RecalculateNormals();

        return mesh;
    }

    private static Vector3[] CreateFanVertices(float i_angle, int i_triangleCount)
    {
        if (i_angle <= 0.0f)
        {
            throw new System.ArgumentException(string.Format("角度がおかしい！ i_angle={0}", i_angle));
        }

        if (i_triangleCount <= 0)
        {
            throw new System.ArgumentException(string.Format("数がおかしい！ i_triangleCount={0}", i_triangleCount));
        }

        i_angle = Mathf.Min(i_angle, 360.0f);

        var vertices = new List<Vector3>(i_triangleCount + 2);

        // 始点
        vertices.Add(Vector3.zero);

        // Mathf.Sin()とMathf.Cos()で使用するのは角度ではなくラジアンなので変換しておく。
        float radian = i_angle * Mathf.Deg2Rad;
        float startRad = -radian / 2;
        float incRad = radian / i_triangleCount;

        for (int i = 0; i < i_triangleCount + 1; ++i)
        {
            float currentRad = startRad + (incRad * i);

            Vector3 vertex = new Vector3(Mathf.Sin(currentRad), 0.0f, Mathf.Cos(currentRad));
            vertices.Add(vertex);
        }

        return vertices.ToArray();
    }

}