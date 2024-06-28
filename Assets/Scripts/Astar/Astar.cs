using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LMagent.Astar {
    public class Astar : Singleton<Astar>
    {
        private GridNodes gridNodes;
        private Node startNode;
        private Node targetNode;
        private int gridWidth;
        private int gridHeight;
        private int originX;
        private int originY;

        private List<Node> openNodelist; // Nodes around current Node 
        private HashSet<Node> closedNodelist; // all selected Nodes

        private bool pathFound;

        public void BuildPath(Vector2Int startPos, Vector2Int endPos, Stack<MovementStep> npcMovementStack)
        {
            pathFound = false;

            if (GenerateGridNodes(startPos, endPos)) {
                // Debug.Log(startPos);
                if (FindShortestPath()) {
                    UpdatePathOnMovementStepStack(npcMovementStack);
                }
            }
        }

        private bool GenerateGridNodes(Vector2Int startPos, Vector2Int endPos) 
        {
            if (GridMapManager.Instance.GetGridDimensions(out Vector2Int gridDimensions, out Vector2Int gridOrigin)) {
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                openNodelist = new List<Node>();

                closedNodelist = new HashSet<Node>();
            }
            else {
                return false;
            }
            startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
            targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);
            // Debug.Log(startNode.gridPosition);
            // Debug.Log(targetNode.gridPosition);

            for (int x = 0; x < gridWidth; x++) {
                for (int y = 0; y < gridHeight; y++) {
                    Vector3Int tilePos = new(x + originX, y + originY, 0);

                    TileDetails tile = GridMapManager.Instance.GetTileDetailsByGridPos(tilePos);

                    if (tile != null) {
                        Node node = gridNodes.GetGridNode(x, y);
                        
                        if (tile.isObstacle) {
                            node.isObstacle = true;
                        }
                    }
                }
            }
            return true;
        }
    

        private bool FindShortestPath()
        {
            openNodelist.Add(startNode);


            while (openNodelist.Count > 0) {
                openNodelist.Sort();

                Node closeNode = openNodelist[0];

                openNodelist.RemoveAt(0);
                closedNodelist.Add(closeNode);

                if (closeNode == targetNode) {
                    pathFound = true;
                    break;
                }

                EvaluateNeighbourNodes(closeNode);
            }


            return pathFound;
        }

        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node validNeighbourNode;

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    if ((x == 0 && y == 0) || ( x != 0 && y != 0)) continue;

                    validNeighbourNode = GetValidNeighbourNode(x + currentNodePos.x, y + currentNodePos.y);

                    if (validNeighbourNode != null) {
                        if (!openNodelist.Contains(validNeighbourNode)) {
                            validNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, validNeighbourNode);
                            validNeighbourNode.hCost = GetDistance(validNeighbourNode, targetNode);

                            validNeighbourNode.parentNode = currentNode;
                            // add to openNodelist
                            openNodelist.Add(validNeighbourNode);
                        }
                    }
                }
            }
        }

        private Node GetValidNeighbourNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0) return null;

            Node neighbourNode = gridNodes.GetGridNode(x, y);

            if (neighbourNode.isObstacle || closedNodelist.Contains(neighbourNode)) return null;
            
            return neighbourNode;
        }

        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance) {
                return 14 * yDistance + 10 * (xDistance - yDistance);
            }
            return 14 * xDistance + 10 * (yDistance - xDistance);
        }

        private void UpdatePathOnMovementStepStack(Stack<MovementStep> npcMovementStep)
        {
            Node nextNode = targetNode;

            while (nextNode != null) {
                MovementStep newStep = new MovementStep();
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);
                // Debug.Log(newStep.gridCoordinate);
                npcMovementStep.Push(newStep);
                nextNode = nextNode.parentNode;
            }
        }
    }
}