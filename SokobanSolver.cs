using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Sokoban_Imperative
{
    /*
    * The solver for our Sokoban puzzles.
    */
    public static class SokobanSolver
    {
        /*
        * Main of the SokobanSolver program.
        */
        static void Main(string[] args)
        {

            Console.WriteLine("Enter the filepath, or test:");
            string filePath = Console.ReadLine();
            
            if (filePath == "test")
            {
                SokobanTester.Test();
            }
            else
            {
                try
                {
                    TileType[,] importPuzzle = SokobanReader.FromFile(filePath);
                    if (importPuzzle == null)
                    {
                        Console.WriteLine("Failed to import puzzle from the specified file.");
                        return;
                    }

                    SokobanPuzzle puzzle = new SokobanPuzzle(importPuzzle);
                    Stack<SokobanPuzzle> solutionStack = SolvePuzzle(puzzle);

                    if (solutionStack.Count > 0)
                    {
                        SokobanPrint.PrintSolution(puzzle, solutionStack);
                        Console.WriteLine("Puzzle solved.");

                    }
                    else
                    {
                        //Console.WriteLine(puzzle.ToString());
                        SokobanPrint.PrintPuzzleState(puzzle);
                        Console.WriteLine("No solution found.");
                    }
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("File not found, or invalid filepath.");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /*
        * Solves the Sokoban puzzle starting from the specified initial state.
        *
        * Parameters:
        *   startState: The initial state of the Sokoban puzzle.
        *
        * Returns:
        *   A stack of SokobanPuzzle instances representing the solution path, if found; otherwise, an empty stack.
        */
        public static Stack<SokobanPuzzle> SolvePuzzle(SokobanPuzzle startState)
        {
            Stack<SokobanPuzzle> stack = new Stack<SokobanPuzzle>();
            HashSet<List<SokobanPuzzle>> visited = new HashSet<List<SokobanPuzzle>>();
            int permutations = 0;

            stack.Push(startState);
            visited.Add(new List<SokobanPuzzle> { startState });

            while (stack.Count > 0)
            {
                permutations++;
                SokobanPuzzle current = stack.Pop();

                if (current.IsSolved())
                {
                    Console.Out.WriteLine(permutations);
                    return ReconstructSolutionPath(current);
                }

                foreach (SokobanPuzzle next in current.GetPossibleMoves())
                {
                    if (!IsVisited(next, visited))
                    {
                        List<SokobanPuzzle> newPath = new List<SokobanPuzzle>();
                        foreach (var state in visited)
                        {
                            newPath.AddRange(state);
                        }
                        newPath.Add(next);

                        stack.Push(next);
                        visited.Add(newPath);
                    }
                }
            }
            Console.Out.WriteLine(permutations);
            return new Stack<SokobanPuzzle>();
        }

        private static Stack<SokobanPuzzle> ReconstructSolutionPath(SokobanPuzzle finalState)
        {
            Stack<SokobanPuzzle> solutionPath = new Stack<SokobanPuzzle>();
            SokobanPuzzle current = finalState;

            while (current != null)
            {
                solutionPath.Push(current);
                current = current.PreviousState;
            }

            return solutionPath;
        }

        private static bool IsVisited(SokobanPuzzle state, HashSet<List<SokobanPuzzle>> visited)
        {
            foreach (var path in visited)
            {
                if (IsSamePath(path, state))
                {
                    return true;
                }
            }
            return false;
        }
        
        private static bool IsSamePath(List<SokobanPuzzle> path, SokobanPuzzle state)
        {
            foreach (var s in path)
            {
                if (s.Equals(state))
                {
                    return true;
                }
            }
            return false;
        }
    }
}