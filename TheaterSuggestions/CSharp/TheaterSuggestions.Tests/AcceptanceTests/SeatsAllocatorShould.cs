﻿using System.Threading.Tasks;
using ExternalDependencies.AuditoriumLayoutRepository;
using ExternalDependencies.ReservationsProvider;
using NFluent;
using NUnit.Framework;
using SeatsSuggestions.Domain;
using SeatsSuggestions.Infra.Adapter;

namespace SeatsSuggestions.Tests.AcceptanceTests
{
    [TestFixture]
    public class SeatsAllocatorShould
    {
        [Test]
        public async Task Return_SeatsNotAvailable_when_Auditorium_has_all_its_seats_already_reserved()
        {
            var showId = new ShowId("5");
            var partyRequested = new PartyRequested(1);

            var auditoriumSeatingRepository = new AuditoriumSeatingRepository(
                new AuditoriumSeatingAdapter(new AuditoriumLayoutRepository(), new ReservationsProvider()));

            var seatAllocator = new SeatAllocator(auditoriumSeatingRepository);

            var suggestionsMade = await seatAllocator.MakeSuggestions(showId, partyRequested);
            Check.That(suggestionsMade.PartyRequested).IsEqualTo(partyRequested);
            Check.That(suggestionsMade.ShowId).IsEqualTo(showId);

            Check.That(suggestionsMade).IsInstanceOf<SuggestionNotAvailable>();
        }

        [Test]
        public async Task Suggest_one_seat_when_Auditorium_contains_one_available_seat_only()
        {
            var showId = new ShowId("1");
            var partyRequested = new PartyRequested(1);

            var auditoriumSeatingRepository = new AuditoriumSeatingRepository(
                new AuditoriumSeatingAdapter(new AuditoriumLayoutRepository(), new ReservationsProvider()));

            var seatAllocator = new SeatAllocator(auditoriumSeatingRepository);

            var suggestionsMade = await seatAllocator.MakeSuggestions(showId, partyRequested);

            Check.That(suggestionsMade.SeatNames(PricingCategory.First)).ContainsExactly("A3");
        }

        [Test]
        public async Task Offer_several_suggestions_ie_1_per_PricingCategory_and_other_one_without_category_affinity()
        {
            var showId = new ShowId("18");
            var partyRequested = new PartyRequested(1);

            var auditoriumSeatingRepository = new AuditoriumSeatingRepository(
                new AuditoriumSeatingAdapter(new AuditoriumLayoutRepository(), new ReservationsProvider()));

            var seatAllocator = new SeatAllocator(auditoriumSeatingRepository);

            var suggestionsMade = await seatAllocator.MakeSuggestions(showId, partyRequested);

            Check.That(suggestionsMade.SeatNames(PricingCategory.First)).ContainsExactly("A5", "A6", "A4");
            Check.That(suggestionsMade.SeatNames(PricingCategory.Second)).ContainsExactly("A2", "A9", "A1");
            Check.That(suggestionsMade.SeatNames(PricingCategory.Third)).ContainsExactly("E5", "E6", "E4");

            Check.That(suggestionsMade.SeatNames(PricingCategory.Mixed)).ContainsExactly("A5", "A6", "A4");
        }

        [Test]
        public async Task Offer_adjacent_seats_nearer_the_middle_of_a_row()
        {
            var showId = new ShowId("9");
            var partyRequested = new PartyRequested(1);

            var auditoriumSeatingRepository = new AuditoriumSeatingRepository(
                new AuditoriumSeatingAdapter(new AuditoriumLayoutRepository(), new ReservationsProvider()));

            var seatAllocator = new SeatAllocator(auditoriumSeatingRepository);

            var suggestionsMade = await seatAllocator.MakeSuggestions(showId, partyRequested);

            Check.That(suggestionsMade.SeatNames(PricingCategory.First)).ContainsExactly("A4", "A3", "B5");
        }

        [Test]
        public async Task Offer_adjacent_seats_nearer_the_middle_of_a_row_when_it_is_possible()
        {
            var showId = new ShowId("3");
            PartyRequested partyRequested = new PartyRequested(4);

            var auditoriumSeatingRepository = new AuditoriumSeatingRepository(
                new AuditoriumSeatingAdapter(new AuditoriumLayoutRepository(), new ReservationsProvider()));

            var seatAllocator = new SeatAllocator(auditoriumSeatingRepository);

            var suggestionsMade = await seatAllocator.MakeSuggestions(showId, partyRequested);

            Check.That(suggestionsMade.SeatNames(PricingCategory.First)).IsEmpty();
            Check.That(suggestionsMade.SeatNames(PricingCategory.Second))
                .ContainsExactly("C4-C5-C6-C7", "D4-D5-D6-D7");
            Check.That(suggestionsMade.SeatNames(PricingCategory.Third))
                .ContainsExactly("E4-E5-E6-E7", "F4-F5-F6-F7");
            Check.That(suggestionsMade.SeatNames(PricingCategory.Mixed))
                .ContainsExactly("A6-A7-A8-A9", "C4-C5-C6-C7", "D4-D5-D6-D7");
        }
    }
}