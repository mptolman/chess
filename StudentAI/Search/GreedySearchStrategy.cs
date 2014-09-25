using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;
using StudentAI.Heuristics;

namespace StudentAI.Search
{
    internal class GreedySearchStrategy : SearchStrategy
    {
        public GreedySearchStrategy(IChessAI ai, IHeuristic heuristic) : base(ai, heuristic)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves, Queue<ChessMove> recentMoves)
        {
            // Find our max value
            var maxValue = moves.Max(move => move.ValueOfMove);

            // Build a sorted list of moves
            var movesInAscendingOrder = moves.OrderByDescending(move => move.ValueOfMove).ToList();

            // Build a list of moves with the max value
            var movesWithMaxValue = moves.Where(move => move.ValueOfMove == maxValue).ToList();

            // Select one of these moves at random
            var random = new Random();
            int index = random.Next(movesWithMaxValue.Count);

            var selectedMove = movesWithMaxValue[index];

            int recentMoveIndex = 0;
            while(recentMoves.Contains(selectedMove))
            {
                selectedMove = movesInAscendingOrder[recentMoveIndex];
                recentMoveIndex++;
            }

            return selectedMove;
        }
    }
}
