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
            get { return "KnottyButchersAI (Debug)"; }
#else
            get { return "KnottyButchersAI"; }
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
                // What do we do now?
                // Maybe see if the opponent has any moves. If not, then stalemate
                nextMove = new ChessMove(null, null);
            }
            else
            {
                // Random selection
                Random random = new Random();
                int index = random.Next(allMoves.Count);
                nextMove = allMoves[index];
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
            // Get a list of all available moves the opponent had
            // If we find their move in the moves we generate, then assume it's valid
            var allOpponentMoves = GetAllMoves(boardBeforeMove, colorOfPlayerMoving);
            foreach (var move in allOpponentMoves)
                if (move == moveToCheck)
                    return true;

            // Change the below return value to "false" after our move generation is finished!
            return true;
        }

        #endregion

        #region Move generation methods

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
            var offset = new int[] { 1, -1 };

            //permutate through every diagonal the piece can move in (4 diagonals)
            foreach(int xOffset in offset)
            {
                foreach(int yOffset in offset)
                {
                    int newX = x + xOffset;
                    int newY = y + yOffset;

                    //continue in that diagonal until we fall out of bounds
                    while(InBounds(newX, newY))
                    {
                        var pieceAtNewPos = board[newX, newY];

                        //add the piece as a potential move if it is empty or if it is occupied by an opponents piece
                        if(pieceAtNewPos == ChessPiece.Empty || _pieceColor[pieceAtNewPos] != myColor)
                            moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                        else //otherwise, we find that one of our own pieces is in the way, so we will stop looking in that diagonal's direction
                            break;

                        newX += xOffset;
                        newY += yOffset;
                    }
                }
            }
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
            var offsets1 = new int[] { -1, 1 };
            var offsets2 = new int[] { -2, 2 };

            // Try permutations where we move 1 space X and 2 spaces Y
            foreach (int xOffset in offsets1)
            {
                foreach (int yOffset in offsets2)
                {
                    int newX = x + xOffset;
                    int newY = y + yOffset;

                    if (!InBounds(newX, newY))
                        continue;

                    var pieceAtNewPos = board[newX, newY];
                    if (pieceAtNewPos == ChessPiece.Empty || _pieceColor[pieceAtNewPos] != myColor)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                }
            }

            // Try permutations where we move 2 spaces X and 1 space Y
            foreach (int xOffset in offsets2)
            {
                foreach (int yOffset in offsets1)
                {
                    int newX = x + xOffset;
                    int newY = y + yOffset;
                    
                    if (!InBounds(newX, newY))
                        continue;

                    var pieceAtNewPos = board[newX, newY];
                    if (pieceAtNewPos == ChessPiece.Empty || _pieceColor[pieceAtNewPos] != myColor)
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                }
            }
        }

        /// <summary>
        /// Adds moves for a Pawn to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddPawnMoves(ChessBoard board, ChessColor myColor, int x, int y, IList<ChessMove> moves)
        {
            int forwardDirection = myColor == ChessColor.Black ? 1 : -1;
            int startingRow = myColor == ChessColor.Black ? 1 : ChessBoard.NumberOfRows - 2;

            int newX, newY;

            // Move forward 2 spaces from starting position
            if (y == startingRow &&
                board[x, y + forwardDirection] == ChessPiece.Empty &&
                board[x, y + forwardDirection * 2] == ChessPiece.Empty)
                moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(x, y + forwardDirection * 2)));

            // Move forward 1 space if it's empty
            if (InBounds(x, y + forwardDirection) && board[x, y + forwardDirection] == ChessPiece.Empty)
                moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(x, y + forwardDirection)));

            // Diagonal attack (forward-left)
            newX = x - 1;
            newY = y + forwardDirection;

            if (InBounds(newX, newY) &&
                board[newX, newY] != ChessPiece.Empty &&
                _pieceColor[board[newX, newY]] != myColor)
                moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));

            // Diagonal attack (forward-right)
            newX = x + 1;
            newY = y + forwardDirection;

            if (InBounds(newX, newY) &&
                board[newX, newY] != ChessPiece.Empty &&
                _pieceColor[board[newX, newY]] != myColor)
                moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
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
            const int UP    = 4;
            const int DOWN  = 8;

            int flags = LEFT | RIGHT | UP | DOWN;

            // Search outward from our current location, one square distance at a time
            for (int distance = 1; flags > 0; ++distance)
            {
                // Try left
                if ((flags & LEFT) > 0)
                {
                    int newX = x - distance;
                    int newY = y;

                    if (!InBounds(newX, newY))
                        flags -= LEFT;
                    else if (board[newX, newY] == ChessPiece.Empty) // Move to an empty space                        
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                    else // We've hit a piece on the board
                    {
                       if (_pieceColor[board[newX, newY]] != myColor) // Capture an opponent's piece
                            moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                        flags -= LEFT;
                    }
                }

                // Try right
                if ((flags & RIGHT) > 0)
                {
                    int newX = x + distance;
                    int newY = y;

                    if (!InBounds(newX, newY))
                        flags -= RIGHT;
                    else if (board[newX, newY] == ChessPiece.Empty) // Move to an empty space                       
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                    else // We've hit a piece on the board
                    {
                        if (_pieceColor[board[newX, newY]] != myColor) // Capture an opponent's piece
                            moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                        flags -= RIGHT;
                    }
                }

                // Try up
                if ((flags & UP) > 0)
                {
                    int newX = x;
                    int newY = y - distance;

                    if (!InBounds(newX, newY))
                        flags -= UP;
                    else if (board[newX, newY] == ChessPiece.Empty) // Move to an empty space                       
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                    else // We've hit a piece on the board
                    {
                        if (_pieceColor[board[newX, newY]] != myColor) // Capture an opponent's piece
                            moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                        flags -= UP;
                    }
                }

                // Try down
                if ((flags & DOWN) > 0)
                {
                    int newX = x;
                    int newY = y + distance;

                    if (!InBounds(newX, newY))
                        flags -= DOWN;
                    else if (board[newX, newY] == ChessPiece.Empty) // Move to an empty space                       
                        moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                    else // We've hit a piece on the board
                    {
                        if (_pieceColor[board[newX, newY]] != myColor) // Capture an opponent's piece
                            moves.Add(new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)));
                        flags -= DOWN;
                    }
                }
            }
        }

        #endregion

        #region Utility methods

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
