using Chap10.Dtos.SaleDTO;
using Chap10.Models.SaleModels;
using Chap10.Services.Shared;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;

namespace Chap10.Services;

public class SaleAPI : ISaleApiClient
{
    private readonly SaleDbContext _saleContext;
    private readonly ILogger<SaleAPI> _logger;

    public SaleAPI(SaleDbContext saleContext, ILogger<SaleAPI> logger)
    {
        _saleContext = saleContext;
        _logger = logger;
    }

    public async Task<SaleDto?> GetSaleDataByVinAsync(int vin)
    {
        var operationId = Guid.NewGuid().ToString("N")[..8];
        using (LogContext.PushProperty("OperationId", operationId))
        {
            _logger.LogInformation("Starting GetSaleDataByVinAsync for VIN: {Vin} | OperationId: {OperationId}", vin, operationId);

            try
            {
                // Fetch sales transaction
                _logger.LogDebug("Querying SalesTransactions table for VIN: {Vin}", vin);
                var saleTransaction = await _saleContext.SalesTransactions
                    .Include(x => x.Customer)
                    .Include(x => x.FinancingContracts)
                    .FirstOrDefaultAsync(x => x.VehicleId == vin);

                if (saleTransaction == null)
                {
                    _logger.LogInformation("No sales transaction found for VIN: {Vin}", vin);
                    return null;
                }

                _logger.LogInformation("Found sales transaction for VIN: {Vin} | TransactionId: {TransactionId} | CustomerId: {CustomerId}",
                    vin, saleTransaction.TransactionId, saleTransaction.CustomerId);

                // Fetch sales document
                _logger.LogDebug("Querying SalesDocuments table for VIN: {Vin}", vin);
                var salesDocument = await _saleContext.SalesDocuments
                    .Where(x => x.VehicleId == vin)
                    .OrderByDescending(x => x.GeneratedDate)
                    .FirstOrDefaultAsync();

                if (salesDocument != null)
                {
                    _logger.LogDebug("Found sales document for VIN: {Vin} | DocumentId: {DocumentId} | Type: {DocumentType}",
                        vin, salesDocument.SalesDocumentId, salesDocument.DocumentType);
                }
                else
                {
                    _logger.LogDebug("No sales documents found for VIN: {Vin}", vin);
                }

                // Fetch warranty registration
                _logger.LogDebug("Querying WarrantyRegistrations table for VIN: {Vin}", vin);
                var warrantyRegistration = await _saleContext.WarrantyRegistrations
                    .Where(x => x.VehicleId == vin)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync();

                if (warrantyRegistration != null)
                {
                    _logger.LogDebug("Found warranty registration for VIN: {Vin} | WarrantyId: {WarrantyId} | Type: {WarrantyType}",
                        vin, warrantyRegistration.WarrantyId, warrantyRegistration.WarrantyType);
                }
                else
                {
                    _logger.LogDebug("No warranty registration found for VIN: {Vin}", vin);
                }

                // Build response
                var result = new SaleDto
                {
                    Customer = saleTransaction.Customer == null ? null : new CustomerDto
                    {
                        CustomerId = saleTransaction.Customer.CustomerId,
                        FullName = saleTransaction.Customer.FullName,
                        Email = saleTransaction.Customer.Email,
                        PhoneNumber = saleTransaction.Customer.PhoneNumber,
                        Address = saleTransaction.Customer.Address,
                        NationalId = saleTransaction.Customer.NationalId,
                        DateOfBirth = saleTransaction.Customer.DateOfBirth,
                        CustomerType = saleTransaction.Customer.CustomerType,
                        DriverLicenseNumber = saleTransaction.Customer.DriverLicenseNumber,
                        LoyaltyTier = saleTransaction.Customer.LoyaltyTier,
                        CreatedAt = saleTransaction.Customer.CreatedAt,
                        UpdatedAt = saleTransaction.Customer.UpdatedAt
                    },
                    SalesTransaction = new SalesTransactionDto
                    {
                        TransactionId = saleTransaction.TransactionId,
                        VehicleId = saleTransaction.VehicleId,
                        CustomerId = saleTransaction.CustomerId,
                        SalesRepresentativeId = saleTransaction.SalesRepresentativeId,
                        DealershipId = saleTransaction.DealershipId,
                        SellingPrice = saleTransaction.SellingPrice,
                        DiscountAmount = saleTransaction.DiscountAmount,
                        TaxAmount = saleTransaction.TaxAmount,
                        PaymentMethod = saleTransaction.PaymentMethod,
                        TransactionStatus = saleTransaction.TransactionStatus,
                        SalesDate = saleTransaction.SalesDate,
                        DeliveryDate = saleTransaction.DeliveryDate,
                        CreatedAt = saleTransaction.CreatedAt,
                        UpdatedAt = saleTransaction.UpdatedAt
                    },
                    FinancingContract = saleTransaction.FinancingContracts
                        .OrderByDescending(x => x.CreatedAt)
                        .Select(x => new FinancingContractDto
                        {
                            FinancingContractId = x.FinancingContractId,
                            TransactionId = x.TransactionId,
                            FinancingProvider = x.FinancingProvider,
                            LoanAmount = x.LoanAmount,
                            InterestRate = x.InterestRate,
                            LoanDurationMonths = x.LoanDurationMonths,
                            MonthlyPayment = x.MonthlyPayment,
                            ContractStartDate = x.ContractStartDate,
                            ContractEndDate = x.ContractEndDate,
                            ApprovalStatus = x.ApprovalStatus,
                            CreatedAt = x.CreatedAt,
                            UpdatedAt = x.UpdatedAt
                        })
                        .FirstOrDefault(),
                    SalesDocument = salesDocument == null ? null : new SalesDocumentDto
                    {
                        SalesDocumentId = salesDocument.SalesDocumentId,
                        VehicleId = salesDocument.VehicleId,
                        DocumentType = salesDocument.DocumentType,
                        DocumentNumber = salesDocument.DocumentNumber,
                        DocumentTitle = salesDocument.DocumentTitle,
                        FileUrl = salesDocument.FileUrl,
                        FileFormat = salesDocument.FileFormat,
                        SourceSystem = salesDocument.SourceSystem,
                        UploadedBy = salesDocument.UploadedBy,
                        GeneratedDate = salesDocument.GeneratedDate,
                        Status = salesDocument.Status,
                        Version = salesDocument.Version,
                        CreatedAt = salesDocument.CreatedAt,
                        UpdatedAt = salesDocument.UpdatedAt
                    },
                    WarrantyRegistration = warrantyRegistration == null ? null : new WarrantyRegistrationDto
                    {
                        WarrantyId = warrantyRegistration.WarrantyId,
                        VehicleId = warrantyRegistration.VehicleId,
                        WarrantyProvider = warrantyRegistration.WarrantyProvider,
                        WarrantyType = warrantyRegistration.WarrantyType,
                        CoverageDetails = warrantyRegistration.CoverageDetails,
                        StartDate = warrantyRegistration.StartDate,
                        EndDate = warrantyRegistration.EndDate,
                        MileageLimit = warrantyRegistration.MileageLimit,
                        Status = warrantyRegistration.Status,
                        CreatedAt = warrantyRegistration.CreatedAt,
                        UpdatedAt = warrantyRegistration.UpdatedAt
                    }
                };

                _logger.LogInformation("Successfully built sale data for VIN: {Vin} | Has Customer: {HasCustomer}, Has FinancingContract: {HasFinancingContract}",
                    vin, result.Customer != null, result.FinancingContract != null);
                
                return result;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while fetching sale data for VIN: {Vin} | Message: {ErrorMessage}", vin, dbEx.Message);
                throw;
            }
            catch (InvalidOperationException invOpEx)
            {
                _logger.LogError(invOpEx, "Invalid operation while fetching sale data for VIN: {Vin} | Message: {ErrorMessage}", vin, invOpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching sale data for VIN: {Vin} | Error: {ErrorType}: {ErrorMessage}",
                    vin, ex.GetType().Name, ex.Message);
                throw;
            }
        }
    }
}