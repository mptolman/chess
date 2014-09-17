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

            // Populate a list of all possible moves
            var allMoves = GetAllMoves(board, myColor);

            if (allMoves.Count == 0)
            {
                // No moves available
                // Flag as stalemate
                nextMove = new ChessMove(null, null);
                nextMove.Flag = ChessFlag.Stalemate;
            }
            else
            {
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
        private ICollection<ChessMove> GetAllMoves(ChessBoard board, ChessColor myColor)
        {
            ICollection<ChessMove> allMoves = new List<ChessMove>();

            // Cycle through the board and generate moves for each of our pieces
            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
            {
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                {
                    var chessPiece = board[x, y];

                    if (chessPiece == ChessPiece.Empty) continue; // Ignore empty tiles
                    if (_pieceColor[chessPiece] != myColor) continue; // Ignore opponent's pieces

                    switch (chessPiece)
                    {
                        case ChessPiece.WhiteBishop:
                        case ChessPiece.BlackBishop:
                            AddBishopMoves(board, myColor, x, y, allMoves);
                            break;
                        case ChessPiece.WhiteKing:
                        case ChessPiece.BlackKing:
                            AddKingMoves(board, myColor, x, y, allMoves);
                            break;
                        case ChessPiece.WhiteKnight:
                        case ChessPiece.BlackKnight:
                            AddKnightMoves(board, myColor, x, y, allMoves);
                            break;
                        case ChessPiece.WhitePawn:
                        case ChessPiece.BlackPawn:
                            AddPawnMoves(board, myColor, x, y, allMoves);
                            break;
                        case ChessPiece.WhiteQueen:
                        case ChessPiece.BlackQueen:
                            AddQueenMoves(board, myColor, x, y, allMoves);
                            break;
                        case ChessPiece.WhiteRook:
                        case ChessPiece.BlackRook:
                            AddRookMoves(board, myColor, x, y, allMoves);
                            break;
                        default:
                            break;
                    }
                }
            }

            return allMoves;
        }

        /// <summary>
        /// Adds moves for a Bishop to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddBishopMoves(ChessBoard board, ChessColor myColor, int x, int y, ICollection<ChessMove> moves)
        {

        }

        /// <summary>
        /// Adds moves for a King to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddKingMoves(ChessBoard board, ChessColor myColor, int x, int y, ICollection<ChessMove> moves)
        {

        }

        /// <summary>
        /// Adds moves for a Knight to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddKnightMoves(ChessBoard board, ChessColor myColor, int x, int y, ICollection<ChessMove> moves)
        {

        }

        /// <summary>
        /// Adds moves for a Pawn to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddPawnMoves(ChessBoard board, ChessColor myColor, int x, int y, ICollection<ChessMove> moves)
        {
            
        }

        /// <summary>
        /// Adds moves for a Queen to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddQueenMoves(ChessBoard board, ChessColor myColor, int x, int y, ICollection<ChessMove> moves)
        {

        }

        /// <summary>
        /// Adds moves for a Rook to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddRookMoves(ChessBoard board, ChessColor myColor, int x, int y, ICollection<ChessMove> moves)
        {

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
