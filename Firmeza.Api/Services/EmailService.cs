using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Firmeza.Api.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    string GeneratePurchaseReceipt(string customerName, int orderId, DateTime orderDate, decimal total, decimal vat, List<(string productName, int quantity, decimal unitPrice, decimal subtotal)> items);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var smtpHost = _configuration["Smtp:Host"];
        var smtpPort = int.Parse(_configuration["Smtp:Port"]);
        var smtpUsername = _configuration["Smtp:Username"];
        var smtpPassword = _configuration["Smtp:Password"];
        var fromName = _configuration["Smtp:FromName"] ?? "Firmeza";

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(fromName, smtpUsername));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = body };

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(smtpUsername, smtpPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            Console.WriteLine($"✅ Email sent successfully to {to}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to send email: {ex.Message}");
            throw;
        }
    }

    public string GeneratePurchaseReceipt(string customerName, int orderId, DateTime orderDate, decimal total, decimal vat, List<(string productName, int quantity, decimal unitPrice, decimal subtotal)> items)
    {
        var itemsHtml = string.Join("", items.Select(item => $@"
            <tr>
                <td style='padding: 12px; border-bottom: 1px solid #e2e8f0;'>{item.productName}</td>
                <td style='padding: 12px; border-bottom: 1px solid #e2e8f0; text-align: center;'>{item.quantity}</td>
                <td style='padding: 12px; border-bottom: 1px solid #e2e8f0; text-align: right;'>${item.unitPrice:N0}</td>
                <td style='padding: 12px; border-bottom: 1px solid #e2e8f0; text-align: right; font-weight: 600;'>${item.subtotal:N0}</td>
            </tr>
        "));

        var subtotal = total - vat;

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; background-color: #f1f5f9;'>
    <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f1f5f9; padding: 40px 20px;'>
        <tr>
            <td align='center'>
                <table width='600' cellpadding='0' cellspacing='0' style='background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 40px 30px; text-align: center;'>
                            <h1 style='margin: 0; color: #ffffff; font-size: 32px; font-weight: bold;'>Firmeza</h1>
                            <p style='margin: 10px 0 0 0; color: #e0e7ff; font-size: 14px;'>Comprobante de Compra</p>
                        </td>
                    </tr>
                    
                    <!-- Greeting -->
                    <tr>
                        <td style='padding: 30px 30px 20px 30px;'>
                            <h2 style='margin: 0 0 10px 0; color: #1e293b; font-size: 24px;'>¡Gracias por tu compra, {customerName}!</h2>
                            <p style='margin: 0; color: #64748b; font-size: 16px;'>Tu orden ha sido procesada exitosamente.</p>
                        </td>
                    </tr>
                    
                    <!-- Order Info -->
                    <tr>
                        <td style='padding: 0 30px 20px 30px;'>
                            <table width='100%' cellpadding='0' cellspacing='0' style='background-color: #f8fafc; border-radius: 8px; padding: 20px;'>
                                <tr>
                                    <td style='padding-bottom: 10px;'>
                                        <strong style='color: #475569;'>Número de Orden:</strong>
                                        <span style='color: #667eea; font-weight: bold; font-size: 18px;'>#{orderId}</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <strong style='color: #475569;'>Fecha:</strong>
                                        <span style='color: #1e293b;'>{orderDate:dd/MM/yyyy HH:mm}</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Products Table -->
                    <tr>
                        <td style='padding: 0 30px 30px 30px;'>
                            <h3 style='margin: 0 0 15px 0; color: #1e293b; font-size: 18px;'>Detalles de la Orden</h3>
                            <table width='100%' cellpadding='0' cellspacing='0' style='border: 1px solid #e2e8f0; border-radius: 8px; overflow: hidden;'>
                                <thead>
                                    <tr style='background-color: #f8fafc;'>
                                        <th style='padding: 12px; text-align: left; color: #475569; font-weight: 600; border-bottom: 2px solid #e2e8f0;'>Producto</th>
                                        <th style='padding: 12px; text-align: center; color: #475569; font-weight: 600; border-bottom: 2px solid #e2e8f0;'>Cantidad</th>
                                        <th style='padding: 12px; text-align: right; color: #475569; font-weight: 600; border-bottom: 2px solid #e2e8f0;'>Precio Unit.</th>
                                        <th style='padding: 12px; text-align: right; color: #475569; font-weight: 600; border-bottom: 2px solid #e2e8f0;'>Subtotal</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {itemsHtml}
                                </tbody>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Totals -->
                    <tr>
                        <td style='padding: 0 30px 30px 30px;'>
                            <table width='100%' cellpadding='0' cellspacing='0'>
                                <tr>
                                    <td style='padding: 8px 0; text-align: right; color: #64748b;'>Subtotal:</td>
                                    <td style='padding: 8px 0 8px 20px; text-align: right; font-weight: 600; color: #1e293b; width: 120px;'>${subtotal:N0}</td>
                                </tr>
                                <tr>
                                    <td style='padding: 8px 0; text-align: right; color: #64748b;'>IVA (19%):</td>
                                    <td style='padding: 8px 0 8px 20px; text-align: right; font-weight: 600; color: #1e293b;'>${vat:N0}</td>
                                </tr>
                                <tr style='border-top: 2px solid #e2e8f0;'>
                                    <td style='padding: 15px 0 0 0; text-align: right; color: #1e293b; font-size: 18px; font-weight: bold;'>Total:</td>
                                    <td style='padding: 15px 0 0 20px; text-align: right; font-size: 24px; font-weight: bold; color: #667eea;'>${total:N0}</td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background-color: #f8fafc; padding: 30px; text-align: center; border-top: 1px solid #e2e8f0;'>
                            <p style='margin: 0 0 10px 0; color: #64748b; font-size: 14px;'>Gracias por confiar en Firmeza</p>
                            <p style='margin: 0; color: #94a3b8; font-size: 12px;'>Este es un correo automático, por favor no responder.</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
