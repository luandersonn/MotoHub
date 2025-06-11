namespace MotoHub.Application.Events;

public record MotorcycleRegisteredEvent(string Identifier,
                                        string Plate,
                                        int Year,
                                        string Model);