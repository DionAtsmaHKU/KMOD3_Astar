using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> visited = new List<Node>();

        Node startNode = new Node(startPos, null, 0, 0);
        List<Node> unvisited = new List<Node>() { startNode };
       // List<Node> unvisited = AddNeighbourNodes(grid, visited, startNode);

        while (unvisited.Count > 0)
        {
            // Find current node
            Node current = LowestScore(unvisited);

            // Check if end reached
            if (current.position == endPos)
            {
                Debug.Log("Path Found!");
                List<Vector2Int> path = ReconstructPath(current);
                path.Reverse();
                return path;
            }
            unvisited.Remove(current);
            visited.Add(current);

            List<Node> neighbourNodes = AddNeighbourNodes(grid, visited, current);
            foreach (Node n in neighbourNodes)
            {
                float tentativeGScore = current.GScore + 1;
                if (tentativeGScore < n.GScore)
                {
                    n.parent = current;
                    n.GScore = tentativeGScore;
                    if (!unvisited.Contains(n))
                    {
                        unvisited.Add(n);
                        SetHScore(n, endPos);
                    }
                }
            }
        }
        Debug.Log("Couldn't find end");
        return null;
    }

    Node LowestScore(List<Node> _unvisited)
    {
        float score = float.MaxValue;
        Node lowestNode = null;
        foreach (Node node in _unvisited)
        {
            if (node.FScore < score)
            {
                score = node.FScore;
                lowestNode = node;
            }
        }
        return lowestNode;
    }

    void SetHScore(Node n, Vector2 endPos)
    {
        n.HScore = Mathf.Abs(n.position.x - endPos.x) + Mathf.Abs(n.position.y - endPos.y);
    }

    bool IsVisited(List<Node> _visited, Cell _neighbour)
    {
        for (int i = 0; i < _visited.Count; i++)
        {
            if (_neighbour.gridPosition == _visited[i].position)
                return true;
        }
        return false;
    }

    List<Vector2Int> ReconstructPath(Node _endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node current = _endNode;
        while (current != null)
        {
            path.Add(current.position);
            current = current.parent;
        }
        return path;
    }

    List<Node> AddNeighbourNodes(Cell[,] _grid, List<Node> _visited, Node _current)
    {
        Cell currentCell = _grid[_current.position.x, _current.position.y];
        List<Cell> neighbourCells = currentCell.GetNeighbours(_grid);
        List<Node> neighbourNodes = new List<Node>();
        List<Cell> notReachable = AddUnreachableCells(_grid, currentCell);

        foreach (Cell neighbour in neighbourCells)
        {
            if (!IsVisited(_visited, neighbour) && !notReachable.Contains(neighbour))
            {
                neighbourNodes.Add(new Node(neighbour.gridPosition, _current, int.MaxValue, int.MaxValue));
            }
        }
        return neighbourNodes;
    }

    List<Cell> AddUnreachableCells(Cell[,] _grid, Cell _current)
    {
        List<Cell> unreachable = new List<Cell>();
        if (_current.HasWall(Wall.RIGHT) && _current.gridPosition.x + 1 <= _grid.GetLength(0) - 1)
        {
            unreachable.Add(_grid[_current.gridPosition.x + 1, _current.gridPosition.y]);
        }
        if (_current.HasWall(Wall.LEFT) && _current.gridPosition.x - 1 >= 0)
        {
            unreachable.Add(_grid[_current.gridPosition.x - 1, _current.gridPosition.y]);
        }
        if (_current.HasWall(Wall.UP) && _current.gridPosition.y + 1 <= _grid.GetLength(1) - 1)
        {
            unreachable.Add(_grid[_current.gridPosition.x, _current.gridPosition.y + 1]);
        }
        if (_current.HasWall(Wall.DOWN) && _current.gridPosition.y - 1 >= 0)
        {
            unreachable.Add(_grid[_current.gridPosition.x, _current.gridPosition.y - 1]);
        }
        return unreachable;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
