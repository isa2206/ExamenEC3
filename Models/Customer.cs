using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace Examen.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        [Required, EmailAddress, StringLength(200)] public string Email { get; set; } = string.Empty;
        [Required, StringLength(200)] public string FullName { get; set; } = string.Empty;
        public bool Active { get; set; } = true;

        // 1:1 (base del examen) — luego lo cambiarán a 1:N
        // public LoyaltyCard? LoyaltyCard { get; set; }

        public ICollection<LoyaltyCard> LoyaltyCards { get; set; } = new List<LoyaltyCard>();

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }

}
