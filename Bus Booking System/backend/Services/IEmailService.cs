namespace backend.Services
{
    public interface IEmailService
    {
        Task SendBookingConfirmationAsync(string userEmail, string userName, string bookingId, string routeDetails, string seatDetails);
        Task SendBookingCancellationAsync(string userEmail, string userName, string bookingId, decimal refundAmount);
    }
}
