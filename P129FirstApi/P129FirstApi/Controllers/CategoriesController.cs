using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P129FirstApi.Data;
using P129FirstApi.Data.Entities;
using P129FirstApi.DTOs.CatagoryDTOs;
using P129FirstApi.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P129FirstApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CategoryPostDto categoryPostDto)
        {
            if (categoryPostDto.AidOlduguUstCategoryId != null && !await _context.Categories.AnyAsync(c => c.Id == categoryPostDto.AidOlduguUstCategoryId && c.IsMain))
            {
                return BadRequest("ParentId Is InCorrect");
            }

            if (await _context.Categories.AnyAsync(c=>!c.IsDeleted && c.Name.ToLower() == categoryPostDto.Ad.Trim().ToLower()))
            {
                return Conflict($"Category {categoryPostDto.Ad} Is Already Exists");
            }

            //Category category = new Category();

            //category.Name = categoryPostDto.Ad.Trim();
            //category.CreatedAt = DateTime.UtcNow.AddHours(4);
            //category.IsMain = categoryPostDto.Esasdirmi;
            //category.ParentId = categoryPostDto.AidOlduguUstCategoryId;
            //category.Image = categoryPostDto.Esasdirmi ? categoryPostDto.Sekli : "1.jpg";

            Category category = _mapper.Map<Category>(categoryPostDto);

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            //CategoryGetDto categoryGetDto = new CategoryGetDto
            //{
            //    Ad = category.Name,
            //    Sekil = category.Image,
            //    AidOlduguUstCategoryId = category.ParentId,
            //    Esasdirmi = category.IsMain,
            //    Id = category.Id
            //};

            CategoryGetDto categoryGetDto = _mapper.Map<CategoryGetDto>(category);

            return StatusCode(201, categoryGetDto);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //Old Mapping
            //List<Category> categories = await _context.Categories.Where(c => !c.IsDeleted).ToListAsync();
            //List<CategoryListDto> categoryListDtos = new List<CategoryListDto>();

            //foreach (Category category in categories)
            //{
            //    CategoryListDto categoryListDto = new CategoryListDto
            //    {
            //        Id = category.Id,
            //        Ad = category.Name
            //    };

            //    categoryListDtos.Add(categoryListDto);
            //}

            //Old Mapping
            //List<CategoryListDto> categoryListDtos = await _context.Categories
            //    .Where(c => !c.IsDeleted)
            //    .Select(x=>new CategoryListDto
            //    {
            //        Id = x.Id,
            //        Ad = x.Name
            //    })
            //    .ToListAsync();

            //List<CategoryListDto> categoryListDtos = _mapper.Map<List<CategoryListDto>>(await _context.Categories.Where(c => !c.IsDeleted).ToListAsync());

            return Ok(_mapper.Map<List<CategoryListDto>>(await _context.Categories.Where(c => !c.IsDeleted).ToListAsync()));
        }

        [HttpGet]
        [Route("{id?}")]
        public async Task<IActionResult> Get(int? id)
        {
            CategoryGetDto categoryGetDto = await _context.Categories
                .Where(c => !c.IsDeleted && c.Id == id)
                .Select(x=>new CategoryGetDto 
                {
                    Id = x.Id,
                    Ad = x.Name,
                    AidOlduguUstCategoryId = x.ParentId,
                    Esasdirmi = x.IsMain,
                    Sekil = x.Image
                })
                .FirstOrDefaultAsync();

            if (categoryGetDto == null) return NotFound("Id Is InCorrect");

            //CategoryGetDto categoryGetDto = new CategoryGetDto
            //{
            //    Id = category.Id,
            //    Ad = category.Name,
            //    AidOlduguUstCategoryId = category.ParentId,
            //    Esasdirmi = category.IsMain,
            //    Sekil = category.Image
            //};

            return Ok(categoryGetDto);
        }

        [HttpPut]
        [Route("{id?}")]
        public async Task<IActionResult> Put(int? id,CategoryPutDto categoryPutDto)
        {
            if (id == null) return BadRequest("id Is required");

            if (categoryPutDto.Id != id) return BadRequest("Id Is Not Mathed By Category Object");

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);

            if (dbCategory == null) return NotFound("Id Is InCorrect");

            if (categoryPutDto.AidOlduguUstCategoryId != null && !await _context.Categories.AnyAsync(c => c.Id == categoryPutDto.AidOlduguUstCategoryId && c.IsMain))
            {
                return BadRequest("AidOlduguUstCategoryId Is InCorrect");
            }

            if (await _context.Categories.AnyAsync(c => !c.IsDeleted && c.Id != id &&  c.Name.ToLower() == categoryPutDto.Ad.Trim().ToLower()))
            {
                return Conflict($"Category {categoryPutDto.Ad} Is Already Exists");
            }

            dbCategory.Name = categoryPutDto.Ad.Trim();
            dbCategory.IsMain = categoryPutDto.Esasdirmi;
            dbCategory.ParentId = categoryPutDto.AidOlduguUstCategoryId;
            dbCategory.Image = categoryPutDto.AidOlduguUstCategoryId != null? "1.jpg":categoryPutDto.Sekli;
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        [Route("{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest("id Is required");

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);

            if (dbCategory == null) return NotFound("Id Is InCorrect");

            dbCategory.IsDeleted = true;
            dbCategory.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpOptions]
        [Route("restore/{id?}")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null) return BadRequest("id Is required");

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted && c.Id == id);

            if (dbCategory == null) return NotFound("Id Is InCorrect");

            dbCategory.IsDeleted = false;
            dbCategory.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
