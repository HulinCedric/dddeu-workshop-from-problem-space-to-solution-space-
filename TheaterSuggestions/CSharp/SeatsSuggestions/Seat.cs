using System.Collections.Generic;
using Value;

namespace SeatsSuggestions
{
    public class Seat : ValueType<Seat>
    {
        private readonly SeatAvailability seatAvailability;

        public Seat(string rowName, uint number, PricingCategory pricingCategory, SeatAvailability seatAvailability)
        {
            RowName = rowName;
            Number = number;
            PricingCategory = pricingCategory;
            this.seatAvailability = seatAvailability;
        }

        public uint Number { get; }
        public PricingCategory PricingCategory { get; }
        public string RowName { get; }

        public bool IsAvailable()
            => seatAvailability == SeatAvailability.Available;

        public override string ToString()
            => $"{RowName}{Number}";

        public bool MatchCategory(PricingCategory pricingCategory)
        {
            if (pricingCategory == PricingCategory.Mixed)
                return true;

            return PricingCategory == pricingCategory;
        }

        public Seat Allocate()
            => seatAvailability == SeatAvailability.Available
                   ? new Seat(
                       RowName,
                       Number,
                       PricingCategory,
                       SeatAvailability.Allocated)
                   : this;

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            yield return Number;
            yield return RowName;
            yield return PricingCategory;
            yield return seatAvailability;
        }
    }
}