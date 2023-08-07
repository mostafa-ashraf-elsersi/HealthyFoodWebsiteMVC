using HealthyFoodWebsite.Models;
using HealthyFoodWebsite.Repositories.ContactUsRepository;
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
        public async Task<IActionResult> GetAllAsync()
        {
            return View("Message", await contactUsRepository.GetAllAsync());
        }


        // Insertion Entrance
        public IActionResult InsertAsync()
        {
            return View("ContactUs");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<bool> InsertAsync(CustomerMessage entity)
        {
            return await contactUsRepository.InsertAsync(entity);
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await contactUsRepository.GetByIdAsync(id);
            return await contactUsRepository.DeleteAsync(entity!);
        }
    }
}
