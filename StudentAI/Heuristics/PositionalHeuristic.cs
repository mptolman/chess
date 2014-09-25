using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI.Heuristics
{
    internal class PositionalHeuristic : IHeuristic
    {
        public int GetMoveValue(ChessBoard boardAfterMove, ChessMove move, ChessColor myColor)
        {
            // If this move leads to checkmate, then go straight to it
            if (move.Flag == ChessFlag.Checkmate)
                return int.MaxValue;

            int moveValue = PositionalValue(boardAfterMove, myColor) + MaterialValue(boardAfterMove, myColor);
            if (Utility.IsProtected(boardAfterMove, myColor, move.To) && boardAfterMove[move.To] != ChessPiece.WhiteKing && boardAfterMove[move.To] != ChessPiece.BlackKing)
            {
                moveValue += PieceValue(boardAfterMove[move.To]) * 2 / 3;
                if (Utility.CanBeCaptured(boardAfterMove, myColor, move.To))
                    moveValue *= 2;
            }
            return moveValue;
        }

        private int PositionalValue(ChessBoard boardAfterMove, ChessColor myColor)
        {
            int positionalValue = 0;

            // Count how many of each piece each player has
            for (int y = 0; y < ChessBoard.NumberOfRows; ++y)
            {
                for (int x = 0; x < ChessBoard.NumberOfColumns; ++x)
                {
                    var piece = boardAfterMove[x, y];
                    if (piece == ChessPiece.Empty)
                        continue;

                    var color = Utility.PieceColor(piece);
                    int position = x * 8 + y;
                    if (color == myColor)
                        positionalValue += PieceValue(piece, color, position);
                    else
                        positionalValue -= PieceValue(piece, color, position);
                }
            }
            return positionalValue;
        }

        private int PieceValue(ChessPiece piece, ChessColor color, int position)
        {
            short[] PawnTable = new short[]
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
            short[] BlackPawnTable = new short[]
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
            short[] KnightTable = new short[]
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
            short[] BlackKnightTable = new short[]
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
            short[] BishopTable = new short[]
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
            short[] BlackBishopTable = new short[]
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
            short[] RookTable = new short[]
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
            short[] BlackRookTable = new short[]
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
            short[] QueenTable = new short[]
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
            short[] BlackQueenTable = new short[]
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
            short[] KingTable = new short[]
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
            short[] BlackKingTable = new short[]
            {
                20,30,10,0,0,10,30,20,
                20,20,0,0,0,0,20,20,
                -10,-20,-20,-20,-20,-20,-20,-10,
                -20,-30,-30,-40,-40,-30,-30,-20,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                 -30, -40,  -40,  -50,  -50,  -40, -40, -30,
                 -30,-40, -40,  -50,  -50, -40, -40, -30
            };
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
                    return 100;
                case ChessPiece.BlackBishop:
                case ChessPiece.WhiteBishop:
                case ChessPiece.BlackKnight:
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
    }
}
