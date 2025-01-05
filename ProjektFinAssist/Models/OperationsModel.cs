using Microsoft.AspNetCore.Identity;

namespace ProjektFinAssist.Models
{
    public enum Category
    {
        Transport,Food,Entertainment,Bills,Shopping,Education
    }

    public enum Type
    {
        Expense,Income
    }

    public class OperationsModel
    {
        public int ID { get; set; }
        public Category Category { get; set; }
        public Type Type { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string? UserID { get; set; }
        public IdentityUser? IdentityUser { get; set; }
    }
}
