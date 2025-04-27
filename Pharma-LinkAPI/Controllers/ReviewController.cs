using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.DTO.AccountDTO;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;
using Pharma_LinkAPI.ViewModels;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IreviewRepositiry ireviewRepositiry;
        private readonly IAccountRepositry _accountRepositry;
        public ReviewController(IreviewRepositiry ireviewRepositiry, IAccountRepositry accountRepositry)
        {
            this.ireviewRepositiry = ireviewRepositiry;
            _accountRepositry = accountRepositry;
        }
        [HttpPost("{CompanyId}")]
        public async Task<ActionResult<Review>> AddReview(int CompanyId, Review review)
        {
            var ph = await _accountRepositry.GetCurrentUser(User);
            review.PharmacyId = ph.Id;
            var exist = await ireviewRepositiry.GetReviewByphAndCo(ph.Id, CompanyId);
            if(exist != null)
            {
                ireviewRepositiry.Delete(exist.Id);
            }
            ireviewRepositiry.Add(review);
            return Ok();
        }
        [HttpGet("{CompanyId}")]
        public async Task<ActionResult<ReviewDTO>> GetReview(int CompanyId)
        {
            var ph = await _accountRepositry.GetCurrentUser(User);
            var review = await ireviewRepositiry.GetReviewByphAndCo(ph.Id, CompanyId);
            if (review == null)
            {
                return NotFound();
            }
            var reviewDTO = new ReviewDTO
            {
                Rating = review.Rating,
                Review = review.Comment,
                ReviewerName = ph.Name,
                ReviewerEmail = ph.Email
            };
            return Ok(reviewDTO);
        }
    }
}
