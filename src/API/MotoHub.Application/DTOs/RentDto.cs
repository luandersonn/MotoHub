﻿using MotoHub.Domain.Entities;

namespace MotoHub.Application.DTOs;

public class RentDto
{
    public required string Identifier { get; set; }
    public required string MotorcycleIdentifier { get; set; }
    public required string CourierIdentifier { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime EstimatedEndDate { get; set; }
    public int Plan { get; set; }
    public required RentStatus Status { get; set; }
    public required decimal DailyRate { get; set; }
}
