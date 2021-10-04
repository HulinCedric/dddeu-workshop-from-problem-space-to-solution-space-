package com.baasie.SeatsSuggestionsApi.controllers;

import com.baasie.SeatsSuggestions.AuditoriumSeatingAdapter;
import com.baasie.SeatsSuggestions.SeatAllocator;
import com.baasie.SeatsSuggestions.SuggestionsMade;
import com.baasie.ExternalDependencies.IProvideAuditoriumLayouts;
import com.baasie.ExternalDependencies.IProvideCurrentReservations;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("api/SeatsSuggestions")
public class SeatSuggestionsController {

    private IProvideAuditoriumLayouts auditoriumSeatingRepository;
    private IProvideCurrentReservations seatReservationsProvider;

    public SeatSuggestionsController() {
    }

    // GET api/SeatsSuggestions?showId=5&party=3
    @GetMapping(produces = "application/json")
    public SuggestionsMade get(@RequestParam String showId, @RequestParam int party) {

        SeatAllocator seatAllocator = new SeatAllocator(new AuditoriumSeatingAdapter(auditoriumSeatingRepository, seatReservationsProvider));
        return seatAllocator.makeSuggestions(showId, party);
    }
}
