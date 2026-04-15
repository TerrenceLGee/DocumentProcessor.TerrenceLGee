using DocumentProcessor.Avalonia.TerrenceLGee.Models.EmailModels;

namespace DocumentProcessor.Avalonia.TerrenceLGee.Helpers;

public static class EmailHelper
{
    public static string GetFormattedEmailText(EmailData emailData, string senderName)
    {
        return $"""
            <!DOCKTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
            </head>
            <body style="margin:0; padding:0; background-color:#f4f4f4; font-family: Arial, sans-serif;">

            <table width="100%" cellpadding="0" cellspacing="0" style="background-color:#f4f4f4; padding: 30px 0;">
            <tr>
            <td align="center">
            <table width="700" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border-radius:8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);">

            <tr>
            <td style="background-color:#2C5F8A; padding: 24px 32px; border-radius: 8px 8px 0 0;">
            <h1 style="margin:0; color:#ffffff; font-size:20px; font-weight:600; text-align:center;">👋 Greetings</hi>
            </td>
            </tr>

            <tr>
            <td style="padding: 32px;">
            <p style="margin: 0 0 16px 0; font-size:15px; color:#333333;">Hello <strong>{emailData.ReceiverName}</strong>,</p>

            <p style="margin: 0 0 24px 0; font-size:15px; color:#333333; line-height:1.6;">{emailData.Body}</p>

            <p style="margin: 0; font-size:14px; color:#666666;">Sincerly yours,<br/><strong>{senderName}</strong></p>
            </td>
            </tr>

            <tr>
            <td style="background-color:#f9f9f9; padding: 16px 32px; border-top: 1px solid #eeeeee; border-radius: 0 0 8px 8px;">
            <p style="margin:0; font-size:12px; color:#999999; text-align:center;">This email was send from the Document Processor App</p>
            </td>
            </tr>
            </table>
            </td>
            </tr>
            </table>
            </body>
            </html>
            """;
    }
}
