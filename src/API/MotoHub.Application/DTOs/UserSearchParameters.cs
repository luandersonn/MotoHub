﻿using MotoHub.Domain.ValueObjects;

namespace MotoHub.Application.DTOs;

public class UserSearchParameters : PagedSearchParameters
{
    public UserRole? Role { get; set; }
    public string? Name { get; set; }
    public string? TaxNumber { get; set; }
    public string? DriverLicenseNumber { get; set; }
}
