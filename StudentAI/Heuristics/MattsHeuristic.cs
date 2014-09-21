using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI.Heuristics
{
    internal class MattsHeuristic : IHeuristic
    {
        public int GetMoveValue(ChessBoard boardAfterMove, ChessMove move, ChessColor myColor)
        {
            // If this move leads to checkmate, then go straight to it
            if (move.Flag == ChessFlag.Checkmate)
                return int.MaxValue;

            return MaterialValue(boardAfterMove, myColor);
        }

        private int MaterialValue(ChessBoard boardAfterMove, ChessColor myColor)
        {
            int materialValue = 0;

            // Initialize white piece counts to zero
            IDictionary<ChessPiece, int> whitePieceCount = new Dictionary<ChessPiece, int>
            {
                {ChessPiece.WhiteBishop, 0},
                {ChessPiece.WhiteKing, 0},
                {ChessPiece.WhiteKnight, 0},
                {ChessPiece.WhitePawn, 0},
                {ChessPiece.WhiteQueen, 0},
                {ChessPiece.WhiteRook, 0}
            };

            // Initialize black piece counts to zero
            IDictionary<ChessPiece, int> blackPieceCount = new Dictionary<ChessPiece, int>
            {
                {ChessPiece.BlackBishop, 0},
                {ChessPiece.BlackKing, 0},
                {ChessPiece.BlackKnight, 0},
                {ChessPiece.BlackPawn, 0},
                {ChessPiece.BlackQueen, 0},
                {ChessPiece.BlackRook, 0}
            };

            IDictionary<ChessPiece, int> myCount;
            IDictionary<ChessPiece, int> opCount;

            if (myColor == ChessColor.Black)
            {
                myCount = blackPieceCount;
                opCount = whitePieceCount;
            }
            else
            {
                myCount = whitePieceCount;
                opCount = blackPieceCount;
            }

            // Count how many of each piece each player has
            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
            {
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                {
                    var piece = boardAfterMove[x, y];
                    if (piece == ChessPiece.Empty)
                        continue;

                    if (Utility.PieceColor(piece) == myColor)
                        ++myCount[piece];
                    else
                        ++opCount[piece];
                }
            }

            // Add the values of our pieces (Value * Count)
            foreach (var pieceCount in myCount)
                materialValue += PieceValue(pieceCount.Key) * pieceCount.Value;

            // Subtract the value of our opponent's pieces (Value * Count)
            foreach (var pieceCount in opCount)
                materialValue -= PieceValue(pieceCount.Key) * pieceCount.Value;

            return materialValue;
        }

        private int PieceValue(ChessPiece piece)
        {
            switch (piece)
            {
                case ChessPiece.BlackPawn:
                case ChessPiece.WhitePawn:
                    return 1;
                case ChessPiece.BlackBishop:
                case ChessPiece.WhiteBishop:
                case ChessPiece.BlackKnight:
                case ChessPiece.WhiteKnight:
                    return 3;
                case ChessPiece.BlackRook:
                case ChessPiece.WhiteRook:
                    return 5;
                case ChessPiece.BlackQueen:
                case ChessPiece.WhiteQueen:
                    return 9;
                case ChessPiece.BlackKing:
                case ChessPiece.WhiteKing:
                    return 1000;
                default:
                    return 0;
            }
        }
    }
}
