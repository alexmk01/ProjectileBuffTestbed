using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Game.Core.Map
{
    public sealed class GameMapGrid : IGameMapGrid
    {
        private struct Node
        {
            public static readonly Node Null = new()
            {
                EntityId = 0,
                CellIndex = -1,
                PrevNodeIndex = -1,
                NextNodeIndex = -1
            };

            public int CellIndex;
            public int EntityId;
            public int PrevNodeIndex;
            public int NextNodeIndex;

            public Node(int cellIndex, int entityId)
            {
                //Assert.AreNotEqual(entityId, 0);
                CellIndex = cellIndex;
                EntityId = entityId;
                PrevNodeIndex = -1;
                NextNodeIndex = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool HasPrevNode() => PrevNodeIndex != -1;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool HasNextNode() => NextNodeIndex != -1;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly bool HasEntity() => EntityId != 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ResetPrevNode() => PrevNodeIndex = -1;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ResetNextNode() => NextNodeIndex = -1;

            public void Invalidate() => this = Null;
        }

        private struct CellNodesEnumerator : IEnumerator<int>
        {
            public readonly int Current => current;
            readonly object IEnumerator.Current => current;

            private readonly Node[] nodes;
            private readonly int startNodeIndex;
            private int nodeIndex;
            private int current;

            public CellNodesEnumerator(GameMapGrid grid, int cellIndex)
            {
                nodes = grid.nodes;
                startNodeIndex = grid.cells[cellIndex];
                nodeIndex = startNodeIndex;
                current = -1;
            }

            public bool MoveNext()
            {
                if (nodeIndex < 0) return false;
                current = nodeIndex;
                nodeIndex = nodes[nodeIndex].NextNodeIndex;
                return true;
            }

            public void Reset()
            {
                nodeIndex = startNodeIndex;
                current = -1;
            }

            public void Dispose()
            {
                nodeIndex = -1;
                current = -1;
            }
        }

        private readonly ref struct CellNodesEnumerable
        {
            private readonly GameMapGrid grid;
            private readonly int cellIndex;

            public CellNodesEnumerable(GameMapGrid grid, int cellIndex)
            {
                this.grid = grid;
                this.cellIndex = cellIndex;
            }

            public CellNodesEnumerator GetEnumerator() => new(grid, cellIndex);
        }

        public int CellCount => cellCountX * cellCountY;
        public float CellSize => cellSize;
        public int CellCountX => cellCountX;
        public int CellCountY => cellCountY;
        public Vector2 Origin => origin;

        private readonly float cellSize;
        private readonly int cellCountX;
        private readonly int cellCountY;
        private readonly Vector2 origin;
        private readonly float invCellSize;
        private readonly int[] cells;
        private readonly Node[] nodes;
        private int nodeCount;

        private void AddNode(int entityId, int cellIndex)
        {
            //Assert.IsTrue(cellIndex >= 0);
            //Assert.IsTrue(nodeCount < nodes.Length);
            int nodeIndex = nodeCount;
            ref int currentCellHead = ref cells[cellIndex];
            Node newNode = new(cellIndex, entityId);
            newNode.ResetPrevNode();

            if (currentCellHead >= 0)
            {
                newNode.NextNodeIndex = currentCellHead;
                ref Node headNode = ref nodes[currentCellHead];
                headNode.PrevNodeIndex = nodeIndex;
                currentCellHead = nodeIndex;
            }
            else
            {
                newNode.ResetNextNode();
                currentCellHead = nodeIndex;
            }

            nodes[nodeIndex] = newNode;
            nodeCount++;
        }

        private void RemoveNode(int nodeIndex)
        {
            void UpdateNodeLinks(ref Node node, int prevNodeNextNodeIndex, int nextNodePrevNodeIndex)
            {
                if (node.HasPrevNode())
                {
                    ref Node prevNode = ref nodes[node.PrevNodeIndex];
                    prevNode.NextNodeIndex = prevNodeNextNodeIndex;
                }

                if (node.HasNextNode())
                {
                    ref Node nextNode = ref nodes[node.NextNodeIndex];
                    nextNode.PrevNodeIndex = nextNodePrevNodeIndex;
                }
            }

            void TrySetNewCellNodesHead(int cellIndex, int nodeIndex, int newHeadNodeIndex)
            {
                ref int headNodeIndex = ref cells[cellIndex];
                if (headNodeIndex == nodeIndex) headNodeIndex = newHeadNodeIndex;
            }

            //Assert.IsTrue(nodeCount > 0);
            ref Node node = ref nodes[nodeIndex];
            //Assert.IsTrue(node.cellIndex >= 0);
            UpdateNodeLinks(ref node, node.NextNodeIndex, node.PrevNodeIndex);
            TrySetNewCellNodesHead(node.CellIndex, nodeIndex, node.NextNodeIndex);
            node.Invalidate();
            int lastNodeIndex = --nodeCount;

            if (nodeIndex != lastNodeIndex)
            {
                ref Node lastNode = ref nodes[lastNodeIndex];
                UpdateNodeLinks(ref lastNode, nodeIndex, nodeIndex);
                TrySetNewCellNodesHead(lastNode.CellIndex, lastNodeIndex, nodeIndex);
                nodes[nodeIndex] = lastNode;
                lastNode.Invalidate();
            }
        }

        private void GetEntities(int cellIndex, Span<int> idsBuffer, ref int entityCount)
        {
            foreach (int nodeIndex in new CellNodesEnumerable(this, cellIndex))
            {
                ref readonly Node node = ref nodes[nodeIndex];

                if (node.HasEntity())
                {
                    idsBuffer[entityCount++] = node.EntityId;
                    if (entityCount == idsBuffer.Length) break;
                }
            }
        }

        public GameMapGrid(Vector2 origin, int cellCountX, int cellCountY, float cellSize, int maxNodesPerCell = 4)
        {
            this.origin = origin;
            this.cellSize = cellSize;
            invCellSize = 1f / cellSize;
            this.cellCountX = cellCountX;
            this.cellCountY = cellCountY;
            int totalCellCount = this.cellCountX * this.cellCountY;
            cells = new int[totalCellCount];
            nodes = new Node[maxNodesPerCell * totalCellCount];
            cells.AsSpan().Fill(-1);
            nodes.AsSpan().Fill(Node.Null);
        }

        public bool AddEntity(int entityId, Vector2 position)
        {
            int cellIndex = GetCellIndex(GetCellCoords(position));

            if (cellIndex >= 0)
            {
                AddNode(entityId, cellIndex);
                return true;
            }

            return false;
        }

        public bool RemoveEntity(int entityId)
        {
            for (int i = 0; i < nodeCount; i++)
            {
                ref Node node = ref nodes[i];
                
                if (node.EntityId == entityId)
                {
                    RemoveNode(i);
                    return true;
                }
            }
            
            return false;
        }
        
        public GameMapCellCoords GetCellCoords(Vector2 position)
        {
            return new((int)Math.Floor((position.X - origin.X) * invCellSize), (int)Math.Floor((position.Y - origin.Y) * invCellSize));
        }

        public GameMapCellCoords GetCellCoords(int cellIndex)
        {
            return new(cellIndex % cellCountX, cellIndex / cellCountX);
        }

        public int GetCellIndex(in GameMapCellCoords cellCoords)
        {
            int maxX = cellCountX - 1;
            int maxY = cellCountY - 1;
            int coordX = cellCoords.X;
            int coordY = cellCoords.Y;
            return coordX >= 0 && coordY >= 0 && coordX <= maxX && coordY <= maxY ? coordX + coordY * cellCountX : -1;
        }

        public Vector2 GetCellPosition(in GameMapCellCoords cellCoords)
        {
            float halfSize = cellSize * 0.5f;
            float offsetX = cellSize * cellCoords.X + halfSize;
            float offsetY = cellSize * cellCoords.Y + halfSize;
            return new(origin.X + offsetX, origin.Y + offsetY);
        }

        public bool IsBusyCell(in GameMapCellCoords cellCoords)
        {
            int cellIndex = GetCellIndex(cellCoords);
            return cellIndex >= 0 && cells[cellIndex] >= 0;
        }

        public int GetEntityCount(int cellIndex)
        {
            int entityCount = 0;

            if (cellIndex >= 0)
            {
                int nodeIndex = cells[cellIndex];

                while (nodeIndex >= 0)
                {
                    entityCount++;
                    nodeIndex = nodes[nodeIndex].NextNodeIndex;
                }
            }

            return entityCount;
        }

        public int GetEntities(int cellIndex, Span<int> entityIdsBuffer)
        {
            if (cellIndex >= 0)
            {
                int entityCount = 0;
                GetEntities(cellIndex, entityIdsBuffer, ref entityCount);
                return entityCount;
            }

            return 0;
        }
    }
}
