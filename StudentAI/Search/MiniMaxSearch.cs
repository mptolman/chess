using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;
using StudentAI.Heuristics;

namespace StudentAI.Search
{
    internal class MiniMaxSearch : SearchStrategy
    {
        private const int MAX_DEPTH = 2;
        private DecisionTree _dt;

        public MiniMaxSearch(IChessAI ai, IHeuristic heuristic)
            : base(ai, heuristic)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves)
        {
            ChessMove selectedMove = null;
            int bestValue = myColor == ChessColor.White ? int.MinValue : int.MaxValue;

            var possibleMoves = new List<ChessMove>();
            ChessColor oppColor = Utility.OppColor(myColor);
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
                move.ValueOfMove = MiniMax(boardAfterMove, move, oppColor, MAX_DEPTH);

                if (myColor == ChessColor.White && move.ValueOfMove > bestValue)
                {
                    bestValue = move.ValueOfMove;
                    possibleMoves.Add(move);
                }
                else if (myColor == ChessColor.Black && move.ValueOfMove < bestValue)
                {
                    bestValue = move.ValueOfMove;
                    possibleMoves.Add(move);
                }
                else if (move.ValueOfMove == bestValue)
                {
                    possibleMoves.Add(move);
                }
#if DEBUG
                _dt.EventualMoveValue = move.ValueOfMove.ToString();
                _dt = _dt.Parent;
#endif
            }

            // Pick one of our top valued moves at random
            var random = new Random();
            possibleMoves = possibleMoves.Where(move => move.ValueOfMove == bestValue).ToList();
            selectedMove = possibleMoves[random.Next(possibleMoves.Count)];

#if DEBUG
            _dt.BestChildMove = selectedMove;
#endif
            return selectedMove;
        }

        private int MiniMax(ChessBoard board, ChessMove opponentsMove, ChessColor myColor, int depth)
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
                move.ValueOfMove = MiniMax(boardAfterMove, move, oppColor, depth - 1);

                if (myColor == ChessColor.White)
                {
                    if (bestMove == null || move.ValueOfMove > bestMove.ValueOfMove)
                        bestMove = move;
                }
                else if (bestMove == null || move.ValueOfMove < bestMove.ValueOfMove)
                {
                    bestMove = move;
                }
#if DEBUG
                _dt.EventualMoveValue = move.ValueOfMove.ToString();
                _dt = _dt.Parent;
#endif
            }
#if DEBUG
            _dt.BestChildMove = bestMove;
#endif
            return bestMove.ValueOfMove;
        }
    }
}
