using Croissant_Rouge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Croissant_Rouge.Utility;

namespace Croissant_Rouge.Controllers;

public class DonationsController : Controller

{
    private readonly MyContext _context;
    public DonationsController(MyContext context)
    {
        _context = context;
    }

    //********************************************************************************* All Donations Filtred by Category For Admin

    [HttpGet("Donations")]
    public IActionResult AllDonations()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LogReg", "Users");
        }
        List<Donation> AllDonations = _context.Donations
    .Include(donnation => donnation.Donner)
    .Where(s => s.status == StaticData.Status.Unvalid)
    .ToList();

        return View(AllDonations);
    }



    //******************************************************************************************************************************

    [HttpGet("/donate")]
    public IActionResult Donate()
    {
     if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("");
        }
     return View();
    }


    //********************************************************************************* Show One Donation For Admin

    [HttpGet("donations/{donationId}")]
    public IActionResult OneDonation(int donationId)
    {
        Donation? OneDonation = _context.Donations
            .Include(donation => donation.Donner)
            .FirstOrDefault(donation => donation.DonationId == donationId);
        return View(OneDonation);
    }

    //********************************************************************************* Delete Donation For Admin
    [HttpPost("donation/destroy")]
    public IActionResult DeleteDonation(int donationId)
    {
        Donation? DonationToDelete = _context.Donations.FirstOrDefault(s => s.DonationId == donationId);
        _context.Donations.Remove(DonationToDelete);
        _context.SaveChanges();
        return RedirectToAction("AllDonations");
    }

    //********************************************************************************* Valid Donation For Admin

    [HttpPost("donation/{donationId}/validate")]
    public IActionResult ValidateDonation(int donationId)
    {
        Donation? OneDonation = _context.Donations
            .FirstOrDefault(donation => donation.DonationId == donationId);
        OneDonation.status = StaticData.Status.Valid;
        _context.SaveChanges();
        return RedirectToAction("ValidateDonations");
    }


    //********************************************************************************* List of Validate Donations For Admin

    [HttpGet("donations/validated")]
    public IActionResult ValidateDonations()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LogReg", "Users");
        }
        List<Donation> ValidateDonations = _context.Donations
            .Include(donnation => donnation.Donner)
            .Include(d => d.Shipment)
            .ThenInclude(b => b.Shipper)
            .Where(s => s.status == StaticData.Status.Valid)
            .ToList();
        return View(ValidateDonations);
    }






    //******************************************************* CREATE DONATION *****************************

    [HttpPost("Donation/create")]
    public IActionResult CreateDonation(Donation newDonation, IFormFile imageFile)
    {

        if (ModelState.IsValid)
        //    Console.WriteLine($"FORM---- {newDonation}");
        //Console.WriteLine($"IMAGE---- {imageFile}");
        {

            if (imageFile != null)
            {
                // Check the file format, size, or perform any other validation you need.
                if (IsImageValid(imageFile))
                {
                    // Generate a unique file name to avoid conflicts.
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    // Define the path where you want to save the uploaded image. Make sure this directory exists.
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads"); // The path to the "uploads" folder.

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fullPath = Path.Combine(path, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    // Save the image file path to the newDonation object.
                    newDonation.Picture = "/uploads/" + fileName;
                }
                else
                {
                    ModelState.AddModelError("imageFile", "Invalid image format or size.");
                    Console.WriteLine($"ERRORS---- {ModelState}");
                    return View("Donate");
                }
            }

            _context.Add(newDonation);
            _context.SaveChanges();
            return RedirectToAction("MyProfile","Users", new { DonationId = newDonation.DonationId });
        }

        return View("Donate");
    }

    private bool IsImageValid(IFormFile imageFile)
    {

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
        return allowedExtensions.Contains(fileExtension);
    }



}