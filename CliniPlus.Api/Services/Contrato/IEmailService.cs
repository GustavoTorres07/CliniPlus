namespace CliniPlus.Api.Services.Contrato
{
    public interface IEmailService
    {
        Task EnviarPasswordTemporalAsync(string emailDestino, string nombreDestino, string passwordTemporal);

    }
}
