using System;
using Agullto_IMS.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Agullto_IMS.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(Product product, string recipientEmail)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["EmailSettings:FromName"],
                _configuration["EmailSettings:FromEmail"]
            ));
            message.To.Add(new MailboxAddress("Inventory Admin", recipientEmail));

            message.Subject = $"NEW PRODUCT ADDED: {product.Name}";
            message.Body = new TextPart("plain")
            {
                Text = $"A new product has been successfully added to Agullto IMS.\n\n" +
                       $"Product Details:\n" +
                       $" - Item Name: {product.Name}\n" +
                       $" - Stock Level: {product.Stock} {product.Unit}\n" +
                       $" - Weight Value: {product.WeightValue}\n" +
                       $" - Wholesale Cost: {product.CostPrice:C}\n" +
                       $" - Retail Price: {product.SellingPrice:C}\n" +
                       $" - Department: {product.Department}\n" +
                       $" - Location: {product.Location}\n\n" +
                       $"Timestamp: {DateTime.Now:f}"
            };

            using (var client = new SmtpClient())
            {
                client.Connect(
                    _configuration["EmailSettings:SmtpHost"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]),
                    SecureSocketOptions.StartTls
                );

                client.Authenticate(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]
                );

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}