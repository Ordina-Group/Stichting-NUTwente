namespace Ordina.StichtingNuTwente.Models.ViewModels;

public class DatabaseIntegrityTestMessage
{
    public DatabaseIntegrityTestMessage()
    {
    }

    public DatabaseIntegrityTestMessage(string message,
        DatabaseIntegrityLevel level = DatabaseIntegrityLevel.Information)
    {
        Message = message;
        Level = level;
    }

    public string Message { get; set; }

    public DatabaseIntegrityLevel Level { get; set; }

    public string MessageTypeColor
    {
        get
        {
            switch (Level)
            {
                case DatabaseIntegrityLevel.Warning:
                    return "orange";
                case DatabaseIntegrityLevel.Error:
                    return "red";
                case DatabaseIntegrityLevel.Success:
                    return "#88CC00";
                default:
                    return "black";
            }
        }
    }
}