using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.Helpers.Mapping;
using TaskManagement.API.Models.DTOs.AddRequest;
using TaskManagementAPI.Erorrs;
using TaskManagement.API.Models;
using TaskManagement.API.Repository;
using TaskManagement.API.Repository.Specifications;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme
        , Roles = "Admin")]
    public class TagController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string NotFoundMessage = "No Tag With this Id";
        public TagController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("GetTags")]
        public async Task<IActionResult> GetTagsAsync(
            [FromQuery] int? pageIndex, [FromQuery] int? pageSize,
            [FromQuery] string? sortType)
        {
            var specification = new TagsWithIdsSpecifications(sortType);            
            var count = await _unitOfWork.Tags.CountAsync(specification);

            specification.AddPagenation(pageIndex, pageSize);
            var tags = await _unitOfWork.Tags.ListAsync(specification);
            //return Ok(tags);
            var response = tags.ToResponse(pageIndex,pageSize,count);
            return Ok(response);
        }
        [HttpGet]
        [Route("GetTag/{id:int}")]
        public async Task<IActionResult> GetTagAsync(int id)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag == null)
                return NotFound(new ApiErrorResponse(404, NotFoundMessage));
            return Ok(tag);
        }
        [HttpPost]
        [Route("AddTag")]
        public async Task<IActionResult> AddTagAsync(TagDto dto)
        {
            var tag = new Tag { Name = dto.Name };
            tag = await _unitOfWork.Tags.AddAsync(tag);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { tag.Id, tag.Name });
        }
        [HttpPut]
        [Route("UpdateTag/{id:int}")]
        public async Task<IActionResult> UpdateTagAsync(int id, TagDto dto)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag == null)
                return NotFound(new ApiErrorResponse(404,NotFoundMessage));
            tag.Name = dto.Name;
            tag = _unitOfWork.Tags.Update(tag);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { tag.Id, tag.Name });
        }
        [HttpDelete]
        [Route("DeleteTag/{id:int}")]
        public async Task<IActionResult> DeleteTagAsync(int id)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(id);
            if (tag == null)
                return NotFound(new ApiErrorResponse(404, NotFoundMessage));
            _unitOfWork.Tags.Delete(tag);
            await _unitOfWork.SaveChangesAsync();
            return Ok("Tag Deleted Successfully");
        }

    }
}
