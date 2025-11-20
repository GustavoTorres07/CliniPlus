namespace CliniPlus.Api.Repositories.Contrato
{
    public interface ITokenRepository
    {
        /// <summary>
        /// Emite un JWT con claims estándar (sub, name, email, role).
        /// </summary>
        /// <param name="usuarioId">Id de Usuario</param>
        /// <param name="nombre">Nombre</param>
        /// <param name="apellido">Apellido</param>
        /// <param name="email">Email</param>
        /// <param name="rol">Rol (Administrador/Secretario/Medico/Paciente)</param>
        /// <param name="horasValidez">Horas de validez del token (default 8h)</param>
        /// <returns>Token JWT serializado</returns>

        string EmitirToken(
            int usuarioId,
            string nombre,
            string apellido,
            string email,
            string rol,
            int horasValidez = 8
        );

    }
}
