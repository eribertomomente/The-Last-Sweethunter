using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class DijkstraSquare : NetworkBehaviour
{
    
    private static int x = 6;
    private static int y = 25;

    [Range(0f, 1f)] public float edgeProbability;
    public Color edgeColor = Color.red;

    private static float gap = 2f;
    public Material pathMaterial;

    // what to put on the scene, not really meaningful
    public GameObject block;

    private GameObject[] blocks;

    protected static Node[,] matrix;
    protected static Graph g;

    public int intermediateStep = 4;

    [SerializeField]
    [Range(0f, 1f)]
    private float candyProbability;

    [SerializeField]
    private GameObject candyPrefab;
    [SerializeField]
    private GameObject chocolatePrefab;

    public static bool readyForMovement = false;
    

    public static int[] GetDimensions()
    {
        int[] res = { x, y };
        return res;
    }

    public static Graph GetGraph()
    {
        return g;
    }

    public static Node[,] GetMatrix()
    {
        return matrix;
    }

    public static float GetGap()
    {
        return gap;
    }

    void Start()
    {
        if (block != null)
        {
            
            // create a x * y matrix of nodes (and scene objects)
            matrix = CreateGrid(block, x, y, gap);

            // create a graph and put random edges inside
            g = new Graph();
            CreateLabyrinth(g, matrix, edgeProbability);

            // ask dijkstra to solve the problem
            Edge[] path = DijkstraSolver.Solve(g, matrix[0, 0], matrix[0, y - 1]);
            // check if there is a solution, otherwise create another labyrinth
            //int test = 0;
            while (path.Length == 0)
            {
                g = new Graph();
                CreateLabyrinth(g, matrix, edgeProbability);
                path = DijkstraSolver.Solve(g, matrix[0, 0], matrix[0, y - 1]);
                //test++;
            }
            
            // draw the connections between blocks
            DrawPath();

            blocks = GameObject.FindGameObjectsWithTag("grass");

            InsertChocolateBar();

            SpawnCandies();

            readyForMovement = true;

           
        }
    }
    
    protected virtual Node[,] CreateGrid(GameObject o, int x, int y, float gap)
    {
        Node[,] matrix = new Node[x, y];
        for (int i = 0; i < x; i += 1)
        {
            for (int j = 0; j < y; j += 1)
            {
                matrix[i, j] = new Node("" + i + "," + j, Instantiate(o));
                matrix[i, j].sceneObject.name = o.name;
                matrix[i, j].sceneObject.transform.position =
                    transform.position +
                    transform.right * gap * (i - ((x - 1) / 2f)) +
                    transform.up * gap * (j - ((y - 1) / 2f));
                matrix[i, j].sceneObject.transform.rotation = transform.rotation;
            }
        }
        return matrix;
    }
    
    protected void CreateLabyrinth(Graph g, Node[,] crossings, float threshold)
    {
        int borderWidth = crossings.GetLength(0);
        int borderHeight = crossings.GetLength(1);

        // adding random edges in the labirint
        for (int i = 0; i < borderWidth; i += 1)
        {
            for (int j = 0; j < borderHeight; j += 1)
            {
                g.AddNode(crossings[i, j]);
                foreach (Edge e in RandomEdges(crossings, i, j, threshold))
                {
                    g.AddEdge(e);
                }
            }
        }
        // adding edges along the border
        for (int i = 0; i < borderWidth - 1; i += 1)
        {
            // lower border
            g.AddEdge(new Edge(crossings[i, 0],
                                crossings[i + 1, 0],
                                Distance(crossings[i, 0], crossings[i + 1, 0])
                                ));
            // upper border
            g.AddEdge(new Edge(crossings[i, borderHeight - 1],
                                crossings[i + 1, borderHeight - 1],
                                Distance(crossings[i, borderHeight - 1], crossings[i + 1, borderHeight - 1])
                                ));
        }

        // adding intermediate steps
        for (int h = intermediateStep; h < borderHeight; h = h + 5)
        {
            for (int i = 0; i < borderWidth - 1; i += 1)
            {
                g.AddEdge(new Edge(crossings[i, h-1],
                                    crossings[i + 1, h-1],
                                    Distance(crossings[i, h], crossings[i+1, h-1])
                                    ));

            }
        }
        // adding side edges
        for (int i = 0; i < borderHeight - 1; i += 1)
        {
            if (i % 2 == 0)
            {
                g.AddEdge(new Edge(crossings[0, i],
                                    crossings[0, i + 1],
                                    Distance(crossings[0, i], crossings[0, i + 1])
                                    ));
            } else
            {
                g.AddEdge(new Edge(crossings[borderWidth-1, i],
                                    crossings[borderWidth-1, i + 1],
                                    Distance(crossings[borderWidth-1, i], crossings[borderWidth-1, i + 1])
                                    ));
            }
        }
    }
    
    protected Edge[] RandomEdges(Node[,] matrix, int x, int y, float threshold)
    {
        List<Edge> result = new List<Edge>();
        if (x != 0 && Random.Range(0f, 1f) <= threshold)
            result.Add(new Edge(matrix[x, y], matrix[x - 1, y], Distance(matrix[x, y], matrix[x - 1, y])));

        if (y != 0 && Random.Range(0f, 1f) <= threshold)
            result.Add(new Edge(matrix[x, y], matrix[x, y - 1], Distance(matrix[x, y], matrix[x, y - 1])));

        if (x != (matrix.GetLength(0) - 1) && Random.Range(0f, 1f) <= threshold)
            result.Add(new Edge(matrix[x, y], matrix[x + 1, y], Distance(matrix[x, y], matrix[x + 1, y])));

        if (y != (matrix.GetLength(1) - 1) && Random.Range(0f, 1f) <= threshold)
            result.Add(new Edge(matrix[x, y], matrix[x, y + 1], Distance(matrix[x, y], matrix[x, y + 1])));

        return result.ToArray();
    }
    
    protected virtual float Distance(Node from, Node to)
    {
        return 1f;
    }
    
    void DrawPath()
    {
        if (matrix != null)
        {
            for (int i = 0; i < x; i += 1)
            {
                for (int j = 0; j < y; j += 1)
                {
                    foreach (Edge e in g.getConnections(matrix[i, j]))
                    {
                        DrawLine(e.from.sceneObject.transform.position, e.to.sceneObject.transform.position);
                    }
                }
            }
        }

    }
    
    void DrawLine(Vector3 start, Vector3 end, float thickness = 0.1f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = pathMaterial;
        lr.startColor = edgeColor;
        lr.endColor = edgeColor;
        lr.startWidth = thickness;
        lr.endWidth = thickness;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.sortingLayerName = "Background";

    }

    void SpawnCandies()
    {
        System.Random rnd = new System.Random();
        foreach (GameObject block in blocks){
            int current = rnd.Next(100);
            if (current < candyProbability * 100 && (block.transform.localPosition.y != 45.6f))
            {

                float xBlock = block.transform.position.x;
                float yBlock = block.transform.position.y;

                candyPrefab.transform.localScale = new Vector3(0.25f,0.25f,0.25f);

                Instantiate(candyPrefab, new Vector2(xBlock, yBlock), Quaternion.identity);
                //RpcSpawnCandies(xBlock, yBlock);
                
            }
        }
        
    }

    /*[ClientRpc]
    void RpcSpawnCandies(float xBlock, float yBlock)
    {

        if (!isServer) Instantiate(candyPrefab, new Vector2(xBlock, yBlock), Quaternion.identity);

    }*/

    void InsertChocolateBar()
    {
        foreach (GameObject block in blocks)
        {
            Vector3 finalBlockPosition = new Vector3(1, 45.6f, 0);
            if (block.transform.localPosition == finalBlockPosition)
            {
                GameObject go = Instantiate(chocolatePrefab, block.transform);

            }
        }
    }

}

