﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pharma_LinkAPI.DTO.AccountDTO;
using Pharma_LinkAPI.Identity;
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
        [Authorize(Roles = SD.Role_Pharmacy)]
        [HttpPost("{CompanyId}")]
        public async Task<IActionResult> AddReview(int CompanyId, ReviewDTO review)
        {
            var ph = await _accountRepositry.GetCurrentUser(User);
            var exist =  ireviewRepositiry.GetReviewByphAndCo(ph.Id, CompanyId);
            if(exist != null)
            {
                ireviewRepositiry.Delete(exist.Id);
            }
            var rev = new Review
            {
                PharmacyId = ph.Id,
                CompanyId = CompanyId,
                Rating = review.Rating.Value,
                Comment = review.Review
            };
            ireviewRepositiry.Add(rev);
            return Ok();
        }
        [Authorize]
        [HttpGet("{CompanyId}")]
        public async Task<ActionResult<ReviewDTO>> GetReview(int CompanyId)
        {
            var ph = await _accountRepositry.GetCurrentUser(User);
            if (ph == null)
            {
                return Problem("Pharmacy not found");
            }
            var review = ireviewRepositiry.GetReviewByphAndCo(ph.Id, CompanyId);
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
