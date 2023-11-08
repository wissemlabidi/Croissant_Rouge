using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Croissant_Rouge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


namespace Croissant_Rouge.Controllers;
public class UsersController : Controller
{
    private readonly MyContext _context;
    public UsersController(MyContext context)
    {
        _context = context;
    }

    [HttpGet("/logreg")]
    public IActionResult LogReg()
    {
        if (HttpContext.Session.GetInt32("userId") == null )
        {
            return View();
        }
        return RedirectToAction("Dashboard");
    }


    [HttpGet("/categories")]
    public IActionResult Categories()

    {
        int? userId = HttpContext.Session.GetInt32("userId");
        if (userId != null)
        {
            User? conectedUser = _context.Users.FirstOrDefault(user => user.UserId == userId);
            return View(conectedUser);
        }
        return View();
    }

    [HttpGet("")]
    public IActionResult Home()
    {

        int? userId = HttpContext.Session.GetInt32("userId");
        if(userId != null)
        {
        User? conectedUser = _context.Users.FirstOrDefault(user => user.UserId == userId);
        return View(conectedUser);
        }
        return View();
    }



    [HttpGet("/dashboard")]
    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetInt32("userId") == null )
        {
            return RedirectToAction("LogReg");
        }
        int? userId = (int)HttpContext.Session.GetInt32("userId");
        return View();
    }



    // *********************************** USER PROFILE **************************

    [HttpGet("/profile")]
    public IActionResult MyProfile()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LogReg");
        }
        int? userId = (int)HttpContext.Session.GetInt32("userId");
        List<Donation> AllDonations = _context.Donations
            .Include(c => c.Donner)
            .Include(c => c.Shipment)
            .ThenInclude(c => c.Shipper)
            .Where(s => s.UserId == userId)
            .ToList();
        return View(AllDonations);
    }



    [HttpGet("/EditProfile")]
    public IActionResult EditProfile()
    {
        int? userId = HttpContext.Session.GetInt32("userId");

        User UserToUpdate = _context.Users.FirstOrDefault(c => c.UserId == userId);

        if (UserToUpdate == null)
        {
            return View("Error");
        }

        return View(UserToUpdate);
    }




    [HttpPost()]
    public IActionResult EditProfile2(User editedProfile)
    {
        User UserToUpdate = _context.Users.FirstOrDefault(d => d.UserId == editedProfile.UserId);




        UserToUpdate.FirstName = editedProfile.FirstName;
        UserToUpdate.LastName = editedProfile.LastName;
        UserToUpdate.Email = editedProfile.Email;
        UserToUpdate.Phone = editedProfile.Phone;
        UserToUpdate.Address = editedProfile.Address;
        UserToUpdate.UpdatedAt = DateTime.Now;

        _context.SaveChanges();
        return RedirectToAction("MyProfile", "Users");


        // If ModelState is not valid, return to the edit view with the model
        return View("EditProfile", UserToUpdate);
    }




    //-------------------- Login ---------------------
    [HttpPost("users/login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if (ModelState.IsValid)
        {
            // User Registered ?
            User? userFromDb = _context.Users.FirstOrDefault(u => u.Email == loginUser.LoginEmail);
            if (userFromDb is null)
            {
                ModelState.AddModelError("LoginEmail", "Email dose not exist !");
                return View("LogReg");
            }
            else
            {
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(loginUser, userFromDb.Password, loginUser.LoginPassword);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("LoginPassword", "Wrong Password !");
                    return View("LogReg");
                }
                else
                {

                    HttpContext.Session.SetInt32("userId", userFromDb.UserId);
                    HttpContext.Session.SetString("FirstName", userFromDb.FirstName);

                    User? user = _context.Users.FirstOrDefault(c => c.UserId == userFromDb.UserId);
                    if (user.Role.ToString() == "Admin")
                    {
                        return RedirectToAction("AllDonations", "Donations");
                    }
                    else if (user.Role.ToString() == "Shipper")
                    {
                        
                        return RedirectToAction("ShipperDashboard", "Shippers");
                    }
                    else
                    {
                        return RedirectToAction("Categories");
                    }
                }
            }
        }
        return View("LogReg");

    }

    // ------------- Logout --------------------
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("LogReg");
    }

    //-------------------- Register ---------------------
    [HttpPost("users/create")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            // Email Exist ?
            if (_context.Users.Any(u => u.Email == newUser.Email))
            {
                // True
                ModelState.AddModelError("Email", "Email already in use .");
                return View("LogReg");
            }
            else
            {
                // False
                // 1 - Hash Password
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                // 2 - Add 
                _context.Add(newUser);
                // 3 - Save
                _context.SaveChanges();
                HttpContext.Session.SetInt32("userId", newUser.UserId);
                HttpContext.Session.SetString("FirstName", newUser.FirstName);
                // HttpContext.Session.
                return RedirectToAction("Categories");
            }
        }
        return View("LogReg");
    }

}
