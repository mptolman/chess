using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;
using StudentAI.Heuristics;

namespace StudentAI.Search
{
    internal class MiniMaxAlphaBetaSearch : SearchStrategy
    {
        private const int MAX_DEPTH = 2;
        private DecisionTree _dt;

        public MiniMaxAlphaBetaSearch(IChessAI ai, IHeuristic heuristic)
            : base(ai, heuristic)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves)
        {
            ChessMove selectedMove = null;
            var possibleMoves = new List<ChessMove>();

            var oppColor = Utility.OppColor(myColor);

            int alpha = int.MinValue;
            int beta = int.MaxValue;
      
            foreach (var move in moves)
            {
                var boardAfterMove = Utility.BoardAfterMove(board, move);

                move.ValueOfMove = AlphaBeta(boardAfterMove, move, oppColor, alpha, beta, MAX_DEPTH);

                if (myColor == ChessColor.White)
                {
                    if (move.ValueOfMove > alpha)
                    {
                        alpha = move.ValueOfMove;
                        selectedMove = move;
                        possibleMoves.Add(move);
                    }
                    else if (move.ValueOfMove == alpha)
                        possibleMoves.Add(move);
                }
                else // Black
                {
                    if (move.ValueOfMove < beta)
                    {
                        beta = move.ValueOfMove;
                        selectedMove = move;
                        possibleMoves.Add(move);
                    }
                    else if (move.ValueOfMove == beta)
                        possibleMoves.Add(move);
                }
            }

            // If I uncomment the following 3 lines of code, then our AI goes to crap.
            // It seems to work as expected if we don't pick a random move

            //var random = new Random();
            //possibleMoves = possibleMoves.Where(move => move.ValueOfMove == (myColor == ChessColor.White ? alpha : beta)).ToList();
            //selectedMove = possibleMoves[random.Next(possibleMoves.Count)];

            return selectedMove;
        }

        private int AlphaBeta(ChessBoard board, ChessMove opponentsMove, ChessColor myColor, int alpha, int beta, int depth)
        {
            if (depth < 1 || opponentsMove.Flag == ChessFlag.Checkmate || _ai.IsMyTurnOver())
                return _heuristic.GetMoveValue(board, opponentsMove, myColor);

            var possibleMoves = GetAllMoves(board, myColor);
            if (possibleMoves.Count == 0)
                return 0; // Stalemate

            var oppColor = Utility.OppColor(myColor);

            foreach (var move in possibleMoves)
            {
                var boardAfterMove = Utility.BoardAfterMove(board, move);

                move.ValueOfMove = AlphaBeta(boardAfterMove, move, oppColor, alpha, beta, depth - 1);

                if (myColor == ChessColor.White)
                    alpha = Math.Max(alpha, move.ValueOfMove);
                else // Black
                    beta = Math.Min(beta, move.ValueOfMove);

                if (alpha >= beta)
                    break;
            }

            return myColor == ChessColor.White ? alpha : beta;
        }
    }
}
