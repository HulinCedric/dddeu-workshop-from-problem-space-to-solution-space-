package com.baasie.SeatsSuggestionsDomain;

import com.google.common.collect.ImmutableList;

import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

class SuggestionMade {

    private ImmutableList<Seat> suggestedSeats;
    private int partyRequested;
    private PricingCategory pricingCategory;

    SuggestionMade(List<Seat> suggestedSeats, int partyRequested, PricingCategory pricingCategory) {
        this.suggestedSeats = ImmutableList.copyOf(suggestedSeats);
        this.partyRequested = partyRequested;
        this.pricingCategory = pricingCategory;
    }

    List<String> seatNames() {
        return suggestedSeats.stream().sorted(Comparator.comparing(Seat::number)).map(Seat::toString).collect(Collectors.toList());
    }

    boolean MatchExpectation() {
        return suggestedSeats.size() == partyRequested;
    }

    PricingCategory pricingCategory() {
        return pricingCategory;
    }
}
