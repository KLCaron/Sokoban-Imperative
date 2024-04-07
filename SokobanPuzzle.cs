using System.Collections.Generic;
using System.Text;


namespace Sokoban_Imperative
{
    /*
    * Represents the possible types of tiles in the Sokoban puzzle.
    */
    public enum TileType
    {
        Wall,
        Empty,
        Player,
        PlayerGoal,
        Box,
        BoxGoal,
        Goal
    }
    
    /*
    * Represents the possible directions the player can move in the Sokoban puzzle.
    */
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    
    /*
    * Represents a Sokoban puzzle, as well as the player movement options within that puzzle
    */
    public class SokobanPuzzle
    {
        private readonly TileType[,] _state;

        private readonly SokobanPuzzle _previousState;
        
        /*
        * Exposes the state of the puzzle.
        */
        public TileType[,] State => _state;
        
        /*
         * Exposes the state of the previous puzzle.
         */
        public SokobanPuzzle PreviousState => _previousState;
        
        /*
        * Initializes a new instance of the SokobanPuzzle class with the provided initial state.
        *
        * Parameters:
        *   initialState: The initial state of the puzzle represented as a 2D array of TileType values.
        */
        public SokobanPuzzle(TileType[,] initialState)
        {
            _state = (TileType[,])initialState.Clone();
        }

        public SokobanPuzzle(TileType[,] initialState, SokobanPuzzle previousState = null)
        {
            _state = (TileType[,])initialState.Clone();
            _previousState = previousState;
        }

        /*
        * Determines whether the puzzle is solved.
        *
        * Returns:
        *   True if the puzzle is solved (all goal tiles are covered by boxes), otherwise false.
        */
        public bool IsSolved()
        {
            foreach (var tile in _state)
            {
                if (tile == TileType.Goal || tile == TileType.PlayerGoal)
                    return false;
            }
            return true;
        }
        
        /*
        * Retrieves a list of possible moves from the current puzzle state. The list is ordered such that badMoves are
        * placed at the back, and so tried last.
        *
        * Returns:
        *   An IEnumerable collection of SokobanPuzzle instances representing possible moves from the current state.
        */
        public IEnumerable<SokobanPuzzle> GetPossibleMoves()
        {
            List<SokobanPuzzle> possibleMoves = new List<SokobanPuzzle>();
            List<SokobanPuzzle> badMoves = new List<SokobanPuzzle>();
            int[,] directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } }; // Up, Down, Left, Right
            
            for (int i = 0; i < _state.GetLength(0); i++)
            {
                for (int j = 0; j < _state.GetLength(1); j++)
                {
                    if (_state[i, j] == TileType.Player)
                    {
                        for (int k = 0; k < directions.GetLength(0); k++)
                        {
                            int newX = i + directions[k, 0];
                            int newY = j + directions[k, 1];

                            if (IsValidMove(newX, newY, (Direction)k))
                            {
                                SokobanPuzzle newMove = MovePlayer(i, j, newX, newY);
                                if (IsBadMove(newX, newY, (Direction)k))
                                {
                                    badMoves.Add(newMove);
                                }
                                else
                                {
                                    possibleMoves.Add(newMove);
                                }
                            }
                        }

                        break;
                    }
                }
            }
            
            possibleMoves.AddRange(badMoves);
            return possibleMoves;
        }

        /*
        * Determines whether a move to the specified position in the given direction is valid. It is invalid if the
        * move is impossible, or if the move would result in pushing a box into a non-goal corner (a failed state).
        *
        * Parameters:
        *   row: The row index of the position to move to.
        *   col: The column index of the position to move to.
        *   direction: The direction of the move (Up, Down, Left, Right).
        *
        * Returns:
        *   True if the move is valid, otherwise false.
        */
        private bool IsValidMove(int row, int col, Direction direction)
        {
            if (row < 0 || row >= _state.GetLength(0) - 1 || col < 0 || col >= _state.GetLength(1) - 1)
                return false;

            TileType currentTile = _state[row, col];

            if (currentTile == TileType.Wall)
                return false;

            if (currentTile == TileType.Box || currentTile == TileType.BoxGoal)
            {
                int behindRow = row + (direction == Direction.Down ? 1 : direction == Direction.Up ? -1 : 0);
                int behindCol = col + (direction == Direction.Right ? 1 : direction == Direction.Left ? -1 : 0);

                TileType behindTile = _state[behindRow, behindCol];

                if (behindTile == TileType.Wall || behindTile == TileType.Box || behindTile == TileType.BoxGoal)
                    return false;

                if (_state[behindRow, behindCol] != TileType.Goal)
                {
                    bool frontBackWall = _state[behindRow + 1, behindCol] == TileType.Wall || 
                                         _state[behindRow - 1, behindCol] == TileType.Wall;
                
                    bool leftRightWall = _state[behindRow, behindCol + 1] == TileType.Wall || 
                                         _state[behindRow, behindCol - 1] == TileType.Wall;

                    return !(frontBackWall && leftRightWall);
                }
            }

            return true;
        }

        /*
         * Determines whether the move being made is 'bad' or not. Currently, moving a box off of a goal is a bad move.
         *
         * Parameters:
         *  row: The row index of the position to move to.
         *  col: The column index of the position to move to.
         *  direction: The direction of the move (Up, Down, Left, Right).
         *
         * Returns:
         *  True if the move is bad, false if not.
         */
        private bool IsBadMove(int row, int col, Direction direction)
        {
            TileType currentTile = _state[row, col];

            if (currentTile == TileType.BoxGoal)
            {
                return true;
            }

            return false;
        }

        /*
        * Moves the player from one position to another in the puzzle state.
        *
        * Parameters:
        *   fromRow: The row index of the current position of the player.
        *   fromCol: The column index of the current position of the player.
        *   toRow: The row index of the destination position.
        *   toCol: The column index of the destination position.
        *
        * Returns:
        *   A new SokobanPuzzle instance representing the puzzle state after the player has been moved.
        */
        private SokobanPuzzle MovePlayer(int fromRow, int fromCol, int toRow, int toCol)
        {
            TileType[,] newState = (TileType[,])_state.Clone();
            
            bool isGoalSpace = newState[toRow, toCol] == TileType.Goal || newState[toRow, toCol] == TileType.BoxGoal;
            
            // If the destination space contains a box or a box on a goal, move the box accordingly
            if (newState[toRow, toCol] == TileType.Box || newState[toRow, toCol] == TileType.BoxGoal)
            {
                int boxToRow = toRow + (toRow - fromRow);
                int boxToCol = toCol + (toCol - fromCol);

                newState[boxToRow, boxToCol] = newState[boxToRow, boxToCol] == TileType.Goal ? 
                    TileType.BoxGoal : TileType.Box;
                newState[toRow, toCol] = newState[toRow, toCol] == TileType.BoxGoal ? 
                    TileType.Goal : TileType.Empty;
            }
            
            // Move the player to the destination space
            newState[toRow, toCol] = isGoalSpace ? TileType.PlayerGoal : TileType.Player;
            
            // Update the source space
            newState[fromRow, fromCol] = newState[fromRow, fromCol] == TileType.PlayerGoal ? 
                TileType.Goal : TileType.Empty;
            
            SokobanPuzzle newPuzzle = new SokobanPuzzle(newState, this);   
            
            return newPuzzle;
        }

        /*
         * Converts a puzzle object into a string
         */
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _state.GetLength(0); i++)
            {
                for (int j = 0; j < _state.GetLength(1); j++)
                {
                    switch (_state[i, j])
                    {
                        case TileType.Wall:
                            sb.Append('X');
                            break;
                        case TileType.Empty:
                            sb.Append(' ');
                            break;
                        case TileType.Player:
                            sb.Append('P');
                            break;
                        case TileType.PlayerGoal:
                            sb.Append('O');
                            break;
                        case TileType.Box:
                            sb.Append('B');
                            break;
                        case TileType.BoxGoal:
                            sb.Append('H');
                            break;
                        case TileType.Goal:
                            sb.Append('G');
                            break;
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}