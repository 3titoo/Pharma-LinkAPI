using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.DTO.AccountDTO;
using Pharma_LinkAPI.Identity;
using Pharma_LinkAPI.Models;
using Pharma_LinkAPI.Repositries.Irepositry;

namespace Pharma_LinkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IreviewRepositiry _ireviewRepositiry;
        private readonly IUnitOfWork _unitOfWork;
        public ReviewController(IreviewRepositiry ireviewRepositiry, IUnitOfWork unitOfWork)
        {
            _ireviewRepositiry = ireviewRepositiry;
            _unitOfWork = unitOfWork;
        }
        [Authorize(Roles = SD.Role_Pharmacy)]
        [HttpPost("{CompanyId}")]
        public async Task<IActionResult> AddReview(int CompanyId, ReviewDTO review)
        {
            var ph = await _unitOfWork._accountRepositry.GetCurrentUser(User);
            var exist = await _ireviewRepositiry.GetReviewByphAndCo(ph.Id, CompanyId);
            if (exist != null)
            {
                await _ireviewRepositiry.Delete(exist.Id);
            }
            var rev = new Review
            {
                PharmacyId = ph.Id,
                CompanyId = CompanyId,
                Rating = review.Rating.Value,
                Comment = review.Review
            };
            await _ireviewRepositiry.Add(rev);
            return Ok();
        }
        [Authorize]
        [HttpGet("{CompanyId}")]
        public async Task<ActionResult<ReviewDTO>> GetReview(int CompanyId)
        {
            var ph = await _unitOfWork._accountRepositry.GetCurrentUser(User);
            if (ph == null)
            {
                return Problem("Pharmacy not found");
            }
            var review = await _ireviewRepositiry.GetReviewByphAndCo(ph.Id, CompanyId);
            if (review == null)
            {
                return NoContent();
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
