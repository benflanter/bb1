using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseCheckers.Model
{
    public class Graph
    {
        private Board board;
        private Dictionary<Piece, List<Piece>> graph;

        public Graph(Board board)
        {
            this.board = board;
            this.graph = new Dictionary<Piece, List<Piece>>();
        }

        public void CreateGraph()
        {
            // Create a node for each piece on the board
            for (int i = 0; i < Board.HEIGHT; i++)
            {
                for (int j = 0; j < Board.WIDTH; j++)
                {
                    if (Board.initmat[i, j] != 0)
                    {
                        Piece piece = board.getPiece(i, j);
                        if (piece == null)
                            piece = new Piece(i, j);
                        graph.Add(piece, new List<Piece>());
                    }
                }
            }

            // For each piece, find the possible destinations and create edges to them
            foreach (Piece piece in graph.Keys)
            {
                List<Move> moves;
                if (piece.empty)
                {
                    moves = board.player1.GetNearMoves(piece);
                }
                else
                {
                    if (piece.side)
                        moves = board.player1.GetMovesForPiece(piece);
                    else
                        moves = board.player2.GetMovesForPiece(piece);
                }
                foreach (Move move in moves)
                {
                    Piece destination = board.getPiece(move.GetRow(), move.GetCol());
                    if (destination == null)
                    {
                        destination = new Piece(move.GetRow(), move.GetCol());
                    }
                    graph[piece].Add(destination);
                }
            }
        }

        public void UpdateGraph(Move move)
        {
            Piece origin = move.GetOrigin();
            Piece destination = new Piece(move.GetRow(), move.GetCol());
            // Update origin piece's edges
            graph[origin].Clear();
            List<Move> nearMoves = board.player1.GetNearMoves(origin);
            foreach (Move nearMove in nearMoves)
            {
                Piece nearDestination = board.getPiece(nearMove.GetRow(), nearMove.GetCol());
                graph[origin].Add(nearDestination);
            }

            // Update destination piece's edges
            List<Move> newMoves;
            if (destination.side)
                newMoves = board.player1.GetMovesForPiece(destination);
            else
                newMoves = board.player2.GetMovesForPiece(destination);

            foreach (Move newMove in newMoves)
            {
                Piece newDestination = board.getPiece(newMove.GetRow(), newMove.GetCol());
                if (!graph[destination].Contains(newDestination))
                {
                    graph[destination].Add(newDestination);
                }
            }

            // Check for new possible moves for other pieces on the board
            foreach (Piece piece in graph.Keys)
            {
                if (!piece.Equals(origin) && !piece.Equals(destination))
                {
                    List<Move> otherMoves;
                    if (piece.empty)
                    {
                        otherMoves = board.player1.GetNearMoves(piece);
                    }
                    else
                    {
                        if (piece.side)
                            otherMoves = board.player1.GetMovesForPiece(piece);
                        else
                            otherMoves = board.player2.GetMovesForPiece(piece);
                    }

                    foreach (Move otherMove in otherMoves)
                    {
                        Piece otherDestination = board.getPiece(otherMove.GetRow(), otherMove.GetCol());
                        if (!graph[piece].Contains(otherDestination))
                        {
                            graph[piece].Add(otherDestination);
                        }
                    }
                }
            }
        }


        public List<Piece> ShortestPathDijkstra(Piece start, Piece end)
        {
            Dictionary<Piece, int> dist = new Dictionary<Piece, int>();
            Dictionary<Piece, Piece> prev = new Dictionary<Piece, Piece>();
            PriorityQueue<Piece> queue = new PriorityQueue<Piece>();

            // Initialize distances and prev
            foreach (Piece piece in graph.Keys)
            {
                dist[piece] = int.MaxValue;
                prev[piece] = null;
            }

            dist[start] = 0;
            queue.Enqueue(start, 0);

            while (queue.Count > 0)
            {
                Piece u = queue.Dequeue();
                if (u.Equals(end))
                {
                    break;
                }
                foreach (Piece v in graph[u])
                {
                    int alt = dist[u] + 1;
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                        queue.Enqueue(v, alt);
                    }
                }
            }
            // Construct the shortest path
            List<Piece> path = new List<Piece>();
            Piece current = end;
            while (current != null)
            {
                path.Add(current);
                current = prev[current];
            }
            path.Reverse();
            return path;
        }
    }
}
