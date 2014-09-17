using System;
using System.Collections.Generic;
using System.Text;
using UvsChess;

namespace StudentAI
{
    public class StudentAI : IChessAI
    {        
        /// <summary>
        /// Utility dictionary for determing the color of a piece.
        /// We could also use a lookup method, but I thought a dictionary would be slightly more efficient.
        /// </summary>
        private static IDictionary<ChessPiece, ChessColor> _pieceColor = new Dictionary<ChessPiece, ChessColor>
        {
            {ChessPiece.BlackBishop,    ChessColor.Black},
            {ChessPiece.BlackKing,      ChessColor.Black},
            {ChessPiece.BlackKnight,    ChessColor.Black},
            {ChessPiece.BlackPawn,      ChessColor.Black},
            {ChessPiece.BlackQueen,     ChessColor.Black},
            {ChessPiece.BlackRook,      ChessColor.Black},
            {ChessPiece.WhiteBishop,    ChessColor.White},
            {ChessPiece.WhiteKing,      ChessColor.White},
            {ChessPiece.WhiteKnight,    ChessColor.White},
            {ChessPiece.WhitePawn,      ChessColor.White},
            {ChessPiece.WhiteQueen,     ChessColor.White},
            {ChessPiece.WhiteRook,      ChessColor.White}
        };

        #region IChessAI Members that are implemented by the Student

        /// <summary>
        /// The name of your AI
        /// </summary>
        public string Name
        {
#if DEBUG
            get { return "StudentAI (Debug)"; }
#else
            get { return "StudentAI"; }
#endif
        }

        /// <summary>
        /// Evaluates the chess board and decides which move to make. This is the main method of the AI.
        /// The framework will call this method when it's your turn.
        /// </summary>
        /// <param name="board">Current chess board</param>
        /// <param name="yourColor">Your color</param>
        /// <returns>Returns the best chess move the player has for the given chess board</returns>
        public ChessMove GetNextMove(ChessBoard board, ChessColor myColor)
        {
            ChessMove nextMove = null;

            IList<ChessMove> allMoves = GetAllMoves(board, myColor);

            if (allMoves.Count == 0)
            {
                // No moves available
                // Flag as stalemate
                nextMove = new ChessMove(null, null);
                nextMove.Flag = ChessFlag.Stalemate;
            }
            else
            {
                // Random selection
                Random random = new Random();
                int index = random.Next(allMoves.Count);
                nextMove = allMoves[index];

                // TODO : Use algorithm for selecting move
                // 1) Random
                // 2) Greedy
                // 3) Minimax
                // Maybe we should break these out into separate DLL's so they can be selected on demand from the GUI.
                // We'll share the move generator between DLL's so we can keep common code centralized in one place.
            }

            return nextMove;
        }

        /// <summary>
        /// Validates a move. The framework uses this to validate the opponents move.
        /// </summary>
        /// <param name="boardBeforeMove">The board as it currently is _before_ the move.</param>
        /// <param name="moveToCheck">This is the move that needs to be checked to see if it's valid.</param>
        /// <param name="colorOfPlayerMoving">This is the color of the player who's making the move.</param>
        /// <returns>Returns true if the move was valid</returns>
        public bool IsValidMove(ChessBoard boardBeforeMove, ChessMove moveToCheck, ChessColor colorOfPlayerMoving)
        {
            // TODO : Implement move validator
            // One option is to call our move generator using their chess piece, and if this move is in our collection of valid moves, then it's valid. Otherwise, invalid.

            // Special situations to consider:
            // Be sure to check that appropriate flags are set; e.g., if opponent puts our King in check and doesn't set ChessFlag.Check, it's invalid!
            // Opponent may not put their own King in check.

            // Just return true for now, so we can perform testing before this is built.
            return true;
        }

        #endregion

        #region Private utility functions

        /// <summary>
        /// Populates a list of all possible (VALID) moves.
        /// </summary>
        /// <param name="board">Current chess board</param>
        /// <param name="myColor">Your color</param>
        /// <returns>Returns a collection of all possible moves</returns>
        private IList<ChessMove> GetAllMoves(ChessBoard board, ChessColor myColor)
        {
            IList<ChessMove> allMoves = new List<ChessMove>();

            // Cycle through the board and generate moves for each of our pieces
            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
            {
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                {
                    var chessPiece = board[x, y];

                    if (chessPiece == ChessPiece.Empty) continue; // Ignore empty tiles
                    if (_pieceColor[chessPiece] != myColor) continue; // Ignore opponent's pieces

                    AddMovesForThisPiece(board, myColor, chessPiece, x, y, allMoves);
                }
            }

            return allMoves;
        }

        /// <summary>
        /// Add to a collection of moves for the piece at this location
        /// This will come in handy when we need to generate oppositional moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="piece">The piece we're evaluating</param>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <param name="moves">Collection to add moves to</param>
        private void AddMovesForThisPiece(ChessBoard board, ChessColor myColor, ChessPiece piece, int x, int y, IList<ChessMove> moves)
        {
            switch (piece)
            {
                case ChessPiece.WhiteBishop:
                case ChessPiece.BlackBishop:
                    AddBishopMoves(board, myColor, x, y, moves);
                    break;
                case ChessPiece.WhiteKing:
                case ChessPiece.BlackKing:
                    AddKingMoves(board, myColor, x, y, moves);
                    break;
                case ChessPiece.WhiteKnight:
                case ChessPiece.BlackKnight:
                    AddKnightMoves(board, myColor, x, y, moves);
                    break;
                case ChessPiece.WhitePawn:
                case ChessPiece.BlackPawn:
                    AddPawnMoves(board, myColor, x, y, moves);
                    break;
                case ChessPiece.WhiteQueen:
                case ChessPiece.BlackQueen:
                    AddQueenMoves(board, myColor, x, y, moves);
                    break;
                case ChessPiece.WhiteRook:
                case ChessPiece.BlackRook:
                    AddRookMoves(board, myColor, x, y, moves);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Adds moves for a Bishop to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddBishopMoves(ChessBoard board, ChessColor myColor, int x, int y, IList<ChessMove> moves)
        {

        }

        /// <summary>
        /// Adds moves for a King to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddKingMoves(ChessBoard board, ChessColor myColor, int x, int y, IList<ChessMove> moves)
        {

        }

        /// <summary>
        /// Adds moves for a Knight to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddKnightMoves(ChessBoard board, ChessColor myColor, int x, int y, IList<ChessMove> moves)
        {

        }

        /// <summary>
        /// Adds moves for a Pawn to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddPawnMoves(ChessBoard board, ChessColor myColor, int x, int y, IList<ChessMove> moves)
        {
            if (myColor == ChessColor.Black)
            {
                // Move forward 2 spaces from starting position
                if (y == 1 &&
                    board[x, y + 1] == ChessPiece.Empty &&
                    board[x, y + 2] == ChessPiece.Empty)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(x, y + 2)));

                // Move forward 1 space
                if (InBounds(x, y + 1) && board[x, y + 1] == ChessPiece.Empty)                    
                    moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(x, y + 1)));

                // Diagonal attack (forward-left)
                if (InBounds(x + 1, y + 1))
                {
                    int newX = x + 1;
                    int newY = y + 1;
                    var pieceInNewPos = board[newX, newY];

                    if (pieceInNewPos != ChessPiece.Empty && _pieceColor[pieceInNewPos] == ChessColor.White)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                }

                // Diagonal attack (forward-right)
                if (InBounds(x - 1, y + 1))
                {
                    int newX = x - 1;
                    int newY = y + 1;
                    var pieceInNewPos = board[newX, newY];

                    if (pieceInNewPos != ChessPiece.Empty && _pieceColor[pieceInNewPos] == ChessColor.White)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                }
            }
            else // White pawn
            {
                // Move forward 2 spaces from starting position
                if (y == ChessBoard.NumberOfRows - 2 &&
                    board[x, y - 1] == ChessPiece.Empty && 
                    board[x, y - 2] == ChessPiece.Empty)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(x, y - 2)));

                // Move forward 1 space
                if (InBounds(x, y - 1) && board[x, y - 1] == ChessPiece.Empty)
                    moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(x, y - 1)));

                // Diagonal attack (forward-left)
                if (InBounds(x - 1, y - 1))
                {
                    int newX = x - 1;
                    int newY = y - 1;
                    var pieceInNewPos = board[newX, newY];

                    if (pieceInNewPos != ChessPiece.Empty && _pieceColor[pieceInNewPos] == ChessColor.Black)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                }

                // Diagonal attack (forward-right)
                if (InBounds(x + 1, y - 1))
                {
                    int newX = x + 1;
                    int newY = y - 1;
                    var pieceInNewPos = board[newX, newY];

                    if (pieceInNewPos != ChessPiece.Empty && _pieceColor[pieceInNewPos] == ChessColor.Black)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                }
            }
        }

        /// <summary>
        /// Adds moves for a Queen to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddQueenMoves(ChessBoard board, ChessColor myColor, int x, int y, IList<ChessMove> moves)
        {

        }

        /// <summary>
        /// Adds moves for a Rook to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddRookMoves(ChessBoard board, ChessColor myColor, int x, int y, IList<ChessMove> moves)
        {
            const int LEFT  = 1;
            const int RIGHT = 2;
            const int UP = 4;
            const int DOWN = 8;

            int flags = LEFT + RIGHT + UP + DOWN;

            for (int i = 1; ; i++)
            {              
                // Try left
                if ((flags & LEFT) > 0)
                {
                    int newX = x - i;
                    int newY = y;

                    if (!InBounds(newX, newY))
                        flags -= LEFT;
                    else if (board[newX, newY] == ChessPiece.Empty)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                    else if (_pieceColor[board[newX, newY]] != myColor)
                    {
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                        flags -= LEFT;
                    }
                }

                // Try right
                if ((flags & RIGHT) > 0)
                {
                    int newX = x + i;
                    int newY = y;

                    if (!InBounds(newX, newY))
                        flags -= RIGHT;
                    else if (board[newX, newY] == ChessPiece.Empty)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                    else if (_pieceColor[board[newX, newY]] != myColor)
                    {
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                        flags -= RIGHT;
                    }
                }

                // Try up
                if ((flags & UP) > 0)
                {
                    int newX = x;
                    int newY = y - i;

                    if (!InBounds(newX, newY))
                        flags -= UP;
                    else if (board[newX, newY] == ChessPiece.Empty)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                    else if (_pieceColor[board[newX, newY]] != myColor)
                    {
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                        flags -= UP;
                    }
                }

                // Try down
                if ((flags & DOWN) > 0)
                {
                    int newX = x;
                    int newY = y + i;

                    if (!InBounds(newX, newY))
                        flags -= DOWN;
                    else if (board[newX, newY] == ChessPiece.Empty)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                    else if (_pieceColor[board[newX, newY]] != myColor)
                    {
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                        flags -= DOWN;
                    }
                }

                if (flags == 0)
                    break;
            }
        }

        /// <summary>
        /// Check if coordinates are on the board
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <returns>Returns true if on board; else false</returns>
        private bool InBounds(int x, int y)
        {
            return (x >= 0 && x < ChessBoard.NumberOfColumns) && (y >= 0 && y < ChessBoard.NumberOfRows);
        }

        #endregion



        #region IChessAI Members that should be implemented as automatic properties and should NEVER be touched by students.
        /// <summary>
        /// This will return false when the framework starts running your AI. When the AI's time has run out,
        /// then this method will return true. Once this method returns true, your AI should return a 
        /// move immediately.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        public AIIsMyTurnOverCallback IsMyTurnOver { get; set; }

        /// <summary>
        /// Call this method to print out debug information. The framework subscribes to this event
        /// and will provide a log window for your debug messages.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="message"></param>
        public AILoggerCallback Log { get; set; }

        /// <summary>
        /// Call this method to catch profiling information. The framework subscribes to this event
        /// and will print out the profiling stats in your log window.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="key"></param>
        public AIProfiler Profiler { get; set; }

        /// <summary>
        /// Call this method to tell the framework what decision print out debug information. The framework subscribes to this event
        /// and will provide a debug window for your decision tree.
        /// 
        /// You should NEVER EVER set this property!
        /// This property should be defined as an Automatic Property.
        /// This property SHOULD NOT CONTAIN ANY CODE!!!
        /// </summary>
        /// <param name="message"></param>
        public AISetDecisionTreeCallback SetDecisionTree { get; set; }
        #endregion
    }
}
