namespace MotoHub.Notifier.Entities;

public record MotorcycleRegisteredEvent(string Identifier,
                                        string Plate,
                                        int Year,
                                        string Model);