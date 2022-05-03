using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordina.StichtingNuTwente.Models.ViewModels
{
    public class DatabaseIntegrityModel : BaseModel
    {
        public List<DatabaseIntegrityTest> Inconsistencies { get; set; } = new List<DatabaseIntegrityTest>();

        public List<DatabaseIntegrityTest> Statistics { get; set; } = new List<DatabaseIntegrityTest>();
    }

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
                        return "green";
                    default:
                        return "black";
                }
            }
        }
    }

    public enum DatabaseIntegrityLevel
    {
        Information,
        Warning,
        Error,
        Success
    }
}
