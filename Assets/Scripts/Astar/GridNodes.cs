using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LMagent.Astar {
    public class GridNodes
    {
        private int width;
        private int height;
        private Node[,] gridNode;

        public GridNodes(int width, int height)
        {
            this.width = width;
            this.height = height;

            gridNode = new Node[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    gridNode[x, y] = new Node(new Vector2Int(x, y));
                }
            }
        }

        public Node GetGridNode(int xPos, int yPos) 
        {
            if (xPos < 0 || yPos < 0) return null;
            if (xPos < width && yPos < height) {
                return gridNode[xPos, yPos];
            }
            return null;
        }
    }
}
