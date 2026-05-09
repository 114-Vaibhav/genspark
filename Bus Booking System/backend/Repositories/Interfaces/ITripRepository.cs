using backend.Models;
using backend.DTOs;

namespace backend.Repositories.Interfaces
{
    public interface ITripRepository
    {
        Task CreateTripAsync(Trip trip);

        Task<bool> BusExistsAsync(Guid busId);

        Task<double?> GetRouteDistanceAsync(Guid routeId);

        Task<TripSearchResponse?> GetTripByIdAsync(Guid tripId);

        Task<List<TripSearchResponse>> SearchTripsAsync(
            string source,
            string destination,
            DateTime date);
    }
}