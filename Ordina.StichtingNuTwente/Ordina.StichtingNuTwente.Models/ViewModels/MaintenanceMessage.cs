namespace Ordina.StichtingNuTwente.Models.ViewModels;

public class MaintenanceMessage
{
    public MaintenanceMessage()
    {
    }

    public MaintenanceMessage(string message, MaintenanceMessageType messageType = MaintenanceMessageType.Info)
    {
        Message = message;
        MessageType = messageType;
    }

    public string Message { get; set; }

    public MaintenanceMessageType MessageType { get; set; }

    public string MessageTypeColor
    {
        get
        {
            switch (MessageType)
            {
                case MaintenanceMessageType.Warning:
                    return "orange";
                case MaintenanceMessageType.Error:
                    return "red";
                case MaintenanceMessageType.Success:
                    return "green";
                default:
                    return "black";
            }
        }
    }
}