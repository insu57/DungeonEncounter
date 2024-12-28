using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class ProceduralMap : MonoBehaviour//일단 후순위로
{
    public class TreeNode
    {
        public TreeNode LeftTree;
        public TreeNode RightTree;
        public TreeNode ParentTree;
        public RectInt TreeSize; //분할된 공간의 rect
        public RectInt RoomSize; //공간 속의 방 rect

        public Vector2Int Center => new(RoomSize.x+RoomSize.width/2, RoomSize.y + RoomSize.height/2);

        public TreeNode(int x, int y, int width, int height)
        {
            TreeSize.x = x;
            TreeSize.y = y;
            TreeSize.width = width;
            TreeSize.height = height;
        }
    }
    
    //Dungeon Procedural...
    private int _maxMapWidth;
    private int _maxMapHeight;
    [SerializeField] private Vector2Int mapSize;//맵 크기
    [SerializeField] private int maxNode; //최대 노드(높을 수록 )
    [SerializeField] private float minDivideRate; //분할 최소 비율
    [SerializeField] private float maxDivideRate; //분할 최대 비율
    [SerializeField] private int minRoomSize;

    [SerializeField] private GameObject line3d;
    [SerializeField] private GameObject floorPrefab;
    
    [SerializeField] private GameObject line;
    [SerializeField] private GameObject rectangle;//map
    [SerializeField] private GameObject roomLine;    

    private void DivideTree(TreeNode treeNode, int n)
    {
        if (n >= maxNode) return;
        RectInt size = treeNode.TreeSize;
        int maxLength = Mathf.Max(size.width, size.height); //긴 부분 기준으로 분할(가로-좌우, 세로-상하)
        int split = Mathf.RoundToInt(Random.Range(maxLength * minDivideRate, maxLength * maxDivideRate));
        //최소~최대 분할 길이에서 랜덤으로 값 결정
        if (size.width >= size.height)//가로가 길 때
        {
            treeNode.LeftTree = new TreeNode(size.x, size.y, split, size.height); //좌측트리노드. 가로길이-Split(랜덤값)
            treeNode.RightTree = new TreeNode(size.x + split, size.y, size.width - split, size.height);
            //우측트리 노드. Split에서 오른쪽 
            //OnDrawLine(new Vector2(size.x+split, size.y), new Vector2(size.x+split, size.y+size.height));//두 트리노드를 나누는 선
            //CreateLine(new Vector3(size.x + split, 0, size.y), new Vector3(size.x + split, 0, size.y + size.height), 5);
        }
        else//세로가 길 때
        {
            treeNode.LeftTree = new TreeNode(size.x, size.y, size.width, split); //split 기준으로 분할
            treeNode.RightTree = new TreeNode(size.x , size.y + split, size.width, size.height - split);
            //OnDrawLine(new Vector2(size.x, size.y + split), new Vector2(size.x + size.width, size.y + split));//나누는 선
            //CreateLine(new Vector3(size.x, 0, size.y+split), new Vector3(size.x + size.width, 0, size.y + split), 5);
        }
        treeNode.LeftTree.ParentTree = treeNode;
        treeNode.RightTree.ParentTree = treeNode;
        DivideTree(treeNode.LeftTree, n+1);
        DivideTree(treeNode.RightTree, n+1);
    }

    private RectInt GenerateRoom(TreeNode treeNode, int n)
    {
        if (n == maxNode)
        {
            RectInt size = treeNode.TreeSize;
            int width = Random.Range(size.width/2 , size.width - 1); //가로넓이/2~가로넓이-1 사이 랜덤 길이
            int height = Mathf.Max(Random.Range(size.height / 2 , size.height - 1));
            int x = treeNode.TreeSize.x + Random.Range(1, size.width - width); //최솟값 1 (방 겹치기 때문에) ~ width(랜덤길이)
            int y = treeNode.TreeSize.y + Random.Range(1, size.height - height);
            OnDrawRectangle(x, y, width, height);
            return new RectInt(x, y, width, height);
        }
        treeNode.LeftTree.RoomSize = GenerateRoom(treeNode.LeftTree, n+1);
        treeNode.RightTree.RoomSize = GenerateRoom(treeNode.RightTree, n+1);
        return treeNode.LeftTree.RoomSize;
    }

    private void GenerateRoad(TreeNode treeNode, int n)
    {
        if (n == maxNode) return;
        Vector2Int leftNodeCenter = treeNode.LeftTree.Center;
        Vector2Int rightNodeCenter = treeNode.RightTree.Center;
        
        OnDrawLine(new Vector2(leftNodeCenter.x, leftNodeCenter.y), new Vector2(rightNodeCenter.x, leftNodeCenter.y));
        OnDrawLine(new Vector2(rightNodeCenter.x, leftNodeCenter.y), new Vector2(rightNodeCenter.x, rightNodeCenter.y));
        
        GenerateRoad(treeNode.LeftTree, n+1);
        GenerateRoad(treeNode.RightTree, n+1);
    }
    
    private void OnDrawLine(Vector2 from, Vector2 to)
    {
        LineRenderer lineRenderer = Instantiate(line).GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.SetPosition(0, from - mapSize / 2);
        lineRenderer.SetPosition(1, to - mapSize / 2);
    }
    private void OnDrawRectangle(int x, int y, int width, int height)
    {
        LineRenderer lineRenderer = Instantiate(roomLine).GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2);
        lineRenderer.SetPosition(1, new Vector2(x + width, y) - mapSize / 2);
        lineRenderer.SetPosition(2, new Vector2(x + width, y + height) - mapSize / 2);
        lineRenderer.SetPosition(3, new Vector2(x, y + height) - mapSize / 2);
    }

    private void DrawMap(int x, int y)
    {
        LineRenderer lineRenderer = Instantiate(rectangle).GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.SetPosition(0, new Vector2(x,y) - mapSize/2);
        lineRenderer.SetPosition(1,new Vector2(x+mapSize.x,y) - mapSize/2);
        lineRenderer.SetPosition(2, new Vector2(x+mapSize.x,y+mapSize.y) - mapSize/2);
        lineRenderer.SetPosition(3, new Vector2(x,y+mapSize.y) - mapSize/2);
    }

    private void DrawMap3D(int x, int z, float height)//Map외곽 그리기 3D
    {
        CreateLine(new Vector3(x, 0, z),
            new Vector3(x + mapSize.x, 0, z), height);
        CreateLine(new Vector3(x + mapSize.x, 0, z),
            new Vector3(x + mapSize.x, 0, z + mapSize.y), height);
        CreateLine(new Vector3(x + mapSize.x, 0, z + mapSize.y),
            new Vector3(x, 0, z + mapSize.y), height);
        CreateLine(new Vector3(x, 0, z + mapSize.y),
            new Vector3(x, 0, z), height);
    }

    private RectInt GenerateRoom3D(TreeNode treeNode, int n)
    {
        if (n == maxNode)
        {
            RectInt size = treeNode.TreeSize;
            int width = Random.Range(size.width/2 , size.width - 1); //가로넓이/2~가로넓이-1 사이 랜덤 길이
            int depth = Random.Range(size.height / 2 , size.height - 1);
            int x = treeNode.TreeSize.x + Random.Range(1, size.width - width); //최솟값 1 (방 겹치기 때문에) ~ width(랜덤길이)
            int z = treeNode.TreeSize.y + Random.Range(1, size.height - depth);
            CreateRect(x, z, width, depth, 5);
            float centerX = x + width/2.0f;
            float centerZ = z + depth/2.0f;
            CreateFloor(centerX,centerZ, width, depth);
            return new RectInt(x, z, width, depth);
        }
        treeNode.LeftTree.RoomSize = GenerateRoom3D(treeNode.LeftTree, n+1);
        treeNode.RightTree.RoomSize = GenerateRoom3D(treeNode.RightTree, n+1);
        return treeNode.LeftTree.RoomSize; 
    }

    private void GenerateRoad3D(TreeNode treeNode, int n)//가장 중요 어떻게?
         {
            if(n == maxNode) return;
            Vector2Int leftNodeCenter = treeNode.LeftTree.Center;
            Vector2Int rightNodeCenter = treeNode.RightTree.Center;
            
            CreateLine(new Vector3(leftNodeCenter.x, 0, leftNodeCenter.y),
                new Vector3(rightNodeCenter.x, 0, leftNodeCenter.y), 3);
            Vector3 start = new Vector3(leftNodeCenter.x, 0, leftNodeCenter.y);
            Vector3 end = new Vector3(rightNodeCenter.x, 0, leftNodeCenter.y);
            Debug.Log("Road1: " +(end-start).normalized);
            
            CreateLine(new Vector3(rightNodeCenter.x, 0, leftNodeCenter.y),
                new Vector3(rightNodeCenter.x, 0, rightNodeCenter.y), 3);
            //door..?
            
            
            
            GenerateRoad3D(treeNode.LeftTree, n+1);
            GenerateRoad3D(treeNode.RightTree, n+1);
         }
    
    private void CreateLine(Vector3 start, Vector3 end, float height)
    {
        //map(x,y)를 중심으로 생성. start-end로 벽 생성
        start.x -= mapSize.x / 2.0f;
        start.z -= mapSize.y / 2.0f;
        end.x -= mapSize.x / 2.0f;
        end.z -= mapSize.y / 2.0f;
        Vector3 position = (start + end) / 2;
        position.y = height/2;
        Vector3 direction = end - start;
        float length = direction.magnitude;
        
        GameObject newLine = Instantiate(line3d, position, Quaternion.identity);
        newLine.transform.forward = direction.normalized;
        newLine.transform.localScale = new Vector3(0.1f, height, length);//x-> 두깨(변수로 지정할수도?)
    }

    private void CreateRect(int x, int z, int width, int depth, int height)
    {
        CreateLine(new Vector3(x, 0, z),
            new Vector3(x + width, 0, z), height);
        CreateLine(new Vector3(x + width, 0, z),
            new Vector3(x + width, 0, z + depth), height);
        CreateLine(new Vector3(x + width, 0, z + depth),
            new Vector3(x, 0, z + depth), height);
        CreateLine(new Vector3(x, 0, z + depth),
            new Vector3(x, 0, z), height);
    }

    private void CreateFloor(float x, float z, int width, int depth)
    {
        Vector3 position = new Vector3(x - mapSize.x / 2.0f, 0, z - mapSize.y / 2.0f);
        GameObject floor = Instantiate(floorPrefab,position, Quaternion.identity);
        floor.transform.localScale = new Vector3(width/10f, 1, depth/10f);
    }
    private void Awake()
    {
        //OnDrawRectangle(0,0,mapSize.x,mapSize.y);
        TreeNode rootNode = new TreeNode(0, 0, mapSize.x, mapSize.y);
        //DrawMap(0,0);
        DrawMap3D(0,0, 5);
        DivideTree(rootNode,0);
        GenerateRoom3D(rootNode,0);
        GenerateRoad3D(rootNode,0);
        //GenerateRoom(rootNode,0);
        //GenerateRoad(rootNode,0);
        //3D 오브젝트로 구현
        
        
    }
}
