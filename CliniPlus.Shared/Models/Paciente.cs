using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Shared.Models
{
    [Table("Paciente")]
    public class Paciente
    {
        [Key]
        [Column("IdPaciente")]
        public int IdPaciente { get; set; }

        [Column("UsuarioId")]
        public int? UsuarioId { get; set; }

        [Required, MaxLength(8)]
        public string DNI { get; set; } = null!;

        [Required]
        public bool IsProvisional { get; set; } = true;

        [MaxLength(30)]
        public string? Telefono { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [Column("ObraSocialId")]
        public int? ObraSocialId { get; set; }

        [MaxLength(50)]
        public string? NumeroAfiliado { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navs
        [ForeignKey(nameof(UsuarioId))]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey(nameof(ObraSocialId))]
        public virtual ObraSocial? ObraSocial { get; set; }

        public virtual FichaMedica? FichaMedica { get; set; }
        public virtual ICollection<Turno> Turnos { get; set; } = new List<Turno>();
        public virtual ICollection<HistoriaClinicaEntrada> HistoriaClinica { get; set; } = new List<HistoriaClinicaEntrada>();
    }
}
