using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrader.GW2API.Model
{
    internal static class TradingPost
    {
        private const double _listingFee = 0.05;
        private const double _exchangeFee = 0.10;

        private static int GetFeePerUnit(int price, double feePercent)
        {
            return Convert.ToInt32(Math.Round((double)price * feePercent));
        }

        internal static int GetUnitListingFee(int price)
        {
            return GetFeePerUnit(price, _listingFee);
        }

        internal static int GetUnitExchangeFee(int price)
        {
            return GetFeePerUnit(price, _exchangeFee);
        }

        internal static int CalculateFees(int price, Direction direction)
        {
            if (direction == Direction.Buy)
                return 0;
            else
                return GetUnitListingFee(price) + GetUnitExchangeFee(price);
        }
    }
}
