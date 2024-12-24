using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class ProceduralMap : MonoBehaviour
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

    [SerializeField] private GameObject line;
    [SerializeField] private Transform lineHolder;
    [SerializeField] private GameObject rectangle;//map
    [SerializeField] private GameObject roomLine;    
    
    [SerializeField] private Tile tile;
    [SerializeField] private Tilemap tilemap;

    private void DivideTree(TreeNode treeNode, int n)
    {
        if (n >= maxNode) return;
        RectInt size = treeNode.TreeSize;
        int length = size.width >= size.height ? size.width : size.height;
        int maxLength = Mathf.Max(size.width, size.height); //긴 부분 기준으로 분할(가로-좌우, 세로-상하)
        int split = Mathf.RoundToInt(Random.Range(maxLength * minDivideRate, maxLength * maxDivideRate));
        //최소~최대 분할 길이에서 랜덤으로 값 결정
        if (size.width >= size.height)//가로가 길 때
        {
            treeNode.LeftTree = new TreeNode(size.x, size.y, split, size.height); //좌측트리노드. 가로길이-Split(랜덤값)
            treeNode.RightTree = new TreeNode(size.x + split, size.y, size.width - split, size.height);
            //우측트리 노드. Split에서 오른쪽 
            //OnDrawLine(new Vector2(size.x+split, size.y), new Vector2(size.x+split, size.y+size.height));//두 트리노드를 나누는 선
        }
        else//세로가 길 때
        {
            treeNode.LeftTree = new TreeNode(size.x, size.y, size.width, split); //split 기준으로 분할
            treeNode.RightTree = new TreeNode(size.x , size.y + split, size.width, size.height - split);
            //OnDrawLine(new Vector2(size.x, size.y + split), new Vector2(size.x + size.width, size.y + split));//나누는 선
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

    private void OnDrawRoom(int x, int y, int width, int height)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                tilemap.SetTile(new Vector3Int(i-mapSize.x/2, j-mapSize.y/2, 0), tile);
            }
        }
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
    
    private int GetCenterX(RectInt size)
    {
        return size.x + size.width / 2;
    }

    private int GetCenterY(RectInt size)
    {
        return size.y + size.height / 2;
    }
    
    private void Awake()
    {
        //OnDrawRectangle(0,0,mapSize.x,mapSize.y);
        TreeNode rootNode = new TreeNode(0,0, mapSize.x,mapSize.y);
        DrawMap(0,0);
        DivideTree(rootNode,0);
        GenerateRoom(rootNode,0);
        GenerateRoad(rootNode,0);
        //3D 오브젝트로 구현
    }
}
