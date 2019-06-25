package com.baasie.SeatsSuggestionsDomain;

import java.util.*;
import java.util.stream.Collectors;
import java.util.stream.Stream;

public final class SeatCollectionExtensions {

    private SeatCollectionExtensions() {
    }

    public static List<Seat> selectAvailableSeatsCompliant(List<Seat> seats, PricingCategory pricingCategory) {
        return seats.stream().filter(seat -> seat.isAvailable() && seat.matchCategory(pricingCategory)).collect(Collectors.toList());
    }

    public static List<AdjacentSeats> selectAdjacentSeats(List<Seat> seats, int partySize) {
        List<AdjacentSeats> adjacentSeatsGroups = new ArrayList<>();
        List<Seat> adjacentSeats = new ArrayList<>();

        if (partySize == 1) {
            return seats.stream().map(seat -> new AdjacentSeats(Stream.of(seat).collect(Collectors.toCollection(ArrayList::new)))).collect(Collectors.toList());
        }

        List<Seat> sortedSeatByDistanceFromCentroid = seats.stream()
                .sorted(Comparator.comparing(Seat::distanceFromCentroid))
                .collect(Collectors.toCollection(ArrayList::new));

        for (Seat candidateSeat : seats) {
            if (adjacentSeats.size() == 0) {
                adjacentSeats.add(candidateSeat);
                continue;
            }

            if (candidateSeat.isAdjacentWith(adjacentSeats)) {
                adjacentSeats.add(candidateSeat);
            } else {
                if (adjacentSeats.size() == 1) {
                    adjacentSeats = Stream.of(candidateSeat).collect(Collectors.toCollection(ArrayList::new));
                } else {
                    adjacentSeatsGroups.add(ReduceAdjacentSeats(partySize, adjacentSeats));
                    adjacentSeats = Stream.of(candidateSeat).collect(Collectors.toCollection(ArrayList::new));
                }
            }
        }

        if (adjacentSeats.size() > 1) {
            adjacentSeatsGroups.add(ReduceAdjacentSeats(partySize, adjacentSeats));
        }

        if (adjacentSeatsGroups.size() == 0) {
            adjacentSeatsGroups.add(new AdjacentSeats(adjacentSeats));
        }
        return adjacentSeatsGroups.stream().filter(a -> a.size() >= partySize).collect(Collectors.toList());
    }

    private static AdjacentSeats ReduceAdjacentSeats(int partySize, List<Seat> adjacentSeats) {

        List<Seat> orderedAdjacentSeats = adjacentSeats.stream()
                .sorted(Comparator.comparing(Seat::distanceFromCentroid))
                .collect(Collectors.toCollection(ArrayList::new));

        List<Seat> sortedSeats = orderedAdjacentSeats.stream()
                .limit(partySize)
                .sorted(Comparator.comparing(Seat::number))
                .collect(Collectors.toCollection(ArrayList::new));
        return new AdjacentSeats(sortedSeats);
    }

    public static List<AdjacentSeats> orderByMiddleOfTheRow(List<AdjacentSeats> adjacentSeats,
                                                            int rowSize) {
        Map<Integer, List<AdjacentSeats>> sortedAdjacentSeatsGroups = new TreeMap<>();

        for (AdjacentSeats adjacentSeat : adjacentSeats) {
            int distance = adjacentSeat.computeDistanceFromRowCentroid();

            if (!sortedAdjacentSeatsGroups.containsKey(distance)) {
                sortedAdjacentSeatsGroups.put(distance, new ArrayList<>());
            }

            sortedAdjacentSeatsGroups.get(distance).add(adjacentSeat);
        }

        return sortedAdjacentSeatsGroups.values().stream().flatMap(Collection::stream).collect(Collectors.toList());
    }

    public static int centroidIndex(int rowSize) {
        return Math.abs(rowSize / 2);
    }

    public static int computeDistanceFromCentroid(int seatLocation, int rowSize) {
        return Math.abs(seatLocation - centroidIndex(rowSize));
    }

    public static boolean isCentroid(int seatLocation, int rowSize) {
        int centroidIndex = centroidIndex(rowSize);

        return seatLocation == centroidIndex || seatLocation == centroidIndex + 1;
    }

    public static boolean isOdd(int rowSize) {
        return rowSize % 2 != 0;
    }
}
