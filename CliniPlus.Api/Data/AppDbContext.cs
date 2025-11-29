using CliniPlus.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CliniPlus.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets (uno por tabla)
        public DbSet<Usuario> Usuario => Set<Usuario>();
        public DbSet<ObraSocial> ObraSocial => Set<ObraSocial>();
        public DbSet<Especialidad> Especialidad => Set<Especialidad>();
        public DbSet<CIE10> CIE10 => Set<CIE10>();
        public DbSet<TipoTurno> TipoTurno => Set<TipoTurno>();
        public DbSet<Paciente> Paciente => Set<Paciente>();
        public DbSet<Medico> Medico => Set<Medico>();
        public DbSet<FichaMedica> FichaMedica => Set<FichaMedica>();
        public DbSet<MedicoHorario> MedicoHorario => Set<MedicoHorario>();
        public DbSet<MedicoBloqueo> MedicoBloqueo => Set<MedicoBloqueo>();
        public DbSet<Turno> Turno => Set<Turno>();
        public DbSet<Consulta> Consulta => Set<Consulta>();
        public DbSet<ConsultaDiagnostico> ConsultaDiagnostico => Set<ConsultaDiagnostico>();
        public DbSet<HistoriaClinicaEntrada> HistoriaClinicaEntrada => Set<HistoriaClinicaEntrada>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // -------------------- Usuario --------------------
            mb.Entity<Usuario>(e =>
            {
                // CORREGIDO: HasCheckConstraint movido dentro de ToTable
                e.ToTable("Usuario", t =>
                {
                    t.HasCheckConstraint("CK_Usuario_Rol",
                        "Rol IN (N'Administrador', N'Secretaria', N'Medico', N'Paciente')");
                });

                e.HasKey(x => x.IdUsuario);
                e.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
                e.Property(x => x.Apellido).HasMaxLength(100).IsRequired();
                e.Property(x => x.Email).HasMaxLength(150).IsRequired();
                e.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
                e.Property(x => x.Rol).HasMaxLength(20).IsRequired();
                e.Property(x => x.IsActive).HasDefaultValue(true);
                e.Property(x => x.FechaRegistro);
                e.HasIndex(x => x.Email).IsUnique();

            });

            // -------------------- ObraSocial --------------------
            mb.Entity<ObraSocial>(e =>
            {
                e.ToTable("ObraSocial");
                e.HasKey(x => x.IdObraSocial);
                e.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
                e.Property(x => x.Descripcion).HasMaxLength(200);
                e.Property(x => x.IsActive).HasDefaultValue(true);
                e.HasIndex(x => x.Nombre).IsUnique();
            });

            // -------------------- Especialidad --------------------
            mb.Entity<Especialidad>(e =>
            {
                e.ToTable("Especialidad");
                e.HasKey(x => x.IdEspecialidad);
                e.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
                e.Property(x => x.Descripcion).HasMaxLength(200);
                e.Property(x => x.IsActive).HasDefaultValue(true);
                e.HasIndex(x => x.Nombre).IsUnique();

                // 1:N Especialidad -> Medicos
                e.HasMany(x => x.Medicos)
                 .WithOne(m => m.Especialidad)
                 .HasForeignKey(m => m.EspecialidadId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------- CIE10 --------------------
            mb.Entity<CIE10>(e =>
            {
                e.ToTable("CIE10");
                e.HasKey(x => x.Codigo);
                e.Property(x => x.Codigo).HasMaxLength(10);
                e.Property(x => x.Descripcion).HasMaxLength(300).IsRequired();
            });

            // -------------------- TipoTurno --------------------
            mb.Entity<TipoTurno>(e =>
            {
                // CORREGIDO: HasCheckConstraint movido dentro de ToTable
                e.ToTable("TipoTurno", t =>
                {
                    t.HasCheckConstraint("CK_TipoTurno_Duracion", "DuracionMin > 0");
                });

                e.HasKey(x => x.IdTipoTurno);
                e.Property(x => x.Nombre).HasMaxLength(120).IsRequired();
                e.Property(x => x.DuracionMin).IsRequired();
                e.Property(x => x.Descripcion).HasMaxLength(200);
                e.Property(x => x.Activo).HasDefaultValue(true);
                e.HasIndex(x => x.Nombre).IsUnique();
            });

            // -------------------- Paciente --------------------
            mb.Entity<Paciente>(e =>
            {
                e.ToTable("Paciente");
                e.HasKey(x => x.IdPaciente);
                e.Property(x => x.DNI).HasMaxLength(8).IsRequired();
                e.Property(x => x.IsProvisional).HasDefaultValue(true);
                e.Property(x => x.Telefono).HasMaxLength(30);
                e.Property(x => x.Email).HasMaxLength(150);
                e.Property(x => x.NumeroAfiliado).HasMaxLength(50);
                e.Property(x => x.IsActive).HasDefaultValue(true);

                e.HasIndex(x => x.DNI).IsUnique();
                e.HasIndex(x => x.UsuarioId).IsUnique();

                // 1:1 opcional con Usuario (FK en Paciente)
                e.HasOne(x => x.Usuario)
                 .WithOne(u => u.Paciente!)
                 .HasForeignKey<Paciente>(x => x.UsuarioId)
                 .OnDelete(DeleteBehavior.Restrict);

                // N:1 con ObraSocial (nullable)
                e.HasOne(x => x.ObraSocial)
                 .WithMany(o => o.Pacientes)
                 .HasForeignKey(x => x.ObraSocialId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------- Medico --------------------
            mb.Entity<Medico>(e =>
            {
                e.ToTable("Medico", t =>
                {
                    t.HasCheckConstraint("CK_Medico_DefaultSlotMin", "DefaultSlotMin > 0");
                });

                e.HasKey(x => x.IdMedico);
                e.Property(x => x.Bio).HasMaxLength(1000);
                e.Property(x => x.FotoUrl).HasMaxLength(300);
                e.Property(x => x.DefaultSlotMin).HasDefaultValue(30);
                e.Property(x => x.IsActive).HasDefaultValue(true);

                e.HasIndex(x => x.UsuarioId).IsUnique();

                e.HasOne(x => x.Usuario)
                 .WithOne(u => u.Medico!)
                 .HasForeignKey<Medico>(x => x.UsuarioId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);

                // No hace falta nada extra para Especialidad, ya lo cubre el HasMany de arriba
            });

            // -------------------- FichaMedica (1:1 Paciente) --------------------
            mb.Entity<FichaMedica>(e =>
            {
                e.ToTable("FichaMedica");
                e.HasKey(x => x.IdFicha);
                e.Property(x => x.GrupoSanguineo).HasMaxLength(5);
                e.Property(x => x.Alergias).HasMaxLength(500);
                e.Property(x => x.EnfermedadesCronicas).HasMaxLength(500);
                e.Property(x => x.MedicacionHabitual).HasMaxLength(500);
                e.Property(x => x.Observaciones).HasMaxLength(1000);

                e.HasIndex(x => x.PacienteId).IsUnique();

                e.HasOne(x => x.Paciente)
                 .WithOne(p => p.FichaMedica!)
                 .HasForeignKey<FichaMedica>(x => x.PacienteId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------- MedicoHorario --------------------
            mb.Entity<MedicoHorario>(e =>
            {
                // CORREGIDO: HasCheckConstraint movido dentro de ToTable
                e.ToTable("MedicoHorario", t =>
                {
                    t.HasCheckConstraint("CK_MedicoHorario_Dia", "DiaSemana BETWEEN 0 AND 6");
                    t.HasCheckConstraint("CK_MedicoHorario_Slot", "(SlotMinOverride IS NULL OR SlotMinOverride > 0)");
                });

                e.HasKey(x => x.IdHorario);
                e.Property(x => x.Activo).HasDefaultValue(true);

                e.HasIndex(x => new { x.MedicoId, x.DiaSemana });

                e.HasOne(x => x.Medico)
                 .WithMany(m => m.Horarios)
                 .HasForeignKey(x => x.MedicoId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------- ConsultaDiagnostico (N:M)  --------------------


            mb.Entity<ConsultaDiagnostico>(e =>
            {
                e.ToTable("ConsultaDiagnostico");
                e.HasKey(x => new { x.ConsultaId, x.CIE10Codigo });
                e.Property(x => x.Comentario).HasMaxLength(500);
                e.Property(x => x.Principal).HasDefaultValue(false); // ← AGREGAR ESTA LÍNEA
                e.Property(x => x.CIE10Codigo).HasMaxLength(10).IsRequired(); // ← AGREGAR IsRequired

                e.HasOne(x => x.Consulta)
                 .WithMany(c => c.Diagnosticos)
                 .HasForeignKey(x => x.ConsultaId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.CIE10)
                 .WithMany(c => c.ConsultaDiagnosticos)
                 .HasForeignKey(x => x.CIE10Codigo)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------- MedicoBloqueo --------------------
            mb.Entity<MedicoBloqueo>(e =>
            {
                // CORREGIDO: HasCheckConstraint movido dentro de ToTable
                e.ToTable("MedicoBloqueo", t =>
                {
                    t.HasCheckConstraint("CK_MedicoBloqueo_Rango", "Hasta > Desde");
                });

                e.HasKey(x => x.IdBloqueo);

                e.HasOne(x => x.Medico)
                 .WithMany(m => m.Bloqueos)
                 .HasForeignKey(x => x.MedicoId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------- Turno --------------------
            mb.Entity<Turno>(e =>
            {
                e.ToTable("Turno", t =>
                {
                    t.HasCheckConstraint("CK_Turno_Duracion", "DuracionMin > 0");
                    t.HasCheckConstraint("CK_Turno_Estado",
                        "Estado IN (N'Disponible', N'Reservado', N'Completado', N'Cancelado', N'Inasistencia', N'Atendido', N'Ausente', N'No Asistió')");
                });

                e.HasKey(x => x.IdTurno);

                e.Property(x => x.Estado).HasMaxLength(20).IsRequired();
                e.Property(x => x.IsActive).HasDefaultValue(true);

                e.HasIndex(x => new { x.MedicoId, x.ScheduledAtUtc })
                 .IsUnique()
                 .HasDatabaseName("UX_Turno_Medico_Fecha");

                e.HasIndex(x => new { x.PacienteId, x.ScheduledAtUtc })
                 .HasDatabaseName("IX_Turno_Paciente_Fecha");

                e.HasIndex(x => new { x.Estado, x.ScheduledAtUtc })
                 .HasDatabaseName("IX_Turno_Estado_Fecha");

                e.HasOne(x => x.Medico)
                 .WithMany(m => m.Turnos)
                 .HasForeignKey(x => x.MedicoId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Paciente)
                 .WithMany(p => p.Turnos)
                 .HasForeignKey(x => x.PacienteId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.TipoTurno)
                 .WithMany(t => t.Turnos)
                 .HasForeignKey(x => x.TipoTurnoId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------- Consulta --------------------
            mb.Entity<Consulta>(e =>
            {
                e.ToTable("Consulta");
                e.HasKey(x => x.IdConsulta);
                e.Property(x => x.Notas).HasMaxLength(2000);

                e.HasOne(x => x.Turno)
                 .WithOne(t => t.Consulta!)
                 .HasForeignKey<Consulta>(x => x.TurnoId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Medico)
                 .WithMany(m => m.Consultas)
                 .HasForeignKey(x => x.MedicoId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // -------------------- HistoriaClinicaEntrada --------------------
            mb.Entity<HistoriaClinicaEntrada>(e =>
            {
                e.ToTable("HistoriaClinicaEntrada");
                e.HasKey(x => x.IdEntrada);
                e.Property(x => x.Notas).HasMaxLength(2000);
                e.Property(x => x.CIE10Codigo).HasMaxLength(10).IsRequired();

                e.HasIndex(x => new { x.PacienteId, x.Fecha }).HasDatabaseName("IX_HC_Paciente_Fecha");
                e.HasIndex(x => new { x.MedicoId, x.Fecha }).HasDatabaseName("IX_HC_Medico_Fecha");

                e.HasOne(x => x.Paciente)
                 .WithMany(p => p.HistoriaClinica)
                 .HasForeignKey(x => x.PacienteId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Medico)
                 .WithMany(m => m.HistoriaClinica)
                 .HasForeignKey(x => x.MedicoId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.CIE10)
                 .WithMany(c => c.HistoriaClinicaEntradas)
                 .HasForeignKey(x => x.CIE10Codigo)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Turno)
                 .WithMany(t => t.HistoriaClinica)
                 .HasForeignKey(x => x.TurnoId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Consulta)
                 .WithMany(c => c.HistoriaClinica)
                 .HasForeignKey(x => x.ConsultaId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
