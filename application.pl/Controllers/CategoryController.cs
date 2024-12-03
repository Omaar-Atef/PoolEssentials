using DataLayer.Entities;
using DataLayer.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Buss.Layer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace application.pl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> CategoryRepo;
        private readonly IRepository<BrandCategory> BrandCategoryRepo;
        private readonly IMapper Mapper;


        public CategoryController(IRepository<Category> categoryRepo , IMapper mapper, IRepository<BrandCategory> brandCategoryRepo)
        {
            CategoryRepo = categoryRepo;
            Mapper = mapper;
            BrandCategoryRepo = brandCategoryRepo;

        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await CategoryRepo.GetAll();
            var CategoriesDTO = Mapper.Map<IEnumerable<CategoryDTO>>(categories);


            foreach (var category in CategoriesDTO) 
            {
                var brandCategoriesList = await BrandCategoryRepo.Get(c => c.CategoryID == category.CategoryID);

                if (category.BrandIDs == null)
                {
                    category.BrandIDs = new List<int>();
                }

                foreach (var BrandCategory in brandCategoriesList)
                {
                    category.BrandIDs.Add(BrandCategory.BrandID);
                }
            }

            if (categories == null)
                return BadRequest();
            return Ok(CategoriesDTO);
        }

        //Pagenation

        [HttpGet("{pagenumber:int}/{pagesize:int}")]
        public async Task<IActionResult> GetAll(int pagenumber, int pagesize)
        {
            var categories = await CategoryRepo.GetAll(pagenumber, pagesize);
            var CategoriesDTO = Mapper.Map<IEnumerable<CategoryDTO>>(categories);

            foreach (var category in CategoriesDTO)
            {
                var brandCategoriesList = await BrandCategoryRepo.Get(c => c.CategoryID == category.CategoryID);

                if (category.BrandIDs == null)
                {
                    category.BrandIDs = new List<int>();
                }

                foreach (var BrandCategory in brandCategoriesList)
                {
                    category.BrandIDs.Add(BrandCategory.BrandID);
                }
            }

            var count = CategoryRepo.GetCount();

            if (categories == null)
                return BadRequest();
            return Ok(new { Count = count, Categories =  CategoriesDTO});
        }

        [HttpGet("{brandid:int}/{pagenumber:int}/{pagesize:int}")]
        public async Task<IActionResult> GetAllBrandCategories(int brandid, int pagenumber, int pagesize)
        {
            var AllBrandCategories = await BrandCategoryRepo.Get(x => x.BrandID == brandid);
            var BrandCategories = AllBrandCategories.Skip((pagenumber - 1) * pagesize).Take(pagesize);
            List<Category> categories = new List<Category>();

            foreach(var BrandCategory in BrandCategories)
            {
                var category = await CategoryRepo.GetById(BrandCategory.CategoryID);
                if(category != null)
                {
                    categories.Add(category);
                }
            }
            var CategoriesDTO = Mapper.Map<IEnumerable<CategoryDTO>>(categories);
            var count = AllBrandCategories.Count();


            if (categories == null)
                return BadRequest();
            return Ok(new { Count = count, Categories = CategoriesDTO });

        }

        [HttpGet("{id:int}", Name = "GetCategoryByIdRoute")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await CategoryRepo.GetById(id);
            var categoryDTO = Mapper.Map<CategoryDTO>(category);

           
                var brandCategoriesList = await BrandCategoryRepo.Get(c => c.CategoryID == categoryDTO.CategoryID);

                if (categoryDTO.BrandIDs == null)
                {
                    categoryDTO.BrandIDs = new List<int>();
                }

                foreach (var BrandCategory in brandCategoriesList)
                {
                    categoryDTO.BrandIDs.Add(BrandCategory.BrandID);
                }
           

            if (category == null)
                return NotFound("Category not found");
            return Ok(categoryDTO);
        }


        [HttpPost("/api/Category/Upload")]
        public IActionResult Upload(IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;

                string filePath = Path.Combine(wwwRootPath, "uploads", uniqueFileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

                return Ok(new { Message = "File uploaded successfully", FileName = uniqueFileName });
            }
            return BadRequest("Invalid image file.");
        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryDTO categoryDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Category category = Mapper.Map<Category>(categoryDTO);

                    await CategoryRepo.Insert(category);

                    if (categoryDTO.BrandIDs != null)
                    {
                        foreach (var brandId in categoryDTO.BrandIDs)
                        {
                            var BrandCategory = new BrandCategory() { CategoryID = category.CategoryID, BrandID = brandId };
                            await BrandCategoryRepo.Insert(BrandCategory);
                        }
                    }

                    string url = Url.Link("GetCategoryByIdRoute", new { id = category.CategoryID })!;
                    return Created(url, category);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        public async Task<IActionResult> Update(CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCategory = await CategoryRepo.GetById(categoryDTO.CategoryID);

            if (existingCategory == null)
            {
                return NotFound("Category not found");
            }
     
            Mapper.Map(categoryDTO, existingCategory);

            try
            {
                var ToBeModifiedList = await BrandCategoryRepo.Get(c => c.CategoryID == categoryDTO.CategoryID);
                foreach (var element in ToBeModifiedList)
                {
                    await BrandCategoryRepo.Delete(element);
                }


                await CategoryRepo.Update(existingCategory);


                if (categoryDTO.BrandIDs.Count != 0)
                    foreach (var brandID in categoryDTO.BrandIDs)
                    {
                        var ToBeAdded = new BrandCategory() { BrandID = brandID, CategoryID = categoryDTO.CategoryID };
                        await BrandCategoryRepo.Insert(ToBeAdded);
                    }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var Categories = await CategoryRepo.Get(x => x.CategoryName == name);
            var CategoriesDTO = Mapper.Map<IEnumerable<CategoryDTO>>(Categories);

            foreach (var category in CategoriesDTO)
            {
                var brandCategoriesList = await BrandCategoryRepo.Get(c => c.CategoryID == category.CategoryID);

                if (category.BrandIDs == null)
                {
                    category.BrandIDs = new List<int>();
                }

                foreach (var BrandCategory in brandCategoriesList)
                {
                    category.BrandIDs.Add(BrandCategory.BrandID);
                }
            }

            if (Categories == null)
                return BadRequest();
            return Ok(CategoriesDTO);

        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Category = await CategoryRepo.GetById(id);
            var BrandCategory = await BrandCategoryRepo.Get(x => x.CategoryID == id);
            if (Category == null)
                return NotFound("Category not found");
            foreach (var brandcat in BrandCategory)
            {
                await BrandCategoryRepo.Delete(brandcat);
            }
            try
            {
                await CategoryRepo.Delete(Category);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }


}
