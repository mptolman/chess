using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UvsChess;

namespace StudentAI
{
    class RandomSearchStrategy : SearchStrategy
    {
        public RandomSearchStrategy(MoveGenerator moveGenerator) : base(moveGenerator)
        { }

        protected override ChessMove SelectFromAvailableMoves(ChessBoard board, ChessColor myColor, IList<ChessMove> moves)
        {
            Random random = new Random();
            int index = random.Next(moves.Count);
            return moves[index];
        }
    }
}
