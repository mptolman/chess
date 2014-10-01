using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI.Heuristics
{
    internal class PositionalHeuristic : IHeuristic
    {
        public int GetMoveValue(ChessBoard boardAfterMove, ChessMove opponentsMove, ChessColor myColor)
        {
            // This means the other player put us in checkmate
            // This is bad, so return the worst possible value
            if (opponentsMove.Flag == ChessFlag.Checkmate)
                return myColor == ChessColor.White ? int.MinValue : int.MaxValue;

            int moveValue = PositionalValue(boardAfterMove, myColor) + MaterialValue(boardAfterMove, myColor);
            if (Utility.IsProtected(boardAfterMove, myColor, opponentsMove.To) && boardAfterMove[opponentsMove.To] != ChessPiece.WhiteKing && boardAfterMove[opponentsMove.To] != ChessPiece.BlackKing)
            {
                moveValue += PieceValue(boardAfterMove[opponentsMove.To]) * 2 / 3;
                if (Utility.CanBeCaptured(boardAfterMove, myColor, opponentsMove.To))
                    moveValue *= 2;
            }

            return myColor == ChessColor.White ? moveValue : -moveValue;
        }

        private int PositionalValue(ChessBoard boardAfterMove, ChessColor myColor)
        {
            int positionalValue = 0;

            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
            {
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                {
                    var piece = boardAfterMove[x, y];
                    if (piece == ChessPiece.Empty)
                        continue;

                    int position = y * ChessBoard.NumberOfColumns + x;

                    if (myColor == Utility.PieceColor(piece))
                        positionalValue += PieceValue(piece, position);
                    else
                        positionalValue -= PieceValue(piece, position);
                }
            }

            return positionalValue;
        }

        private int PieceValue(ChessPiece piece, int position)
        {
            switch (piece)
            {
                case ChessPiece.BlackPawn:
                    return BlackPawnTable[position];
                case ChessPiece.WhitePawn:
                    return PawnTable[position];
                case ChessPiece.BlackBishop:
                    return BlackBishopTable[position];
                case ChessPiece.WhiteBishop:
                    return BishopTable[position];
                case ChessPiece.BlackKnight:
                    return BlackKnightTable[position];
                case ChessPiece.WhiteKnight:
                    return KnightTable[position];
                case ChessPiece.BlackRook:
                    return BlackRookTable[position];
                case ChessPiece.WhiteRook:
                    return RookTable[position];
                case ChessPiece.BlackQueen:
                    return BlackQueenTable[position];
                case ChessPiece.WhiteQueen:
                    return QueenTable[position];
                case ChessPiece.BlackKing:
                    return BlackKingTable[position];
                case ChessPiece.WhiteKing:
                    return KingTable[position];
                default:
                    return 0;
            }
        }

        private int MaterialValue(ChessBoard boardAfterMove, ChessColor myColor)
        {
            int materialValue = 0;

            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                    if (myColor == Utility.PieceColor(boardAfterMove[x, y]))
                        materialValue += PieceValue(boardAfterMove[x, y]);

            return materialValue;
        }

        private int PieceValue(ChessPiece piece)
        {
            switch (piece)
            {
                case ChessPiece.BlackPawn:
                case ChessPiece.WhitePawn:
                    return 100;
                case ChessPiece.BlackBishop:
                case ChessPiece.BlackKnight:
                case ChessPiece.WhiteBishop:
                case ChessPiece.WhiteKnight:
                    return 300;
                case ChessPiece.BlackRook:
                case ChessPiece.WhiteRook:
                    return 500;
                case ChessPiece.BlackQueen:
                case ChessPiece.WhiteQueen:
                    return 900;
                case ChessPiece.BlackKing:
                case ChessPiece.WhiteKing:
                    return 1000;
                default:
                    return 0;
            }
        }

        private short[] PawnTable = new short[]
        {
            100,  100,  100,  100,  100,  100,  100,  100,
            50, 50, 50, 50, 50, 50, 50, 50,
            10, 10, 20, 30, 30, 20, 10, 10,
            5,  5,  10, 25, 25, 10,  5,  5,
            0,  0,  0, 20, 20,  0,  0,  0,
            5, -5, -10,  0,  0,-10, -5,  5,
            5, 10, 10, -20, -20, 10, 10, 5,
            0,  0,  0,  0,   0,   0,  0, 0
        };
        private short[] BlackPawnTable = new short[]
        {
            0,  0,  0,  0,  0,  0,  0,  0,
            5, 10, 10, -20, -20, 10, 10, 5,
            5, -5, -10, 0, 0, -10, -5,  5,
            0,  0,  10, 20, 20, 0,  0,  0,
            5,  5,  10,  25, 25,  10,  5, 5,
            10, 10, 20,  30,  30, 20, 10,  10,
            50, 50, 50, 50, 50, 50, 50, 50,
            100,  100,  100,  100,   100,   100,  100, 100
        };
        private short[] KnightTable = new short[]
        {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50
        };
        private short[] BlackKnightTable = new short[]
        {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50
        };
        private short[] BishopTable = new short[]
        {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-10,-10,-10,-10,-10,-20
        };
        private short[] BlackBishopTable = new short[]
        {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -10,  10,  10, 10, 10,  10,  10,-10,
            -10,  0,  10, 10, 10,  10,  0,-10,
            -10,  5, 5, 10, 10, 5,  5,-10,
            -10, 0, 5, 10, 10, 5, 00,-10,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -20,-10,-10,-10,-10,-10,-10,-20
        };
        private short[] RookTable = new short[]
        {
            0,  0,  0,  0,  0,  0,  0,  0,
            5, 10, 10, 10, 10, 10, 10,  5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            0,  0,  0,  0,  0,  0,  0,  0
        };
        private short[] BlackRookTable = new short[]
        {
            0,  0,  0,  0,  0,  0,  0,  0,
            -5, 0, 0, 0, 0, 0, 0,  -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            -5,  0,  0,  0,  0,  0,  0, -5,
            5,  10,  10, 10,  10,  10,  10,  5,
            0,  0,  0,  0,  0,  0,  0,  0
        };
        private short[] QueenTable = new short[]
        {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5,  5,  5,  5,  0,-10,
            -5,  0,  5,  5,  5,  5,  0, -5,
                0,  0,  5,  5,  5,  5,  0, -5,
            -10,  5,  5,  5,  5,  5,  0,-10,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        };
        private short[] BlackQueenTable = new short[]
        {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  0,  0,  0,  5,  0,-10,
            -10,  0,  5,  5,  5,  5,  5,-10,
            -5,  0,  5,  5,  5,  5,  0, 0,
            -5,  0,  5,  5,  5,  5,  0, -5,
            -10,  0,  5,  5,  5,  5,  0,-10,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        };
        private short[] KingTable = new short[]
        {
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -10,-20,-20,-20,-20,-20,-20,-10,
            20, 20,  0,  0,  0,  0, 20, 20,
            20, 30, 10,  0,  0, 10, 30, 20
        };
        private short[] BlackKingTable = new short[]
        {
            20,30,10,0,0,10,30,20,
            20,20,0,0,0,0,20,20,
            -10,-20,-20,-20,-20,-20,-20,-10,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30
        };
    }
}
