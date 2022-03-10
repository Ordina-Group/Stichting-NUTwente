namespace Ordina.StichtingNuTwente.Models
{
    public class Vrijwilliger: BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public VrijwilligerType Type { get; set; }
    }
}