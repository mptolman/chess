using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;
using StudentAI.Heuristics;

namespace StudentAI.Search
{
    internal class GreedySearch : SearchStrategy
    {
        public GreedySearch(IChessAI ai, IHeuristic heuristic)
            : base(ai, heuristic)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves)
        {
            ChessMove selectedMove = null;
            IList<ChessMove> bestMoves;

            // Calculate our move values
            foreach (var move in moves)
                move.ValueOfMove = _heuristic.GetMoveValue(Utility.BoardAfterMove(board, move), move, Utility.OppColor(myColor));

            // Build a list of moves with the best value
            if (myColor == ChessColor.White)
                // White: higher is better
                bestMoves = moves.Where(m => m.ValueOfMove == moves.Max(x => x.ValueOfMove)).ToList();
            else
                // Black: lower is better
                bestMoves = moves.Where(m => m.ValueOfMove == moves.Min(x => x.ValueOfMove)).ToList();

            // Pick the best move that is not in our recent moves
            foreach (var move in bestMoves)
            {
                selectedMove = move;
                if (!_recentMoves.Contains(selectedMove))
                    break;
            }

            return selectedMove;
        }
    }
}
