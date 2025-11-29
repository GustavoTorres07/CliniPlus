namespace CliniPlus.Api.Services.Contrato
{
    public class EmailSettings
    {
        public string From { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = "CliniPlus";
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
    }
}
