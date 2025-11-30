using CliniPlus.Api.Data;
using CliniPlus.Api.Repositories.Contrato;
using CliniPlus.Shared.DTOs;
using CliniPlus.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Repositories.Implementa
{
    public class FichaMedicaRepository : IFichaMedicaRepository
    {
        private readonly AppDbContext _db;

        public FichaMedicaRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<FichaMedicaDTO?> ObtenerPorPacienteAsync(int pacienteId)
        {
            var ficha = await _db.FichaMedica
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.PacienteId == pacienteId);

            if (ficha == null)
                return null;

            return new FichaMedicaDTO
            {
                PacienteId = ficha.PacienteId,
                GrupoSanguineo = ficha.GrupoSanguineo,
                Alergias = ficha.Alergias,
                EnfermedadesCronicas = ficha.EnfermedadesCronicas,
                MedicacionHabitual = ficha.MedicacionHabitual,
                Observaciones = ficha.Observaciones
            };
        }

        public async Task<FichaMedicaDTO> GuardarAsync(int pacienteId, FichaMedicaDTO dto)
        {
            var paciente = await _db.Paciente
                .Include(p => p.FichaMedica)
                .FirstOrDefaultAsync(p => p.IdPaciente == pacienteId && p.IsActive);

            if (paciente == null)
                throw new InvalidOperationException("PACIENTE_NO_ENCONTRADO");

            // Normalizar textos
            string? Normalizar(string? s) =>
                string.IsNullOrWhiteSpace(s) ? null : s.Trim();

            if (paciente.FichaMedica == null)
            {
                // Crear nueva ficha
                var ficha = new FichaMedica
                {
                    PacienteId = pacienteId,
                    GrupoSanguineo = Normalizar(dto.GrupoSanguineo),
                    Alergias = Normalizar(dto.Alergias),
                    EnfermedadesCronicas = Normalizar(dto.EnfermedadesCronicas),
                    MedicacionHabitual = Normalizar(dto.MedicacionHabitual),
                    Observaciones = Normalizar(dto.Observaciones)
                };

                _db.FichaMedica.Add(ficha);
            }
            else
            {
                // Actualizar ficha existente
                paciente.FichaMedica.GrupoSanguineo = Normalizar(dto.GrupoSanguineo);
                paciente.FichaMedica.Alergias = Normalizar(dto.Alergias);
                paciente.FichaMedica.EnfermedadesCronicas = Normalizar(dto.EnfermedadesCronicas);
                paciente.FichaMedica.MedicacionHabitual = Normalizar(dto.MedicacionHabitual);
                paciente.FichaMedica.Observaciones = Normalizar(dto.Observaciones);
            }

            await _db.SaveChangesAsync();

            dto.PacienteId = pacienteId;
            return dto;
        }
    }
}
