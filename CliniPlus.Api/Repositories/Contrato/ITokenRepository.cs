namespace CliniPlus.Api.Repositories.Contrato
{
    public interface ITokenRepository
    {
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
