namespace Ordina.StichtingNuTwente.Business.Services;

public class MaintenanceMessage
{
    public MaintenanceMessage(string message, MaintenanceMessageType messageType = MaintenanceMessageType.Info)
    {
        Message = message;
        MessageType = messageType;
    }

    public string Message { get; set; }

    public MaintenanceMessageType MessageType { get; set; }
}