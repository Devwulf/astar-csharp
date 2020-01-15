using System;

namespace AStar.Utils
{
    public struct Vector2Int
    {
        public readonly int x, y;

        public Vector2Int(int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Vector2Int other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector2Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        public static Vector2Int zero => new Vector2Int(0, 0);

        public static bool operator ==(Vector2Int left, Vector2Int right)
        {
            if (left.x == right.x &&
                left.y == right.y)
                return true;

            return false;
        }

        public static bool operator !=(Vector2Int left, Vector2Int right)
        {
            return !(left == right);
        }

        public static double SqrDistance(Vector2Int from, Vector2Int to)
        {
            return Math.Pow(to.x - from.x, 2) + Math.Pow(to.y - from.y, 2);
        }
    }
}
