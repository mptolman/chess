using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI.Search
{
    internal class GreedySearchStrategy : SearchStrategy
    {
        public GreedySearchStrategy(IChessAI ai, MoveGenerator moveGenerator) : base(ai, moveGenerator)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves)
        {
            // Find our max value
            var maxValue = moves.Max(move => move.ValueOfMove);

            // Build a list of moves with the max value
            var movesWithMaxValue = moves.Where(move => move.ValueOfMove == maxValue).ToList();

            // Select one of these moves at random
            var random = new Random();
            int index = random.Next(movesWithMaxValue.Count);

            return movesWithMaxValue[index];
        }
    }
}
