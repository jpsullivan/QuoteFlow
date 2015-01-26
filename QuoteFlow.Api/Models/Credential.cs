using System.ComponentModel.DataAnnotations;

namespace QuoteFlow.Api.Models
{
    public class Credential
    {
        public Credential() { }

        public Credential(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(maximumLength: 64)]
        public string Type { get; set; }

        [Required]
        [StringLength(maximumLength: 256)]
        public string Value { get; set; }

        [StringLength(maximumLength: 256)]
        public string Ident { get; set; } // shorthand for identity which is a sql server keyword

        public virtual User User { get; set; }
    }
}