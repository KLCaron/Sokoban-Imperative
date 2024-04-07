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
            Queue<SokobanPuzzle> queue = new Queue<SokobanPuzzle>();
            HashSet<SokobanPuzzle> visited = new HashSet<SokobanPuzzle>();
            int permutations = 0;
            
            queue.Enqueue(startState);
            visited.Add(startState);

            while (queue.Count > 0)
            {
                permutations++;
                SokobanPuzzle current = queue.Dequeue();
                
                if (current.IsSolved())
                {
                    Console.Out.WriteLine(permutations);
                    return ReconstructSolutionPath(current);
                }

                foreach (SokobanPuzzle next in current.GetPossibleMoves())
                {
                    string nextState = next.ToString();
                    //Console.Out.WriteLine(nextState);
                    if (!IsVisited(next, visited))
                    {
                        queue.Enqueue(next);
                        visited.Add(next);
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

        private static bool IsVisited(SokobanPuzzle state, HashSet<SokobanPuzzle> visited)
        {
            SokobanPuzzle current = state;
            while (current != null)
            {
                if (!visited.Contains(current))
                {
                    return false;
                }

                current = current.PreviousState;
            }

            return true;
        }
    }
}