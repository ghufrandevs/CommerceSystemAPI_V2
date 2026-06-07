namespace CommerceSystemAPI.Services
{
    public class EmailService
    {
        public void SendEmail(
            string to,
            string subject,
            string body)
        {
            Console.WriteLine($"Sending email to: {to}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");
        }
    }
}

