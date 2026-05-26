using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickleballBookingSystem.DTOs;
using PickleballBookingSystem.Interfaces;

namespace PickleballBookingSystem.Controllers;

[ApiController, Route("api/admin")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _admin;
    private readonly IBookingService _booking;

    public AdminController(IAdminService admin, IBookingService booking)
    {
        _admin = admin;
        _booking = booking;
    }

    [HttpGet("analytics")]
    public async Task<ActionResult<AnalyticsDto>> GetAnalytics()
    {
        var analytics = await _admin.GetAnalyticsAsync();
        return Ok(analytics);
    }

    [HttpGet("bookings")]
    public async Task<ActionResult<List<BookingDto>>> GetBookings()
    {
        var bookings = await _booking.GetAllBookingsAsync();
        return Ok(bookings);
    }

    [HttpPut("bookings/{id}")]
    public async Task<ActionResult<BookingDto>> UpdateBooking(Guid id, AdminUpdateBookingRequest request)
    {
        var booking = await _booking.AdminUpdateBookingAsync(id, request);
        return Ok(booking);
    }
}