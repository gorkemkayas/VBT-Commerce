using BuildingBlocks.Application.Messaging;

namespace Pricing.Application.Queries.TaxRate.GetTaxRate;

public record GetTaxRateQuery : IQuery<decimal>;
