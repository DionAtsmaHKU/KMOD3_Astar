using System.Collections.Generic;
using UnityEngine;

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
    public List<Vector2Int> FindPathToTarget(Vector2Int _startPos, Vector2Int _endPos, Cell[,] _grid)
    {
        List<Node> visited = new List<Node>();

        Node startNode = new Node(_startPos, null, 0, 0);
        List<Node> unvisited = new List<Node>() { startNode };
       // List<Node> unvisited = AddNeighbourNodes(grid, visited, startNode);

        while (unvisited.Count > 0)
        {
            // Find current node
            Node current = LowestScore(unvisited);

            // Check if end reached
            if (current.position == _endPos)
            {
                Debug.Log("Path Found!");
                return ReconstructPath(current);
            }
            unvisited.Remove(current);
            visited.Add(current);

            List<Node> neighbourNodes = AddNeighbourNodes(_grid, visited, current);
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
                        SetHScore(n, _endPos);
                    }
                }
            }
        }
        Debug.Log("Couldn't find end");
        return null;
    }

    // Returns the Node with the lowest score in _unvisited
    private Node LowestScore(List<Node> _unvisited)
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

    // Sets the H score using the Manhattan distance
    private void SetHScore(Node _n, Vector2 _endPos)
    {
        _n.HScore = Mathf.Abs(_n.position.x - _endPos.x) + Mathf.Abs(_n.position.y - _endPos.y);
    }

    // Returns whether this cell has been visited
    private bool IsVisited(List<Node> _visited, Cell _cell)
    {
        for (int i = 0; i < _visited.Count; i++)
        {
            if (_cell.gridPosition == _visited[i].position)
                return true;
        }
        return false;
    }

    // Returns the path from start to end
    private List<Vector2Int> ReconstructPath(Node _endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node current = _endNode;
        while (current != null)
        {
            path.Add(current.position);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    // Returns a list of nodes neighbouring _current
    private List<Node> AddNeighbourNodes(Cell[,] _grid, List<Node> _visited, Node _current)
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

    // Returns a List of unreachable cells
    private List<Cell> AddUnreachableCells(Cell[,] _grid, Cell _current)
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
