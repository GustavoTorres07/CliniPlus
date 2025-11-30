using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using CliniPlus.Api.Services.Contrato;

namespace CliniPlus.Api.Services.Implementa
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarPasswordTemporalAsync(string emailDestino, string nombreDestino, string passwordTemporal)
        {
            var emailOrigen = _config["Email:SmtpUser"];
            var password = _config["Email:SmtpPass"];
            var host = _config["Email:SmtpServer"] ?? "smtp.gmail.com";
            var portStr = _config["Email:SmtpPort"] ?? "587";
            int port = int.Parse(portStr);
            var fromName = _config["Email:FromName"] ?? "CliniPlus";

            if (string.IsNullOrWhiteSpace(emailOrigen) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Configuración de correo incompleta.");

            var cuerpoMail =
                $"<h3>Recuperacion de contraseña - Clini+</h3>" +
                $"<p>Hola! <b>{nombreDestino}</b>,</p>" +
                $"<p>Hemos generado una contraseña temporal para que puedas acceder a la app Clini+.</p>" +
                $"<p>Tu contraseña temporal es: <b>{passwordTemporal}</b></p>" +
                $"<p>Por seguridad, al ingresar deberas cambiarla por una nueva contraseña.</p>" +
                $"<br/><p>Saludos, equipo Clini+</p>";

            var mensaje = new MailMessage
            {
                From = new MailAddress(emailOrigen, fromName),
                Subject = "Recuperacion de contraseña - Clini+",
                Body = cuerpoMail,
                IsBodyHtml = true
            };

            mensaje.To.Add(emailDestino);

            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailOrigen, password)
            };

            await smtp.SendMailAsync(mensaje);
        }
    }
}
