using System;
using System.Collections.Generic;

namespace Chap10.Models.SaleModels;

public partial class WarrantyRegistration
{
    public int WarrantyId { get; set; }

    public int? VehicleId { get; set; }

    public string? WarrantyProvider { get; set; }

    public string? WarrantyType { get; set; }

    public string? CoverageDetails { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? MileageLimit { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
