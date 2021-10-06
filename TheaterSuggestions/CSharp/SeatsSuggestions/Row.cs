using System.Collections.Generic;
using System.Linq;
using Value;

namespace SeatsSuggestions
{
    public class Row : ValueType<Row>
    {
        public Row(string name, List<Seat> seats)
        {
            Name = name;
            Seats = seats.AsReadOnly();
        }

        public string Name { get; }
        public IReadOnlyList<Seat> Seats { get; }

        public Row AddSeat(Seat seat)
        {
            var seats = Seats.ToList();
            seats.Add(seat);
            return new Row(Name, seats);
        }

        public SeatingOptionSuggested SuggestSeatingOption(int partyRequested, PricingCategory pricingCategory)
        {
            foreach (var seat in Seats)
                if (seat.IsAvailable() &&
                    seat.MatchCategory(pricingCategory))
                {
                    var seatAllocation = new SeatingOptionSuggested(partyRequested, pricingCategory);

                    seatAllocation.AddSeat(seat);

                    if (seatAllocation.MatchExpectation())
                        return seatAllocation;
                }

            return new SeatingOptionNotAvailable(partyRequested, pricingCategory);
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            yield return Name;
            yield return new ListByValue<Seat>(Seats.ToList());
        }

        public Row Allocate(Seat seatToAllocate)
        {
            var newVersionOfSeats = new List<Seat>(Seats);

            var indexOfSeatToAllocate = newVersionOfSeats.IndexOf(seatToAllocate);
            newVersionOfSeats[indexOfSeatToAllocate] = seatToAllocate.Allocate();

            return new Row(seatToAllocate.RowName, newVersionOfSeats);
        }
    }
}