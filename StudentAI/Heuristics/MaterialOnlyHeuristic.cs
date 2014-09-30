using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI.Heuristics
{
    internal class MaterialOnlyHeuristic : IHeuristic
    {
        public int GetMoveValue(ChessBoard boardAfterMove, ChessMove opponentsMove, ChessColor myColor)
        {
            // This means the other player put us in checkmate
            // This is bad
            if (opponentsMove.Flag == ChessFlag.Checkmate)
                return myColor == ChessColor.White ? int.MinValue : int.MaxValue;

            return MaterialValue(boardAfterMove, myColor);
        }

        private int MaterialValue(ChessBoard boardAfterMove, ChessColor myColor)
        {
            int materialValue = 0;

            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                    materialValue += PieceValue(boardAfterMove[x, y]);

            return materialValue;
        }

        private int PieceValue(ChessPiece piece)
        {
            switch (piece)
            {
                case ChessPiece.BlackPawn:
                    return -1;
                case ChessPiece.WhitePawn:
                    return 1;
                case ChessPiece.BlackBishop:
                case ChessPiece.BlackKnight:
                    return -3;
                case ChessPiece.WhiteBishop:
                case ChessPiece.WhiteKnight:
                    return 3;
                case ChessPiece.BlackRook:
                    return -5;
                case ChessPiece.WhiteRook:
                    return 5;
                case ChessPiece.BlackQueen:
                    return -9;
                case ChessPiece.WhiteQueen:
                    return 9;
                case ChessPiece.BlackKing:
                    return -11;
                case ChessPiece.WhiteKing:
                    return 11;
                default:
                    return 0;
            }
        }
    }
}
