using Microsoft.EntityFrameworkCore;
using PickleballBookingSystem.Data;
using PickleballBookingSystem.DTOs;
using PickleballBookingSystem.Interfaces;

namespace PickleballBookingSystem.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;

    public AdminService(AppDbContext db) => _db = db;

    public async Task<AnalyticsDto> GetAnalyticsAsync()
    {
        var totalRevenue = await _db.Bookings
            .Where(b => b.Status == "confirmed" || b.Status == "completed")
            .SumAsync(b => (decimal?)b.TotalAmount) ?? 0;

        var totalBookings = await _db.Bookings.CountAsync();
        var activeUsers = await _db.Bookings.Select(b => b.CustomerEmail).Distinct().CountAsync();

        var confirmedBookings = await _db.Bookings
            .Where(b => b.Status == "confirmed" || b.Status == "completed")
            .Select(b => new { b.Date, b.TotalAmount })
            .ToListAsync();

        var revenueByDay = confirmedBookings
            .GroupBy(b => b.Date.Date)
            .Select(g => new RevenueByDayDto(g.Key.ToString("yyyy-MM-dd"), g.Sum(b => b.TotalAmount)))
            .OrderBy(r => r.Date).TakeLast(30).ToList();

        var allBookingDates = await _db.Bookings.Select(b => b.Date).ToListAsync();

        var bookingsByDay = allBookingDates
            .GroupBy(d => d.Date)
            .Select(g => new BookingsByDayDto(g.Key.ToString("yyyy-MM-dd"), g.Count()))
            .OrderBy(b => b.Date).TakeLast(30).ToList();

        return new AnalyticsDto(totalRevenue, totalBookings, activeUsers, revenueByDay, bookingsByDay, 12.5, 8.3, 5.1);
    }
}