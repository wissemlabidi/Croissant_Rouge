using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Croissant_Rouge.Models;

public class MoneyController : Controller
{
    private readonly MyContext _context;

    public MoneyController(MyContext context)
    {
        _context = context;
    }

    public IActionResult OrderConfirmation()
    {
        var service = new SessionService();
        Session session = service.Get(TempData["Session"].ToString());
        if (session.PaymentStatus == "paid")
        {
            var transaction = session.PaymentIntentId.ToString();
            return View("Success");
        }
        {
            return View("Login");
        }
    }

    public IActionResult Success()
    {
        return View();
    }
    public IActionResult Login()
    {
        return View();
    }


    [HttpGet("/MoneyDonation")]
    public IActionResult Money()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("LogReg");
        }
        return View();
    }

    [HttpPost]
    public IActionResult CreateMoney(int userId, int amount)
    {
        // Create a Money object and save it to your database
        var donation = new Money
        {
            UserId = userId,
            Amount = amount,
        };
        _context.Moneys.Add(donation);
        _context.SaveChanges();

        // Initialize a Stripe payment session
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = amount * 100, // Amount in cents
                        Currency = "usd", // Change to your desired currency
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Donation",
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = Url.Action("OrderConfirmation", "Money", null, Request.Scheme),
            CancelUrl = "https://your-website.com/cancel",
        };

        var service = new SessionService();
        var session = service.Create(options);

        TempData["Session"] = session.Id;
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
    }
}