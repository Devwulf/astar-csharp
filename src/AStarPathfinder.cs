using System;
using System.Collections.Generic;
using AStar.Utils;

namespace AStar
{
    class AStarPathfinder
    {
        public class WeighedBlock : IComparable<WeighedBlock>
        {
            public Vector2Int Position { get; }
            public double StepWeight = 0;
            public double DistanceWeight => Vector2Int.SqrDistance(Position, _endBlock.Position);
            public double TotalWeight => StepWeight + DistanceWeight;
            public WeighedBlock PrevBlock;

            private WeighedBlock _endBlock;

            public WeighedBlock(Vector2Int position, WeighedBlock endBlock)
            {
                if (endBlock == null)
                    _endBlock = this;

                Position = position;
                _endBlock = endBlock;
            }

            public int CompareTo(WeighedBlock other)
            {
                if (ReferenceEquals(this, other)) return 0;
                if (ReferenceEquals(null, other)) return 0;

                return TotalWeight.CompareTo(other.TotalWeight);
            }

            public override bool Equals(object obj)
            {
                var block = obj as WeighedBlock;
                if (block == null)
                    return false;

                return Position == block.Position;
            }

            public override int GetHashCode()
            {
                return int.Parse(Position.x.ToString() + Position.y.ToString());
            }

            public Vector2Int[] GetSurroundingLocations()
            {
                var array = new Vector2Int[8];
                // East
                array[0] = new Vector2Int(Position.x + 1, Position.y);
                // South East
                array[1] = new Vector2Int(Position.x + 1, Position.y + 1);
                // South
                array[2] = new Vector2Int(Position.x, Position.y + 1);
                // South West
                array[3] = new Vector2Int(Position.x - 1, Position.y + 1);
                // West
                array[4] = new Vector2Int(Position.x - 1, Position.y);
                // North West
                array[5] = new Vector2Int(Position.x - 1, Position.y - 1);
                // North
                array[6] = new Vector2Int(Position.x, Position.y - 1);
                // North East
                array[7] = new Vector2Int(Position.x + 1, Position.y - 1);

                return array;
            }
        }

        // W = wall
        // S = start
        // E = end
        private char[,] _map =
        {
            { 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W' },
            { 'W', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 'W', ' ', ' ', 'W' },
            { 'W', ' ', 'W', 'W', 'W', ' ', 'W', ' ', 'W', ' ', 'W', 'W' },
            { 'W', ' ', ' ', ' ', 'W', 'W', 'W', ' ', 'W', ' ', 'W', 'W' },
            { 'W', 'W', 'W', ' ', 'W', ' ', ' ', ' ', ' ', ' ', ' ', 'W' },
            { 'W', 'S', 'W', ' ', 'W', 'W', 'W', 'W', 'W', 'W', ' ', 'W' },
            { 'W', ' ', 'W', ' ', 'W', 'W', 'E', ' ', ' ', 'W', ' ', 'W' },
            { 'W', ' ', ' ', ' ', 'W', 'W', ' ', 'W', ' ', 'W', ' ', 'W' },
            { 'W', ' ', 'W', 'W', 'W', 'W', 'W', 'W', ' ', 'W', ' ', 'W' },
            { 'W', ' ', 'W', ' ', 'W', ' ', ' ', ' ', ' ', 'W', ' ', 'W' },
            { 'W', ' ', ' ', ' ', 'W', ' ', 'W', 'W', ' ', ' ', ' ', 'W' },
            { 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W' }
        };
        
        private WeighedBlock _start, _end;

        private Stack<Vector2Int> _path = new Stack<Vector2Int>();
        private readonly SortedLinkedList<WeighedBlock> _queue = new SortedLinkedList<WeighedBlock>();
        private readonly List<WeighedBlock> _finishedBlocks = new List<WeighedBlock>();

        // Creates WeighedBlocks only for the positions checked.
        public void FindPath()
        {
            FindStartAndEnd();

            // Now we need to weigh the blocks starting from the Start block
            _queue.SortedAddFront(_start);
            bool atEnd = false;
            while (_queue.Count > 0)
            {
                /* For debugging purposes.
                Console.Clear();
                Console.WriteLine($"Cleared screen! {_queue.Count}");
                foreach (var b in _queue)
                {
                    var prev = b.PrevBlock != null ? b.PrevBlock.Position.ToString() : "null";
                    Console.WriteLine($"Pos: {b.Position}, Prev: {prev}, Step: {b.StepWeight}, Dist: {b.DistanceWeight}, Total: {b.TotalWeight}");
                }
                /**/

                var block = _queue.PopFront();

                // Now we get the surrounding blocks
                var positions = block.GetSurroundingLocations();
                var sizeX = _map.GetLength(1);
                var sizeY = _map.GetLength(0);

                foreach (var pos in positions)
                {
                    // Skips the positions outside our map
                    if (pos.x < 0 || pos.x >= sizeX)
                        continue;
                    if (pos.y < 0 || pos.y >= sizeY)
                        continue;

                    // Skips the previous block that this block is linked to
                    if (block.PrevBlock != null && pos == block.PrevBlock.Position)
                        continue;

                    // Skips walls
                    var mapValue = _map[pos.y, pos.x];
                    if (mapValue.Equals('W'))
                        continue;

                    // Breaks out of this for loop when one of the neighboring
                    // blocks is the end block.
                    if (mapValue.Equals('E'))
                    {
                        Console.WriteLine("Found end!\n");
                        _end.StepWeight = block.StepWeight + 1;
                        _end.PrevBlock = block;
                        atEnd = true;
                        break;
                    }

                    // Skips if the new block has already been processed
                    // or is in the queue of being processed.
                    var newBlock = new WeighedBlock(pos, _end);
                    if (_finishedBlocks.Contains(newBlock))
                        continue;
                    if (_queue.Contains(newBlock))
                        continue;

                    newBlock.StepWeight = block.StepWeight + 1;
                    newBlock.PrevBlock = block;

                    _queue.SortedAddFront(newBlock);
                }

                if (atEnd)
                {
                    break;
                }

                _finishedBlocks.Add(block);
            }

            TracePath();
        }

        // Print the map
        public void PrintMap()
        {
            for (int row = 0; row < _map.GetLength(0); row++)
            {
                for (int col = 0; col < _map.GetLength(1); col++)
                {
                    Console.Write($"{_map[row, col]}");
                }
                Console.WriteLine();
            }
        }

        // Search for the start and end points.
        // Done together for efficiency.
        private void FindStartAndEnd()
        {
            Vector2Int startPos = new Vector2Int(), endPos = new Vector2Int();
            bool foundStart = false, foundEnd = false;

            for (int row = 0; row < _map.GetLength(0); row++)
            {
                if (foundStart && foundEnd)
                    break;

                for (int col = 0; col < _map.GetLength(1); col++)
                {
                    if (foundStart && foundEnd)
                        break;

                    var current = _map[row, col];
                    if (current.Equals('S'))
                    {
                        startPos = new Vector2Int(col, row);
                        foundStart = true;
                    }
                    else if (current.Equals('E'))
                    {
                        endPos = new Vector2Int(col, row);
                        foundEnd = true;
                    }
                }
            }

            if (!foundStart || !foundEnd)
                throw new Exception("No start and end points!");

            _end = new WeighedBlock(endPos, null);
            _start = new WeighedBlock(startPos, _end);
        }

        // Trace the path from end
        private void TracePath()
        {
            var currentBlock = _end.PrevBlock;
            while (currentBlock != null)
            {
                if (_map[currentBlock.Position.y, currentBlock.Position.x].Equals(' '))
                {
                    _path.Push(new Vector2Int(currentBlock.Position.x, currentBlock.Position.y));
                    _map[currentBlock.Position.y, currentBlock.Position.x] = 'X';
                }
                currentBlock = currentBlock.PrevBlock;
            }
        }
    }
}
