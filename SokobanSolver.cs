using System;
using System.Collections.Generic;
using System.IO;

namespace Sokoban_Imperative
{
    public class SokobanSolver
    {
        static void Main(string[] args)
        {
            const string filePath = "../../puzzle.txt";

            TileType[,] importPuzzle = SokobanReader.FromFile(filePath);

            SokobanPuzzle puzzle = new SokobanPuzzle(importPuzzle);
            
            // Just for testing purposes
            // Console.WriteLine("ToString test:");
            // Console.WriteLine(puzzle.ToString());
            
            bool solved = SolvePuzzle(puzzle);

            if (solved)
            {
                Console.WriteLine("Puzzle solved: ");
                Console.WriteLine(puzzle.ToString());
            }
            else
            {
                Console.WriteLine("No solution found.");
            }
        }

        public static bool SolvePuzzle(SokobanPuzzle startState)
        {
            Stack<SokobanPuzzle> stack = new Stack<SokobanPuzzle>();
            HashSet<string> visited = new HashSet<string>();
            
            stack.Push(startState);
            visited.Add(startState.ToString());

            while (stack.Count > 0)
            {
                SokobanPuzzle current = stack.Peek();
                Console.WriteLine(current.ToString());
                
                if (current.IsSolved())
                {
                    return true;
                }

                bool foundMove = false;

                foreach (SokobanPuzzle next in current.GetPossibleMoves())
                {
                    string nextState = next.ToString();
                    if (!visited.Contains(nextState))
                    {
                        stack.Push(next);
                        visited.Add(nextState);
                        foundMove = true;
                        break;
                    }
                }

                if (!foundMove)
                {
                    stack.Pop();
                }
            }
            
            return false;
        }
    }
}