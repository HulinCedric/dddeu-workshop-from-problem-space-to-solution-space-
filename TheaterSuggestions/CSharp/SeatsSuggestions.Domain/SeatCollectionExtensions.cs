using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SeatsSuggestions.Domain
{
    public static class SeatCollectionExtensions
    {
        public static IEnumerable<Seat> SelectAvailableSeatsCompliant(this IEnumerable<Seat> seats,
            PricingCategory pricingCategory)
        {
            return seats.Where(s => s.IsAvailable() && s.MatchCategory(pricingCategory));
        }

         public static IEnumerable<AdjacentSeats> SelectAdjacentSeats(this IEnumerable<Seat> seats,
            int partySize)
        {
            var adjacentSeatsGroups = new List<AdjacentSeats>();
            var adjacentSeats = new List<Seat>();
            var currentSeats = seats.ToList();

            if (partySize == 1) return currentSeats.Select(s => new AdjacentSeats(new List<Seat> {s}));

            foreach (var candidateSeat in currentSeats)
            {
                if (!adjacentSeats.Any())
                {
                    adjacentSeats.Add(candidateSeat);
                    continue;
                }

                adjacentSeats = adjacentSeats.OrderBy(s => s.Number).ToList();

                if (candidateSeat.IsAdjacentWith(adjacentSeats))
                {
                    adjacentSeats.Add(candidateSeat);

                    if (NoMoreSeats(adjacentSeats, currentSeats))
                    {
                        adjacentSeatsGroups.Add(new AdjacentSeats(ReduceAdjacentSeats(partySize, adjacentSeats)));
                    }
                }
                else
                {
                    if (adjacentSeats.Any())
                    {
                        adjacentSeatsGroups.Add(ReduceAdjacentSeats(partySize, adjacentSeats));
                    }

                    adjacentSeats = new List<Seat> {candidateSeat};
                }
            }

            return adjacentSeatsGroups.Where(adjacent => adjacent.Count() >= partySize);
        }

        private static AdjacentSeats ReduceAdjacentSeats(int partySize, List<Seat> adjacentSeats)
        {
            var orderedEnumerable = adjacentSeats.OrderBy(s => s.DistanceFromRowCentroid).ToList();

            return new AdjacentSeats(orderedEnumerable.Take(partySize).OrderBy(s => s.Number));
        }

        private static bool NoMoreSeats(ICollection adjacentSeats, ICollection currentSeats)
        {
            return adjacentSeats.Count == currentSeats.Count;
        }

        private static bool DoesNotExceedPartyRequestedAndCandidateSeatIsAdjacent(Seat candidateSeat,
            List<Seat> adjacentSeats,
            int partySize)
        {
            return candidateSeat.IsAdjacentWith(adjacentSeats) && adjacentSeats.Count < partySize;
        }

        public static IEnumerable<AdjacentSeats> OrderByMiddleOfTheRow(this IEnumerable<AdjacentSeats> adjacentSeats,
            int rowSize)
        {
            var sortedAdjacentSeatsGroups = new SortedList<int, List<AdjacentSeats>>();

            foreach (var adjacentSeat in adjacentSeats)
            {
                var distanceFromRowCentroid = adjacentSeat.ComputeDistanceFromRowCentroid(rowSize);

                if (!sortedAdjacentSeatsGroups.ContainsKey(distanceFromRowCentroid))
                    sortedAdjacentSeatsGroups.Add(distanceFromRowCentroid, new List<AdjacentSeats>());

                sortedAdjacentSeatsGroups[distanceFromRowCentroid].Add(adjacentSeat);
            }

            return sortedAdjacentSeatsGroups.Values.SelectMany(_ => _);
        }

        internal static int CentroidIndex(this int rowSize)
        {
            return (int)Math.Ceiling((decimal)rowSize / 2);
        }

        internal static int ComputeDistanceFromCentroid(this uint seatLocation, int rowSize)
        {
            return (int)Math.Abs(seatLocation - rowSize.CentroidIndex());
        }

        internal static bool IsCentroid(this uint seatLocation, int rowSize)
        {
            var centroidIndex = rowSize.CentroidIndex();
            return seatLocation == centroidIndex;
        }

        internal static bool IsOdd(this int rowSize)
        {
            return rowSize % 2 != 0;
        }
    }
}