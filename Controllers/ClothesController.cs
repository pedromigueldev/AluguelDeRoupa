using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AluguelRoupa.Models;
using AluguelRoupa.DTO.Clothes;
using AluguelRoupa.AppDbContext;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AluguelRoupa.Controllers;

public class ClothesController(IDbContextFactory<Context> dbContextFactory, ILogger<ClothesController> Logger) : Controller
{
    public async Task<IActionResult> Index()
    {
        bool isHtmx = Request.Headers.ContainsKey("HX-Request");
        if (isHtmx)
        {
            using var _context = await dbContextFactory.CreateDbContextAsync();
            var all = await _context.Clothes.AsNoTracking().Take(10).OrderByDescending(e => e.Id).ToListAsync();
            return PartialView("_ClothesList", all);
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Extensions.ValidateObj]
    public async Task<IActionResult> Create (ClothesCreate clothesCreate) {
        try
        {
            using var _context = await dbContextFactory.CreateDbContextAsync();
            await _context.Clothes.AddAsync(Clothes.NewFromCreate(clothesCreate));
            await _context.SaveChangesAsync();
            var all = await _context.Clothes.AsNoTracking().Take(10).OrderByDescending(e => e.Id).ToListAsync();
            Response.Headers["HX-Trigger"] = "clothesChanged";
            return PartialView("_ClothesList", all);
        } catch (Exception ex)
        {
            Logger.LogError(ex, $"Error creating clothes: {clothesCreate}"); 
            return Error();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Extensions.ValidateObj]
    public async Task<IActionResult> Edit (ClothesCreate clothesCreate, Guid Id) {
        try
        {
            using var _context = await dbContextFactory.CreateDbContextAsync();
            var item = _context.Clothes.AsNoTracking().FirstOrDefault(e => e.Id == Id);
            if (item is null) return NotFound();

            _context.Clothes.Update(clothesCreate.NewFromCreateWithId(Id));
            await _context.SaveChangesAsync();
            Response.Headers["HX-Trigger"] = "clothesChanged";
            return PartialView("_CreateForm");
        } catch (Exception ex)
        {
            Logger.LogError(ex, $"Error editiing clothes: {clothesCreate}"); 
            return Error();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create () => PartialView("_CreateForm");
    [HttpGet]
    public async Task<IActionResult> Edit (Guid Id) {
        try
        {
            using var _context = await dbContextFactory.CreateDbContextAsync();
            var item = _context.Clothes.AsNoTracking().FirstOrDefault(e => e.Id == Id);
            if (item is null) 
                return Error();
            Response.Headers["HX-Trigger"] = "clothesChanged";
            return PartialView("_EditForm", item);
        } catch (Exception ex)
        {
            Logger.LogError(ex, $"Error creating clothes: {Id}"); 
            return Error();
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private string RenderValidationErrors(List<ValidationResult> errors)
    {
        var html = "<ul class='validation-errors'>";

        foreach (var error in errors)
            html += $"<li class='error'>{error.ErrorMessage}</li>";

        html += "</ul>";

        return html;
    }
}
