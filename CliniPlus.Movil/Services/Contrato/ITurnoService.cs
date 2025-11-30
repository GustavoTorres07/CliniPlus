using CliniPlus.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CliniPlus.Movil.Services.Contrato
{
    public interface ITurnoService
    {
        Task<List<TurnoListadoPacienteDTO>?> ObtenerMisTurnosAsync();

        Task<bool> ReservarAsync(TurnoReservarDTO dto);

        Task<bool> CancelarAsync(int turnoId);

        Task<List<TurnoSlotDTO>?> ObtenerSlotsDiaAsync(int medicoId, DateTime fechaLocal);

        Task<bool> ReservarSlotAsync(TurnoReservarSlotDTO dto);

        Task<List<TurnoSlotDTO>?> ObtenerSlotsDiaPublicAsync(int medicoId, DateTime fechaLocal);

        Task<bool> ReservarPublicoAsync(TurnoPublicoReservarDTO dto);

        Task<List<TurnoAgendaMedicoDTO>?> ObtenerAgendaHoyAsync();

        Task<List<TurnoAgendaMedicoDTO>?> ObtenerAgendaDiaAsync(DateTime fechaLocal);

        Task<List<PacienteListadoMedicoDTO>?> ObtenerPacientesMedicoAsync();

        Task<TurnoDetalleMedicoDTO?> ObtenerDetalleMedicoAsync(int turnoId);

        Task<List<HistoriaClinicaItemDTO>?> ObtenerHistoriaRapidaPacienteAsync(int pacienteId);

        Task<bool> RegistrarConsultaAsync(RegistrarConsultaMedicoDTO dto);

        Task<bool> MarcarTurnoCompletadoAsync(int turnoId);

        Task<PacienteDetalleMedicoDTO?> ObtenerPacienteDetalleAsync(int pacienteId);

        Task<List<HistoriaClinicaListadoDTO>?> ObtenerHistoriaClinicaAsync(int pacienteId, DateTime? desde, DateTime? hasta);

        Task<HistoriaClinicaDetalleDTO?> ObtenerHistoriaClinicaDetalleAsync(int entradaId);

        Task<HistoriaClinicaDetalleDTO?> ObtenerMiHistoriaClinicaDetalleAsync(int entradaId);

        Task<List<HistoriaClinicaListadoDTO>?> ObtenerMiHistoriaClinicaAsync(DateTime? desde, DateTime? hasta);

        Task<List<TurnoListadoSecretariaDTO>?> ListarTurnosHoySecretariaAsync();

        Task<bool> CancelarTurnoSecretariaAsync(int turnoId);

        Task<List<TurnoListadoSecretariaDTO>?> ListarAgendaSecretariaAsync(int medicoId, DateTime? desde, DateTime? hasta);

        Task<List<HistoriaClinicaListadoDTO>?> ObtenerHistoriaSecretariaAsync(int pacienteId, DateTime? desde, DateTime? hasta);

        Task<HistoriaClinicaDetalleDTO?> ObtenerHistoriaSecretariaDetalleAsync(int pacienteId, int entradaId);


    }
}
