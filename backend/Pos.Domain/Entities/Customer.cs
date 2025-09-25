using Pos.Domain.Common;

namespace Pos.Domain.Entities;

public class Customer : AuditableEntity
{
    protected Customer()
    {
    }

    public Customer(string code, string documentType, string documentNumber, string name)
    {
        Code = code;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        Name = name;
    }

    public string Code { get; private set; } = string.Empty;
    public string DocumentType { get; private set; } = string.Empty;
    public string DocumentNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? TradeName { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Mobile { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string Country { get; private set; } = "CO";
    public string? PostalCode { get; private set; }
    public decimal CreditLimit { get; private set; }
    public decimal AvailableCredit { get; private set; }
    public Guid? PriceListId { get; private set; }
    public string? TaxResponsibility { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public decimal LoyaltyPoints { get; private set; }

    public void UpdateContact(string? email, string? phone, string? mobile)
    {
        Email = email;
        Phone = phone;
        Mobile = mobile;
        MarkUpdated();
    }

    public void UpdateAddress(string? address, string? city, string? state, string country, string? postalCode)
    {
        Address = address;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        MarkUpdated();
    }

    public void SetCredit(decimal limit, decimal available)
    {
        CreditLimit = limit;
        AvailableCredit = available;
        MarkUpdated();
    }

    public void SetPriceList(Guid? priceListId)
    {
        PriceListId = priceListId;
        MarkUpdated();
    }

    public void SetTaxResponsibility(string? responsibility)
    {
        TaxResponsibility = responsibility;
        MarkUpdated();
    }

    public void SetBirthDate(DateOnly? birthDate)
    {
        BirthDate = birthDate;
        MarkUpdated();
    }

    public void AddLoyaltyPoints(decimal points)
    {
        LoyaltyPoints += points;
        MarkUpdated();
    }
}
