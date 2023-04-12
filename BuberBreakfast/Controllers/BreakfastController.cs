using BuberBreakfast.Contracts.Breakfast;
using BuberBreakfast.Models;
using BuberBreakfast.ServiceErrors;
using BuberBreakfast.Services.Breakfasts;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
namespace BuberBreakfast.Controllers;


public class BreakfastController : ApiController
{
    private readonly IBreakfastService _breakfastService;

    public BreakfastController(IBreakfastService breakfastService){
        _breakfastService = breakfastService;
    }

    [HttpPost()]

    public IActionResult CreateBreakfast(CreateBreakfastRequest request)
    {
        var breakfast = new Breakfast(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.StartDateTime,
            request.EndDateTime,
            DateTime.UtcNow,
            request.Savory,
            request.Sweet);

        ErrorOr<Created> createdBreakfastResult = _breakfastService.CreateBreakfast(breakfast);

       return createdBreakfastResult.Match(
        created => CreatedAtGetBreakfast(breakfast),
        errors => Problem(errors)
       );
    }

    private IActionResult CreatedAtGetBreakfast(Breakfast breakfast)
    {
        return CreatedAtAction(
            nameof(GetBreakfast),
            new { id = breakfast.Id },
            MapBreakfastResponse(breakfast));
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetBreakfast(Guid id)
    {
        ErrorOr<Breakfast> getBreakfastResult = _breakfastService.GetBreakfast(id);

        return getBreakfastResult.Match(
            breakfast => Ok(MapBreakfastResponse(breakfast)),
            errors => Problem(errors)
        );

        // if (getBreakfastResult.IsError &&
        //     getBreakfastResult.FirstError == Errors.Breakfast.NotFound)
        // {
        //     return NotFound();
        // }

        // var breakfast = getBreakfastResult.Value;
        // BreakfastResponse response = MapBreakfastResponse(breakfast);
        // return Ok(response);
    }

    private static BreakfastResponse MapBreakfastResponse(Breakfast breakfast)
    {
        return new BreakfastResponse(
                    breakfast.Id,
                    breakfast.Name,
                    breakfast.Description,
                    breakfast.StartDateTime,
                    breakfast.EndDateTime,
                    breakfast.LastModifiedDateTime,
                    breakfast.Savory,
                    breakfast.Sweet
                );
    }

    [HttpPut("{id:guid}")]
    public IActionResult UpsertBreakfast(Guid id, UpsertBreakfastRequest request)
    {
        var breakfast = new Breakfast(
            id,
            request.Name,
            request.Description,
            request.StartDateTime,
            request.EndDateTime,
            DateTime.UtcNow,
            request.Savory,
            request.Sweet
        );

        ErrorOr<UpsertBreakfast> upsertedBreakfastResult = _breakfastService.UpsertBreakfast(breakfast);
        return upsertedBreakfastResult.Match(
            upserted => upserted.IsNewlyCreated ? CreatedAtGetBreakfast(breakfast)  : NoContent(),
            errors => Problem(errors)
        );
    }

    [HttpDelete("{id:guid}")]
    public IActionResult DeleteBreakfast(Guid id)
    {

        ErrorOr<Deleted> deletedBreakfastResult = _breakfastService.DeleteBreakfast(id);
        return deletedBreakfastResult.Match(
            deleted => NoContent(),
            errors => Problem(errors)
        );
    }
    
}
 
