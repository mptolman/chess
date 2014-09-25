using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;
using StudentAI.Heuristics;

namespace StudentAI.Search
{
    internal class RandomSearchStrategy : SearchStrategy
    {
        public RandomSearchStrategy(IChessAI ai, IHeuristic heuristic) : base(ai, heuristic)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves, Queue<ChessMove> recentMoves)
        {
            Random random = new Random();
            int index = random.Next(moves.Count);
            return moves[index];
        }
    }
}
