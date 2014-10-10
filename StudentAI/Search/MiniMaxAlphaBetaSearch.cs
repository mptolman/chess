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
        public MiniMaxAlphaBetaSearch(IChessAI ai, IHeuristic heuristic)
            : base(ai, heuristic)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves)
        {
#if DEBUG
            _ai.Log("***** Selecting move *****");
#endif
            ChessMove selectedMove = null;
            ChessMove nextBestMove = null;

            var oppColor = Utility.OppColor(myColor);

            for (int depthLimit = 0; !_ai.IsMyTurnOver(); ++depthLimit)
            {
                // Select the best move from the last completed search
                if (nextBestMove != null)
                    selectedMove = nextBestMove.Clone();

                var alpha = int.MinValue;
                var beta = int.MaxValue;
#if DEBUG
                _ai.Log(String.Format("Searching depth {0}", depthLimit));
#endif
                foreach (var move in moves)
                {
                    var boardAfterMove = Utility.BoardAfterMove(board, move);

                    move.ValueOfMove = AlphaBeta(boardAfterMove, move, oppColor, alpha, beta, depthLimit);

                    if (myColor == ChessColor.White && move.ValueOfMove > alpha)
                    {
                        alpha = move.ValueOfMove;
                        nextBestMove = move;
#if DEBUG
                        _ai.Log(String.Format("{0} Value: {1}", move, move.ValueOfMove));
#endif
                    }
                    else if (myColor == ChessColor.Black && move.ValueOfMove < beta)
                    {
                        beta = move.ValueOfMove;
                        nextBestMove = move;
#if DEBUG
                        _ai.Log(String.Format("{0} Value: {1}", move, move.ValueOfMove));
#endif
                    }
                }
            }
#if DEBUG
            _ai.Log(String.Format(">>>>> {0} Value: {1}", selectedMove, selectedMove.ValueOfMove));
#endif
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
                else 
                    beta = Math.Min(beta, move.ValueOfMove);

                if (alpha >= beta)
                    break;
            }

            return myColor == ChessColor.White ? alpha : beta;
        }
    }
}
