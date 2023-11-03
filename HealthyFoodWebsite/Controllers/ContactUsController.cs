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
        [AllowAnonymous, Authorize(Roles = "User")]
        public IActionResult InsertAsync()
        {
            return View("ContactUs");
        }

        [AllowAnonymous, Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> InsertAsync([Bind("UserName, PhoneNumber, Subject, Message")] CustomerMessage entity)
        {
            if (ModelState.IsValid)
                return await contactUsRepository.InsertAsync(entity);
            else
                return false;
        }


        [Authorize(Roles = "BusinessOwner")]
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await contactUsRepository.GetByIdAsync(id);
            return await contactUsRepository.DeleteAsync(entity!);
        }
    }
}
