using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LMagent.Astar {
    public class Node : IComparable<Node>
    {
        public Vector2Int gridPosition;
        public int gCost = 0; // distance to Start Grid
        public int hCost = 0; // distance to Target Grid

        public int FCost => gCost + hCost;
        public bool isObstacle = false; 
        public Node parentNode;

        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }

        public int CompareTo(Node other)
        {
            int result = FCost.CompareTo(other.FCost);
            if (result == 0) {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }
}