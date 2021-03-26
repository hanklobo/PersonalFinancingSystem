using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace PersonalFinancialSystem.Services
{
    public class Entry : IValidatableObject
    {
        [Required]
        public string Data { get; set; }
        [DataType(DataType.Currency)]
        [Required]
        public decimal Valor { get; set; }
        [StringLength(255, MinimumLength = 3)]
        [Required]
        public string Descricao { get; set; }
        [StringLength(50, MinimumLength = 2)]
        [Required]
        public string Conta { get; set; }
        [StringLength(1)]
        [Required]
        public string Tipo { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!DateTime.TryParseExact(Data, "yyyy/MM/dd",CultureInfo.CurrentCulture,
                DateTimeStyles.AssumeLocal, out var dateValue))
            {
                yield return new ValidationResult("Data inválida. Inserir no formato 'yyyy/MM/dd'.");
            }
            if (Tipo != "c" && Tipo != "d")
            {
                yield return new ValidationResult("Tipo deve ser 'c' para crédito ou 'd' para débito.");
            }
            if (Valor <= 0)
            {
                yield return new ValidationResult("Valor deve ser positivo.");
            }
        }
    }
    public class Totalizador
    {
        public string Conta { get; set; }
        public decimal Valor { get; set; }
    }
    public class EntryOutput
    {
        public EntryOutput()
        {
            Totalizadores = new Dictionary<string, decimal>();
            Lancamentos = new List<Entry>();
        }
        public IDictionary<string, decimal> Totalizadores { get; set; }
        public IList<Entry> Lancamentos { get; set; }
    }
}