using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrawMesh : MonoBehaviour
{
    public GameObject leftPoint;
    //rightPoint;

    GameObject drawing;

    public MeshCollider drawArea;

    Camera cam;

    bool hasDrawingStarted;
    private bool IsCursorInDrawArea
    {
        get
        {
            return drawArea.bounds.Contains(cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, 11)));
        }
    }

    private void Start()
    {
        cam = GetComponent<PlayerInput>().camera;
    }
    public void StartDraw(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (!IsCursorInDrawArea)
            return;

        Debug.Log("Start Draw");
        StartCoroutine(Draw());
    }

    private IEnumerator Draw()
    {
        hasDrawingStarted = true;

        drawing = new GameObject("Drawing");

        drawing.transform.localScale = new Vector3(1, 1, 0);

        drawing.AddComponent<MeshFilter>();
        drawing.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>(new Vector3[8]);

        Vector3 startPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, 10));

        Vector3 temp = new Vector3(startPosition.x, startPosition.y, 0.5f);

        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = temp;
        }

        List<int> triangles = new List<int>(new int[36]);

        //Front Face
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 2;

        //Top Face
        triangles[6] = 2;
        triangles[7] = 3;
        triangles[8] = 4;
        triangles[9] = 2;
        triangles[10] = 4;
        triangles[11] = 5;

        //Right Face
        triangles[12] = 1;
        triangles[13] = 2;
        triangles[14] = 5;
        triangles[15] = 1;
        triangles[16] = 5;
        triangles[17] = 6;

        //Left Face
        triangles[18] = 0;
        triangles[19] = 7;
        triangles[20] = 4;
        triangles[21] = 0;
        triangles[22] = 4;
        triangles[23] = 5;

        //Back Face
        triangles[24] = 5;
        triangles[25] = 4;
        triangles[26] = 7;
        triangles[27] = 5;
        triangles[28] = 7;
        triangles[29] = 6;

        //Bottom Face
        triangles[30] = 0;
        triangles[31] = 6;
        triangles[32] = 7;
        triangles[33] = 0;
        triangles[34] = 1;
        triangles[35] = 6;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        drawing.GetComponent<MeshFilter>().mesh = mesh;
        drawing.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Unlit");
        drawing.GetComponent<Renderer>().material.color = new Color(0.1f, 0.1f, 0.1f);


        Vector3 lastMousePosition = startPosition;

        while (IsCursorInDrawArea)
        {
            float minDistance = 0.1f;

            float distance = Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, Input.mousePosition.y, 10)), lastMousePosition);

            while (distance < minDistance)
            {
                distance = Vector3.Distance(cam.ScreenToWorldPoint(new Vector3(
                Input.mousePosition.x, Input.mousePosition.y, 10)), lastMousePosition);

                yield return null;
            }

            vertices.AddRange(new Vector3[4]);
            triangles.AddRange(new int[30]);

            int vIndex = vertices.Count - 8;
            // previous vertices index
            int vIndex0 = vIndex + 3;
            int vIndex1 = vIndex + 2;
            int vIndex2 = vIndex + 1;
            int vIndex3 = vIndex + 0;

            // new vertices index
            int vIndex4 = vIndex + 4;
            int vIndex5 = vIndex + 5;
            int vIndex6 = vIndex + 6;
            int vIndex7 = vIndex + 7;

            Vector3 currentMousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, 10));

            Vector3 mouseForwardVector = (currentMousePosition - lastMousePosition).normalized;

            float lineThickness = 0.1f;

            Vector3 topRightVertex = currentMousePosition + Vector3.Cross(mouseForwardVector, Vector3.back) * lineThickness;
            Vector3 bottomRightVertex = currentMousePosition + Vector3.Cross(mouseForwardVector, Vector3.forward) * lineThickness;

            Vector3 topLeftVertex = new Vector3(topRightVertex.x,
                topRightVertex.y, 1);
            Vector3 bottomLeftVertex = new Vector3(bottomRightVertex.x,
                bottomRightVertex.y, 1);


            vertices[vIndex4] = topLeftVertex;
            vertices[vIndex5] = topRightVertex;
            vertices[vIndex6] = bottomRightVertex;
            vertices[vIndex7] = bottomLeftVertex;

            int tIndex = triangles.Count - 30;

            //new top face
            triangles[tIndex + 0] = vIndex2;
            triangles[tIndex + 1] = vIndex3;
            triangles[tIndex + 2] = vIndex4;
            triangles[tIndex + 3] = vIndex2;
            triangles[tIndex + 4] = vIndex4;
            triangles[tIndex + 5] = vIndex5;

            //new right face
            triangles[tIndex + 6] = vIndex1;
            triangles[tIndex + 7] = vIndex2;
            triangles[tIndex + 8] = vIndex5;
            triangles[tIndex + 9] = vIndex1;
            triangles[tIndex + 10] = vIndex5;
            triangles[tIndex + 11] = vIndex6;

            //new left face
            triangles[tIndex + 12] = vIndex0;
            triangles[tIndex + 13] = vIndex7;
            triangles[tIndex + 14] = vIndex4;
            triangles[tIndex + 15] = vIndex0;
            triangles[tIndex + 16] = vIndex4;
            triangles[tIndex + 17] = vIndex3;

            //new back face
            triangles[tIndex + 18] = vIndex5;
            triangles[tIndex + 19] = vIndex4;
            triangles[tIndex + 20] = vIndex7;
            triangles[tIndex + 21] = vIndex0;
            triangles[tIndex + 22] = vIndex4;
            triangles[tIndex + 23] = vIndex3;

            //new bottom face
            triangles[tIndex + 18] = vIndex0;
            triangles[tIndex + 19] = vIndex6;
            triangles[tIndex + 20] = vIndex7;
            triangles[tIndex + 21] = vIndex0;
            triangles[tIndex + 22] = vIndex1;
            triangles[tIndex + 23] = vIndex6;

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            lastMousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, 10));

            yield return null;
        }
    }

    public void EndDraw(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (!hasDrawingStarted)
            return;

        hasDrawingStarted = false;

        Debug.Log("End Draw");

        StopAllCoroutines();
        ReDraw();

        Mesh mesh = drawing.GetComponent<MeshFilter>().mesh;

        Mesh leftPointMesh = new Mesh();
        leftPointMesh.vertices = mesh.vertices;
        leftPointMesh.triangles = mesh.triangles;

        Mesh rightPointMesh = new Mesh();
        rightPointMesh.vertices = mesh.vertices;
        rightPointMesh.triangles = mesh.triangles;
        rightPointMesh.RecalculateNormals();

        leftPoint.GetComponent<MeshFilter>().mesh = leftPointMesh;
      //  rightPoint.GetComponent<MeshFilter>().mesh = rightPointMesh;

        leftPoint.GetComponent<MeshCollider>().sharedMesh = leftPointMesh;
        //rightPoint.GetComponent<MeshCollider>().sharedMesh = rightPointMesh;

        Destroy(drawing);
    }

    void ReDraw()
    {
        Mesh mesh = drawing.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

       int[] triangles = mesh.triangles;
        

        //redraw
        for (int i = 1; i < vertices.Length;i++)
        {
            vertices[i] = new Vector3(vertices[i].x + (vertices[0].x * -1),
                vertices[i].y + (vertices[0].y * -1),
                vertices[i].z + (vertices[0].z * -1));

        }


        vertices[0] = Vector3.zero;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }
}

