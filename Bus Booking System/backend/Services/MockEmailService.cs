namespace backend.Services
{
    public class MockEmailService : IEmailService
    {
        public Task SendBookingConfirmationAsync(string userEmail, string userName, string bookingId, string routeDetails, string seatDetails)
        {
            Console.WriteLine($"[MockEmail] To: {userEmail} | Subject: Booking Confirmed | Body: Hello {userName}, your booking {bookingId} for {routeDetails} with seats {seatDetails} is confirmed.");
            return Task.CompletedTask;
        }

        public Task SendBookingCancellationAsync(string userEmail, string userName, string bookingId, decimal refundAmount)
        {
            Console.WriteLine($"[MockEmail] To: {userEmail} | Subject: Booking Cancelled | Body: Hello {userName}, your booking {bookingId} has been cancelled. Refund: ${refundAmount:F2}");
            return Task.CompletedTask;
        }
    }
}
