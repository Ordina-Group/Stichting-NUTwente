namespace Ordina.StichtingNuTwente.Models.ViewModels;

public class DatabaseIntegrityTest
{
    public string Title { get; set; }
        
    public string Description { get; set; }

    public List<DatabaseIntegrityTestMessage> Messages { get; set; } = new List<DatabaseIntegrityTestMessage>();

    public void AddMessage(string message, DatabaseIntegrityLevel level = DatabaseIntegrityLevel.Information)
    {
        Messages.Add(new DatabaseIntegrityTestMessage(message, level));
    }
}