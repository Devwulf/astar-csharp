using System;

namespace AStar
{
    public class Program
    {
        static void Main(string[] args)
        {
            AStarPathfinder path = new AStarPathfinder();
            path.FindPath();
            path.PrintMap();

#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
        }
    }
}
