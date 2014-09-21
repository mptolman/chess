using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    internal class GreedySearchStrategy : SearchStrategy
    {
        public GreedySearchStrategy(IChessAI ai, MoveGenerator moveGenerator) : base(ai, moveGenerator)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves)
        {
            return moves.OrderByDescending(move => move.ValueOfMove).FirstOrDefault();
        }
    }
}
