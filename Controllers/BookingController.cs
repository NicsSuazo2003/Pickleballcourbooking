using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickleballBookingSystem.DTOs;
using PickleballBookingSystem.Interfaces;

namespace PickleballBookingSystem.Controllers;

[ApiController, Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _booking;

    public BookingController(IBookingService booking) => _booking = booking;

    [HttpPost]
    public async Task<ActionResult<BookingDto>> Create(CreateBookingRequest request)
    {
        var booking = await _booking.CreateBookingAsync(request);
        return CreatedAtAction(nameof(Track), new { referenceCode = booking.ReferenceCode }, booking);
    }

    [HttpGet("track/{referenceCode}")]
    public async Task<ActionResult<BookingDto>> Track(string referenceCode, [FromQuery] string email)
    {
        var booking = await _booking.TrackBookingAsync(referenceCode, email);
        if (booking == null) return NotFound(new { message = "Booking not found" });
        return Ok(booking);
    }

    [Authorize(Roles = "admin"), HttpGet("admin")]
    public async Task<ActionResult<List<BookingDto>>> GetAll()
    {
        var bookings = await _booking.GetAllBookingsAsync();
        return Ok(bookings);
    }

    [Authorize(Roles = "admin"), HttpPut("admin/{id}")]
    public async Task<ActionResult<BookingDto>> AdminUpdate(Guid id, AdminUpdateBookingRequest request)
    {
        var booking = await _booking.AdminUpdateBookingAsync(id, request);
        return Ok(booking);
    }
}