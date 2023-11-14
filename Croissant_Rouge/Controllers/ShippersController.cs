using Croissant_Rouge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Croissant_Rouge.Utility;

namespace Croissant_Rouge.Controllers;

public class ShippersController : Controller

{
    private readonly MyContext _context;
    public ShippersController(MyContext context)
    {
        _context = context;
    }



    [HttpGet("/shipper/dashboard")]
    public IActionResult ShipperDashboard()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LogReg");
        }

        int? userId = (int)HttpContext.Session.GetInt32("userId");
        string? firstname = HttpContext.Session.GetString("FirstName");



       List <Donation> AllDonations =
            _context.Donations
            .Include(s => s.Donner)
            .Include(h => h.Shipment)
            .Where(s => s.status == StaticData.Status.Valid)
            .ToList();

        List<Shipment> AllShippments =
            _context.Shipments
            .Include(g=> g.Donation)
            .ThenInclude(h => h.Donner)
            .Where(s => s.UserId == userId && s.ShipStatus == StaticData.ShipStatus.Received)
            .ToList();


        ShipmentAndDonations vm = new ShipmentAndDonations();
        vm.Alldonations = AllDonations;
        vm.AllShippments = AllShippments;


        return View(vm);
    }


    [HttpPost("/shipdonation")]
    public IActionResult ShipDonation(Shipment newShippment)
    {
        if (ModelState.IsValid)
        {
            var userIdFromSession = HttpContext.Session.GetInt32("userId");
            //newShippment.UserId = (int)HttpContext.Session.GetInt32("userId");
            //1 Add 
            _context.Add(newShippment);

            //2 Save
            _context.SaveChanges();
            return RedirectToAction("ShipperDashboard");
        }
        return RedirectToAction("ShipperDashboard");

    }



    [HttpPost("/shipManydonation")]
    public IActionResult ShipManyDonations(List<Donation> donations)
    {
        Console.WriteLine($"{donations.Count()} *********************");
        //if (ModelState.IsValid)
        //{
        //    int? userId = (int)HttpContext.Session.GetInt32("userId");


        //    //1 Add 
        //    _context.Add(newShipper);

        //    //2 Save
        //    _context.SaveChanges();
        //    return RedirectToAction("ShipperDashboard");
        //}
        return RedirectToAction("ShipperDashboard");

    }









    [HttpPost("donations/{id}/validate")]
    public IActionResult AcceptShipping(int id)
    {
        Shipment? OneShipment = _context.Shipments
            .FirstOrDefault(shipment => shipment.ShipmentId == id);
        OneShipment.ShipStatus = StaticData.ShipStatus.Received;
        _context.SaveChanges();
        return RedirectToAction("ShipperDashboard");
    }



    // ************************* Create Shipper *******************

    [HttpPost("shipper/create")]
    public IActionResult CreateShipper(User newShipper)
    {
        if (ModelState.IsValid)
        {
            // Email Exist ?
            if (_context.Users.Any(u => u.Email == newShipper.Email))
            {
                // True
                ModelState.AddModelError("Email", "Email already in use .");
                return View("CreateShipper");
            }
            else
            {
                // False
                // 1 - Hash Password
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newShipper.Password = Hasher.HashPassword(newShipper, newShipper.Password);
                // 2 - Add 
                _context.Add(newShipper);
                // 3 - Save
                _context.SaveChanges();
                // HttpContext.Session.
                return RedirectToAction("ShipperList");
            }
        }
        return View("CreateShipper");
    }

    [HttpGet("shipper/create")]
    public IActionResult CreateShipper()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        return View("CreateShipper");
    }



    // ******************** All Shippers *************************

    [HttpGet("Allshippers")]
    public IActionResult ShipperList()
    {


        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LogReg", "Users");
        }
        List<User> ShipperList = _context.Users
            .Where(user=> user.Role == StaticData.Roles.Shipper)
            .ToList();
        return View("ShipperList", ShipperList);
    }


    //** ShippAll **


    [HttpGet("ShippAll")]
    public IActionResult ShippAll(int donnerId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LogReg", "Users");
        }
        List<Donation> userDonnations = _context.Donations
            .Where(donation => donation.UserId == donnerId && donation.status == StaticData.Status.Valid && donation.Shipment == null)
            .Include(donation => donation.Donner)
            .Include(donation => donation.Shipment)
            .ToList();
        return View(userDonnations);
    }


    //** Delete Shipper **

    [HttpPost("shipper/destroy")]
    public IActionResult DeleteShipper(int userId)
    {
        User? ShipperToDelete = _context.Users.FirstOrDefault(s => s.UserId == userId);
        _context.Users.Remove(ShipperToDelete);
        _context.SaveChanges();
        return RedirectToAction("ShipperList");
    }


}
