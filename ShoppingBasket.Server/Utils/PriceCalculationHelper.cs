namespace ShoppingBasket.Server.Utils
{
    public static class PriceCalculationHelper
    {
        public static decimal CalculateSubTotalCost(decimal pricePerUnit, int quantity)
        {
            return Math.Round(pricePerUnit * quantity, 2);
        }

        public static decimal CalculateDiscountedCost(decimal pricePerUnit, int quantity, decimal percentage)
        {
            var subTotalCost = CalculateSubTotalCost(pricePerUnit, quantity);
            var discountAmount = Math.Round(subTotalCost * (percentage / 100m), 2);
            return Math.Round(subTotalCost - discountAmount, 2);
        }
    }
}
