using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.ContactUsRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthyFoodWebsite.Controllers
{
    public class ContactUsController : Controller, IController.IOperationalController<CustomerMessage>
    {
        // Object Fields Zone
        private readonly AbstractContactUsRepository contactUsRepository;


        // Dependency Injection Zone
        public ContactUsController(AbstractContactUsRepository contactUsRepository) =>
            this.contactUsRepository = contactUsRepository;


        // Object Methods Zone
        [Authorize(Roles = "BusinessOwner, Admin")]
        public async Task<IActionResult> GetAllAsync()
        {
            return View("Message", await contactUsRepository.GetAllAsync());
        }


        // Insertion Entrance
        [AllowAnonymous]
        public IActionResult InsertAsync()
        {
            ViewBag.MessageSent = -1;
            return View("ContactUs");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InsertAsync([Bind("UserName, PhoneNumber, Subject, Message")] CustomerMessage entity)
        {
            if (ModelState.IsValid)
            {
                if (await contactUsRepository.InsertAsync(entity))
                {
                    ViewBag.MessageSent = 1;
                    return View("ContactUs");
                }
            }

            ViewBag.MessageSent = 0;
            return View("ContactUs");
        }


        [Authorize(Roles = "BusinessOwner")]
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await contactUsRepository.GetByIdAsync(id);
            return await contactUsRepository.DeleteAsync(entity!);
        }
    }
}
