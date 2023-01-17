namespace AoC2022
{
    public struct Vector3Int
    {
        public int x;
        public int y;
        public int z;

        public Vector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Equals(Vector3Int other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        public override bool Equals(object obj)
        {
            return obj is Vector3Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y + z;
            }
        }
        public static bool operator ==(Vector3Int first, Vector3Int second) => first.x == second.x && first.y == second.y && first.z == second.z;
        public static bool operator !=(Vector3Int first, Vector3Int second) => !(first == second);
        public override string ToString()
        {
            return $"({x},{y},{z})";
        }

        public Vector2Int ToVector2Int()
        {
            return new Vector2Int(x,y);
        }
    }
}