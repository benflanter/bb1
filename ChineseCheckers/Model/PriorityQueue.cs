using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseCheckers.Model
{
    public class PriorityQueue<Piece>
    {
        private List<Tuple<Piece, int>> elements = new List<Tuple<Piece, int>>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(Piece item, int priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public Piece Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Item2 < elements[bestIndex].Item2)
                {
                    bestIndex = i;
                }
            }

            Piece bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }

}
