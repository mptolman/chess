using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    static class Utility
    {
        /// <summary>
        /// Convenience dictionary for determing the color of a piece.
        /// We could also use a lookup method, but I thought a dictionary would be slightly more efficient.
        /// </summary>
        public static IDictionary<ChessPiece, ChessColor> PieceColor = new Dictionary<ChessPiece, ChessColor>
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

        /// <summary>
        /// This helps us calculate forward/backward movements without having to branch based on color
        /// For example, to get the Y-coordinate for moving forward N spaces:
        ///     location.Y + ForwardDirection[myColor] * N
        /// And to move backwards N spaces:
        ///     location.Y - ForwardDirection[myColor] * N
        /// </summary>
        public static IDictionary<ChessColor, int> ForwardDirection = new Dictionary<ChessColor, int>
        {
            {ChessColor.Black, 1},
            {ChessColor.White, -1}
        };

        /// <summary>
        /// Check if coordinates are on the board
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <returns>Returns true if on board; else false</returns>
        public static bool InBounds(int x, int y)
        {
            return (x >= 0 && x < ChessBoard.NumberOfColumns) && (y >= 0 && y < ChessBoard.NumberOfRows);
        }

        /// <summary>
        /// Determine if `color` is in check given a board state
        /// </summary>
        /// <param name="board">Board we are evaluating</param>
        /// <param name="color">Color we are evaluating</param>
        /// <returns>Returns true if the King can be captured in this state; else false</returns>
        public static bool InCheck(ChessBoard board, ChessColor color)
        {
            ChessLocation kingLocation;
            var kingPiece = color == ChessColor.Black ? ChessPiece.BlackKing : ChessPiece.WhiteKing;

            if (FindPiece(board, kingPiece, out kingLocation))
                return CanBeCaptured(board, color, kingLocation);

            return false;
        }

        /// <summary>
        /// Determine if an opposing piece can capture this location
        /// </summary>
        /// <param name="board">Board we are evaluating</param>
        /// <param name="myColor">Color we are evaluating to see if its piece can be captured</param>
        /// <param name="location">Location of the piece we are evaluating</param>
        /// <returns>Returns true if opposing piece can reach this location; else false</returns>
        public static bool CanBeCaptured(ChessBoard board, ChessColor myColor, ChessLocation location)
        {
            const int FORWARD = 1 << 0;
            const int BACK = 1 << 1;
            const int LEFT = 1 << 2;
            const int RIGHT = 1 << 3;
            const int FORWARD_LEFT = 1 << 4;
            const int FORWARD_RIGHT = 1 << 5;
            const int BACK_LEFT = 1 << 6;
            const int BACK_RIGHT = 1 << 7;
            const int ALL_DIRECTIONS = (1 << 8) - 1;

            int flags = ALL_DIRECTIONS;

            for (int distance = 1; flags > 0; ++distance)
            {
                //------------------------------------
                // Check FORWARD direction
                //------------------------------------
                if ((flags & FORWARD) > 0)
                {
                    int newX = location.X;
                    int newY = location.Y + distance * ForwardDirection[myColor];

                    if (!InBounds(newX, newY))
                    {
                        // We're out of bounds; stop looking in this direction
                        flags -= FORWARD;
                    }
                    else
                    {
                        var pieceAtPos = board[newX, newY];
                        if (pieceAtPos != ChessPiece.Empty)
                        {
                            if (PieceColor[pieceAtPos] != myColor)
                            {
                                switch (pieceAtPos)
                                {
                                    case ChessPiece.BlackKing:
                                    case ChessPiece.WhiteKing:
                                        if (distance == 1)
                                            return true;
                                        break;
                                    case ChessPiece.BlackQueen:
                                    case ChessPiece.WhiteQueen:
                                    case ChessPiece.BlackRook:
                                    case ChessPiece.WhiteRook:
                                        return true;
                                    default:
                                        break;
                                }
                            }
                            // We run into a non-threatening piece; stop searching this direction
                            flags -= FORWARD;
                        }
                    }
                }

                //------------------------------------
                // Check BACK direction
                //------------------------------------
                if ((flags & BACK) > 0)
                {
                    int newX = location.X;
                    int newY = location.Y - distance * ForwardDirection[myColor];

                    if (!InBounds(newX, newY))
                    {
                        // We're out of bounds; stop looking in this direction
                        flags -= BACK;
                    }
                    else
                    {
                        var pieceAtPos = board[newX, newY];
                        if (pieceAtPos != ChessPiece.Empty)
                        {
                            if (PieceColor[pieceAtPos] != myColor)
                            {
                                switch (pieceAtPos)
                                {
                                    case ChessPiece.BlackKing:
                                    case ChessPiece.WhiteKing:
                                        if (distance == 1)
                                            return true;
                                        break;
                                    case ChessPiece.BlackQueen:
                                    case ChessPiece.WhiteQueen:
                                    case ChessPiece.BlackRook:
                                    case ChessPiece.WhiteRook:
                                        return true;
                                    default:
                                        break;
                                }
                            }
                            // We run into a non-threatening piece; stop searching this direction
                            flags -= BACK;
                        }
                    }
                }

                //------------------------------------
                // Check LEFT direction
                //------------------------------------
                if ((flags & LEFT) > 0)
                {
                    int newX = location.X - distance;
                    int newY = location.Y;

                    if (!InBounds(newX, newY))
                    {
                        // We're out of bounds; stop looking in this direction
                        flags -= LEFT;
                    }
                    else
                    {
                        var pieceAtPos = board[newX, newY];
                        if (pieceAtPos != ChessPiece.Empty)
                        {
                            if (PieceColor[pieceAtPos] != myColor)
                            {
                                switch (pieceAtPos)
                                {
                                    case ChessPiece.BlackKing:
                                    case ChessPiece.WhiteKing:
                                        if (distance == 1)
                                            return true;
                                        break;
                                    case ChessPiece.BlackQueen:
                                    case ChessPiece.WhiteQueen:
                                    case ChessPiece.BlackRook:
                                    case ChessPiece.WhiteRook:
                                        return true;
                                    default:
                                        break;
                                }
                            }
                            // We run into a non-threatening piece; stop searching this direction
                            flags -= LEFT;
                        }
                    }
                }

                //------------------------------------
                // Check RIGHT direction
                //------------------------------------
                if ((flags & RIGHT) > 0)
                {
                    int newX = location.X + distance;
                    int newY = location.Y;

                    if (!InBounds(newX, newY))
                    {
                        // We're out of bounds; stop looking in this direction
                        flags -= RIGHT;
                    }
                    else
                    {
                        var pieceAtPos = board[newX, newY];
                        if (pieceAtPos != ChessPiece.Empty)
                        {
                            if (PieceColor[pieceAtPos] != myColor)
                            {
                                switch (pieceAtPos)
                                {
                                    case ChessPiece.BlackKing:
                                    case ChessPiece.WhiteKing:
                                        if (distance == 1)
                                            return true;
                                        break;
                                    case ChessPiece.BlackQueen:
                                    case ChessPiece.WhiteQueen:
                                    case ChessPiece.BlackRook:
                                    case ChessPiece.WhiteRook:
                                        return true;
                                    default:
                                        break;
                                }
                            }
                            // We run into a non-threatening piece; stop searching this direction
                            flags -= RIGHT;
                        }
                    }
                }

                //------------------------------------
                // Check FORWARD_LEFT direction
                //------------------------------------
                if ((flags & FORWARD_LEFT) > 0)
                {
                    int newX = location.X - distance;
                    int newY = location.Y + distance * ForwardDirection[myColor];

                    if (!InBounds(newX, newY))
                    {
                        // We're out of bounds; stop looking in this direction
                        flags -= FORWARD_LEFT;
                    }
                    else
                    {
                        var pieceAtPos = board[newX, newY];
                        if (pieceAtPos != ChessPiece.Empty)
                        {
                            if (PieceColor[pieceAtPos] != myColor)
                            {
                                switch (pieceAtPos)
                                {
                                    case ChessPiece.BlackKing:
                                    case ChessPiece.WhiteKing:
                                    case ChessPiece.BlackPawn:
                                    case ChessPiece.WhitePawn:
                                        if (distance == 1)
                                            return true;
                                        break;
                                    case ChessPiece.BlackQueen:
                                    case ChessPiece.WhiteQueen:
                                    case ChessPiece.BlackBishop:
                                    case ChessPiece.WhiteBishop:
                                        return true;
                                    default:
                                        break;
                                }
                            }
                            // We run into a non-threatening piece; stop searching this direction
                            flags -= FORWARD_LEFT;
                        }
                    }
                }

                //------------------------------------
                // Check FORWARD_RIGHT direction
                //------------------------------------
                if ((flags & FORWARD_RIGHT) > 0)
                {
                    int newX = location.X + distance;
                    int newY = location.Y + distance * ForwardDirection[myColor];

                    if (!InBounds(newX, newY))
                    {
                        // We're out of bounds; stop searching this direction
                        flags -= FORWARD_RIGHT;
                    }
                    else
                    {
                        var pieceAtPos = board[newX, newY];
                        if (pieceAtPos != ChessPiece.Empty)
                        {
                            if (PieceColor[pieceAtPos] != myColor)
                            {
                                switch (pieceAtPos)
                                {
                                    case ChessPiece.BlackKing:
                                    case ChessPiece.WhiteKing:
                                    case ChessPiece.BlackPawn:
                                    case ChessPiece.WhitePawn:
                                        if (distance == 1)
                                            return true;
                                        break;
                                    case ChessPiece.BlackQueen:
                                    case ChessPiece.WhiteQueen:
                                    case ChessPiece.BlackBishop:
                                    case ChessPiece.WhiteBishop:
                                        return true;
                                    default:
                                        break;
                                }
                            }
                            // We run into a non-threatening piece; stop searching this direction
                            flags -= FORWARD_RIGHT;
                        }
                    }
                }

                //------------------------------------
                // Check BACK_LEFT direction
                //------------------------------------
                if ((flags & BACK_LEFT) > 0)
                {
                    int newX = location.X - distance;
                    int newY = location.Y - distance * ForwardDirection[myColor];

                    if (!InBounds(newX, newY))
                    {
                        // We're out of bounds; stop searching this direction
                        flags -= BACK_LEFT;
                    }
                    else
                    {
                        var pieceAtPos = board[newX, newY];
                        if (pieceAtPos != ChessPiece.Empty)
                        {
                            if (PieceColor[pieceAtPos] != myColor)
                            {
                                switch (pieceAtPos)
                                {
                                    case ChessPiece.BlackKing:
                                    case ChessPiece.WhiteKing:
                                        if (distance == 1)
                                            return true;
                                        break;
                                    case ChessPiece.BlackQueen:
                                    case ChessPiece.WhiteQueen:
                                    case ChessPiece.BlackBishop:
                                    case ChessPiece.WhiteBishop:
                                        return true;
                                    default:
                                        break;
                                }
                            }
                            // We run into a non-threatening piece; stop searching this direction
                            flags -= BACK_LEFT;
                        }
                    }
                }

                //------------------------------------
                // Check BACK_RIGHT direction
                //------------------------------------
                if ((flags & BACK_RIGHT) > 0)
                {
                    int newX = location.X + distance;
                    int newY = location.Y - distance * ForwardDirection[myColor];

                    if (!InBounds(newX, newY))
                    {
                        // We're out of bounds; stop searching this direction
                        flags -= BACK_RIGHT;
                    }
                    else
                    {
                        var pieceAtPos = board[newX, newY];
                        if (pieceAtPos != ChessPiece.Empty)
                        {
                            if (PieceColor[pieceAtPos] != myColor)
                            {
                                switch (pieceAtPos)
                                {
                                    case ChessPiece.BlackKing:
                                    case ChessPiece.WhiteKing:
                                        if (distance == 1)
                                            return true;
                                        break;
                                    case ChessPiece.BlackQueen:
                                    case ChessPiece.WhiteQueen:
                                    case ChessPiece.BlackBishop:
                                    case ChessPiece.WhiteBishop:
                                        return true;
                                    default:
                                        break;
                                }
                            }
                            // We run into a non-threatening piece; stop searching this direction
                            flags -= BACK_RIGHT;
                        }
                    }
                }
            }

            //------------------------------------
            // Check for KNIGHTS
            //------------------------------------
            var offsets1 = new int[] { -1, 1 };
            var offsets2 = new int[] { -2, 2 };

            // Check positions +/-1 in the X direction and +/-2 in the Y direction
            foreach (var xOffset in offsets1)
            {
                foreach (var yOffset in offsets2)
                {
                    int newX = location.X + xOffset;
                    int newY = location.Y + yOffset;

                    // Ignore positions out of bounds
                    if (!InBounds(newX, newY))
                        continue;

                    var piece = board[newX, newY];

                    // Ignore empty space and our own pieces
                    if (piece == ChessPiece.Empty || PieceColor[piece] == myColor)
                        continue;

                    switch (piece)
                    {
                        case ChessPiece.BlackKnight:
                        case ChessPiece.WhiteKnight:
                            return true;
                        default:
                            break;
                    }
                }
            }

            // Check positions +/-2 in the X direction and +/-1 in the Y direction
            foreach (var xOffset in offsets2)
            {
                foreach (var yOffset in offsets1)
                {
                    int newX = location.X + xOffset;
                    int newY = location.Y + yOffset;

                    // Ignore positions out of bounds
                    if (!InBounds(newX, newY))
                        continue;

                    var piece = board[newX, newY];

                    // Ignore empty space and our own pieces
                    if (piece == ChessPiece.Empty || PieceColor[piece] == myColor)
                        continue;

                    switch (piece)
                    {
                        case ChessPiece.BlackKnight:
                        case ChessPiece.WhiteKnight:
                            return true;
                        default:
                            break;
                    }
                }
            }

            // All clear
            return false;
        }

        /// <summary>
        /// Find the location of a specific piece on the board
        /// </summary>
        /// <param name="board">Board we are evaluating</param>
        /// <param name="piece">Piece we are looking for</param>
        /// <param name="location">Location where the piece was found</param>
        /// <returns>Returns true if the piece is found; else false (already captured). Location will be stored in the "location" parameter if found</returns>
        public static bool FindPiece(ChessBoard board, ChessPiece piece, out ChessLocation location)
        {
            location = null;

            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
            {
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                {
                    if (board[x, y] == piece)
                    {
                        location = new ChessLocation(x, y);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get a copy of what the board will look like after executing a specific move
        /// </summary>
        /// <param name="board">Board before move</param>
        /// <param name="move">Move we would like to evaluate</param>
        /// <returns>Board as it would appear after making move</returns>
        public static ChessBoard BoardAfterMove(ChessBoard board, ChessMove move)
        {
            var boardAfterMove = board.Clone();
            boardAfterMove.MakeMove(move);

            return boardAfterMove;
        }

        /// <summary>
        /// Convenience function to get the opposite color
        /// </summary>
        /// <param name="color">Color we want the opposite of</param>
        /// <returns>Returns the opposite color</returns>
        public static ChessColor OppColor(ChessColor color)
        {
            return color == ChessColor.Black ? ChessColor.White : ChessColor.Black;
        }
    }
}
