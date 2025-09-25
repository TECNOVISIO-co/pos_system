namespace Pos.Domain.ValueObjects;

public readonly record struct Money(decimal Amount, string Currency)
{
    public static Money Zero(string currency = "COP") => new(0m, currency);

    public decimal Rounded(int decimals = 2) => Math.Round(Amount, decimals, MidpointRounding.AwayFromZero);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor) => new(Amount * factor, Currency);

    private void EnsureSameCurrency(Money other)
    {
        if (!Currency.Equals(other.Currency, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Currency mismatch: {Currency} vs {other.Currency}");
        }
    }

    public override string ToString() => $"{Currency} {Amount:0.00}";
}
