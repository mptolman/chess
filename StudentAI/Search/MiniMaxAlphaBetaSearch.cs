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
            ChessMove bestMove = null;
            ChessColor oppColor = Utility.OppColor(myColor);

            int alpha = int.MinValue;
            int beta = int.MaxValue;
#if DEBUG
            _dt = new DecisionTree(board);
            _ai.SetDecisionTree(_dt);
#endif
            foreach (var move in moves)
            {
                var boardAfterMove = Utility.BoardAfterMove(board, move);
#if DEBUG
                _dt.AddChild(boardAfterMove, move);
                _dt = _dt.LastChild;
#endif
                move.ValueOfMove = AlphaBeta(boardAfterMove, move, oppColor, alpha, beta, MAX_DEPTH);
                if (bestMove == null || move.ValueOfMove > bestMove.ValueOfMove)
                    bestMove = move;
#if DEBUG
                _dt.EventualMoveValue = move.ValueOfMove.ToString();
                _dt = _dt.Parent;
#endif
            }
#if DEBUG
            _dt.BestChildMove = bestMove;
#endif
            return bestMove;
        }

        private int AlphaBeta(ChessBoard board, ChessMove opponentsMove, ChessColor myColor, int alpha, int beta, int depth)
        {
            if (depth < 1 || opponentsMove.Flag == ChessFlag.Checkmate || _ai.IsMyTurnOver())
                return _heuristic.GetMoveValue(board, opponentsMove, myColor);

            var possibleMoves = GetAllMoves(board, myColor);
            if (possibleMoves.Count == 0)
                return 0; // Stalemate

            ChessMove bestMove = null;
            ChessColor oppColor = Utility.OppColor(myColor);

            foreach (var move in possibleMoves)
            {
                var boardAfterMove = Utility.BoardAfterMove(board, move);
#if DEBUG
                _dt.AddChild(boardAfterMove, move);
                _dt = _dt.LastChild;
#endif
                move.ValueOfMove = AlphaBeta(boardAfterMove, move, oppColor, alpha, beta, depth - 1);

                if (myColor == ChessColor.White)
                {
                    if (move.ValueOfMove > alpha)
                    {
                        alpha = move.ValueOfMove;
                        bestMove = move;
                    }
                }
                else if (move.ValueOfMove < beta)
                {
                    beta = move.ValueOfMove;
                    bestMove = move;
                }
#if DEBUG
                _dt.EventualMoveValue = move.ValueOfMove.ToString();
                _dt = _dt.Parent;
#endif
                if (beta <= alpha)
                    break;
            }
#if DEBUG
            _dt.BestChildMove = bestMove;
#endif
            return myColor == ChessColor.White ? alpha : beta;
        }
    }
}
