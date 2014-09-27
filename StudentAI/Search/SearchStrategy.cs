using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;
using StudentAI.Heuristics;

namespace StudentAI.Search
{
    internal abstract class SearchStrategy
    {
        private const int MAX_RECENT_MOVES = 5;
        private int _recentIndex = 0;

        /// <summary>
        /// This is used in AddThisMove to avoid infinite recursion.
        /// </summary>
        private bool _searchingForCheckmate = false;

        protected IChessAI _ai;
        protected IHeuristic _heuristic;
        protected ChessMove[] _recentMoves;

        /// <summary>
        /// Implemented by subclasses to select which move to make given all possible moves
        /// </summary>
        /// <param name="board">Current board state</param>
        /// <param name="myColor">Which color we're selecting a move for</param>
        /// <param name="moves">The list of available moves to choose from</param>
        /// <returns>ChessMove to make</returns>
        protected abstract ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves);
        
        public SearchStrategy(IChessAI ai, IHeuristic heuristic)
        {
            _ai = ai;
            _heuristic = heuristic;
            _recentMoves = new ChessMove[MAX_RECENT_MOVES];
        }

        /// <summary>
        /// Get the next best move to make
        /// Defers to subclasses to select the move
        /// </summary>
        /// <param name="board">Current board state</param>
        /// <param name="myColor">Which color we're selecting a move for</param>
        /// <returns>ChessMove to make</returns>
        public ChessMove GetNextMove(ChessBoard board, ChessColor myColor)
        {
            ChessMove nextMove = null;

            var allMoves = GetAllMoves(board, myColor);

            if (allMoves.Count == 0)
            {
                // This must be stalemate.
                // Technically, we would need to make sure we're not in check before declaring stalemate,
                // but if we were in check, then the opponent would have flagged checkmate on their last move.
                // If they didn't set the checkmate flag, then our move validator would have caught their error.
                nextMove = new ChessMove(null, null);
                nextMove.Flag = ChessFlag.Stalemate;
            }
            else
            {
                nextMove = SelectFromAvailableMoves(board, myColor, allMoves);

                _recentIndex %= MAX_RECENT_MOVES;
                _recentMoves[_recentIndex++] = nextMove;
            }

            return nextMove;
        }

        /// <summary>
        /// Populates a list of all possible (VALID) moves.
        /// </summary>
        /// <param name="board">Current chess board</param>
        /// <param name="myColor">Your color</param>
        /// <returns>Returns a collection of all possible moves</returns>
        public IList<ChessMove> GetAllMoves(ChessBoard board, ChessColor myColor)
        {
            var allMoves = new List<ChessMove>();

            // Cycle through the board and generate moves for each of our pieces
            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
            {
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                {
                    var chessPiece = board[x, y];

                    if (chessPiece == ChessPiece.Empty) continue; // Ignore empty tiles
                    if (Utility.PieceColor(chessPiece) != myColor) continue; // Ignore opponent's pieces

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
            foreach (int xOffset in offset)
            {
                foreach (int yOffset in offset)
                {
                    int newX = x + xOffset;
                    int newY = y + yOffset;

                    //continue in that diagonal until we fall out of bounds
                    while (Utility.InBounds(newX, newY))
                    {
                        var pieceAtNewPos = board[newX, newY];

                        //add the piece as a potential move if it is empty
                        if (pieceAtNewPos == ChessPiece.Empty)
                        {
                            AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);
                        }
                        else
                        {
                            //if we've hit an opponent's piece then add that move
                            if (Utility.PieceColor(pieceAtNewPos) != myColor)
                                AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);
                            //we've hit another piece, so stop looking in this direction
                            break;
                        }

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
            int forwardDirection = Utility.ForwardDirection(myColor);
            int rightDirection = Utility.RightDirection(myColor);

            Action<int, int> tryThisDirection = (int newX, int newY) =>
            {
                if (!Utility.InBounds(newX, newY))
                    return;

                if (board[newX, newY] == ChessPiece.Empty || Utility.PieceColor(board[newX, newY]) != myColor)
                    AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);
            };

            // Try forward
            tryThisDirection(x, y + forwardDirection);

            // Try backward
            tryThisDirection(x, y - forwardDirection);

            // Try left
            tryThisDirection(x - rightDirection, y);

            // Try right
            tryThisDirection(x + rightDirection, y);

            // Try forward-left
            tryThisDirection(x - rightDirection, y + forwardDirection);

            // Try forward-right
            tryThisDirection(x + rightDirection, y + forwardDirection);

            // Try back-left
            tryThisDirection(x - rightDirection, y - forwardDirection);

            // Try back-right
            tryThisDirection(x + rightDirection, y - forwardDirection);
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

                    if (!Utility.InBounds(newX, newY))
                        continue;

                    var pieceAtNewPos = board[newX, newY];
                    if (pieceAtNewPos == ChessPiece.Empty || Utility.PieceColor(pieceAtNewPos) != myColor)
                        AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);
                }
            }

            // Try permutations where we move 2 spaces X and 1 space Y
            foreach (int xOffset in offsets2)
            {
                foreach (int yOffset in offsets1)
                {
                    int newX = x + xOffset;
                    int newY = y + yOffset;

                    if (!Utility.InBounds(newX, newY))
                        continue;

                    var pieceAtNewPos = board[newX, newY];
                    if (pieceAtNewPos == ChessPiece.Empty || Utility.PieceColor(pieceAtNewPos) != myColor)
                        AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);
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
            int forwardDirection = Utility.ForwardDirection(myColor);
            int startingRow = myColor == ChessColor.Black ? 1 : ChessBoard.NumberOfRows - 2;

            int newX, newY;

            // Move forward 2 spaces from starting position
            if (y == startingRow &&
                board[x, y + forwardDirection] == ChessPiece.Empty &&
                board[x, y + forwardDirection * 2] == ChessPiece.Empty)
                AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(x, y + forwardDirection * 2)), moves);

            // Move forward 1 space if it's empty
            if (Utility.InBounds(x, y + forwardDirection) && board[x, y + forwardDirection] == ChessPiece.Empty)
                AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(x, y + forwardDirection)), moves);

            // Diagonal attack (forward-left)
            newX = x - 1;
            newY = y + forwardDirection;

            if (Utility.InBounds(newX, newY) &&
                board[newX, newY] != ChessPiece.Empty &&
                Utility.PieceColor(board[newX, newY]) != myColor)
                AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);

            // Diagonal attack (forward-right)
            newX = x + 1;
            newY = y + forwardDirection;

            if (Utility.InBounds(newX, newY) &&
                board[newX, newY] != ChessPiece.Empty &&
                Utility.PieceColor(board[newX, newY]) != myColor)
                AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);
        }

        /// <summary>
        /// Adds moves for a Queen to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddQueenMoves(ChessBoard board, ChessColor myColor, int x, int y, IList<ChessMove> moves)
        {
            const int LEFT = 1;
            const int RIGHT = 2;
            const int UP = 4;
            const int DOWN = 8;
            const int LEFT_UP = 16;
            const int LEFT_DOWN = 32;
            const int RIGHT_UP = 64;
            const int RIGHT_DOWN = 128;

            int flags = LEFT | RIGHT | UP | DOWN | LEFT_UP | LEFT_DOWN | RIGHT_UP | RIGHT_DOWN;

            Action<int, int, int> tryThisDirection = (int newX, int newY, int direction) =>
            {
                if (!Utility.InBounds(newX, newY))
                {
                    flags -= direction;
                    return;
                }

                var piece = board[newX, newY];
                if (piece == ChessPiece.Empty)
                {
                    AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);
                }
                else
                {
                    if (Utility.PieceColor(piece) != myColor)
                        AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);
                    flags -= direction;
                }
            };

            // Search outward from our current location, one square distance at a time
            for (int distance = 1; flags > 0; ++distance)
            {
                // Try left
                if ((flags & LEFT) > 0)
                    tryThisDirection(x - distance, y, LEFT);

                // Try right
                if ((flags & RIGHT) > 0)
                    tryThisDirection(x + distance, y, RIGHT);

                // Try up
                if ((flags & UP) > 0)
                    tryThisDirection(x, y + distance, UP);

                // Try down
                if ((flags & DOWN) > 0)
                    tryThisDirection(x, y - distance, DOWN);

                // Try left up
                if ((flags & LEFT_UP) > 0)
                    tryThisDirection(x - distance, y - distance, LEFT_UP);

                // Try left down
                if ((flags & LEFT_DOWN) > 0)
                    tryThisDirection(x - distance, y + distance, LEFT_DOWN);

                // Try right up
                if ((flags & RIGHT_UP) > 0)
                    tryThisDirection(x + distance, y - distance, RIGHT_UP);

                // Try right down
                if ((flags & RIGHT_DOWN) > 0)
                    tryThisDirection(x + distance, y + distance, RIGHT_DOWN);
            }
        }

        /// <summary>
        /// Adds moves for a Rook to our collection of available moves
        /// </summary>
        /// <param name="board">Current board</param>
        /// <param name="myColor">Your color</param>
        /// <param name="moves">The collection of moves we're adding to</param>
        private void AddRookMoves(ChessBoard board, ChessColor myColor, int x, int y, IList<ChessMove> moves)
        {
            const int LEFT = 1;
            const int RIGHT = 2;
            const int UP = 4;
            const int DOWN = 8;

            int flags = LEFT | RIGHT | UP | DOWN;

            Action<int, int, int> tryThisDirection = (int newX, int newY, int direction) =>
            {
                if (!Utility.InBounds(newX, newY))
                {
                    flags -= direction;
                    return;
                }

                var piece = board[newX, newY];
                if (piece == ChessPiece.Empty)
                {
                    AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);
                }
                else
                {
                    if (Utility.PieceColor(piece) != myColor)
                        AddThisMove(board, myColor, new ChessMove(new ChessLocation(x, y), new ChessLocation(newX, newY)), moves);
                    flags -= direction;
                }
            };

            for (int distance = 1; flags > 0; ++distance)
            {
                if ((flags & LEFT) > 0)
                    tryThisDirection(x - distance, y, LEFT);

                if ((flags & RIGHT) > 0)
                    tryThisDirection(x + distance, y, RIGHT);

                if ((flags & UP) > 0)
                    tryThisDirection(x, y - distance, UP);

                if ((flags & DOWN) > 0)
                    tryThisDirection(x, y + distance, DOWN);
            }
        }

        private void AddThisMove(ChessBoard board, ChessColor myColor, ChessMove move, IList<ChessMove> moves)
        {
            var oppColor = Utility.OppColor(myColor);
            var boardAfterMove = Utility.BoardAfterMove(board, move);

            // Cannot put ourselves in Check; skip this move
            if (Utility.InCheck(boardAfterMove, myColor))
                return;

            // See if this move will put opponent in Check or Checkmate.
            // Don't do this search if we're already in process of looking for Checkmate.
            // Otherwise, we'll end up in an infinite loop of recursion.
            if (!_searchingForCheckmate && Utility.InCheck(boardAfterMove, oppColor))
            {
                // Avoid infinite recursion
                _searchingForCheckmate = true;

                var oppMoves = GetAllMoves(boardAfterMove, oppColor);
                if (oppMoves.Count == 0)
                    // If the opponent has no available moves, then it's Checkmate
                    move.Flag = ChessFlag.Checkmate;
                else
                    // Otherwise, just Check
                    move.Flag = ChessFlag.Check;

                _searchingForCheckmate = false;
            }

            // Now add the move
            moves.Add(move);
        }
    }
}
