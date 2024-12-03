using AutoMapper;
using Buss.Layer.DTOs;
using DataLayer.Entities;
using DataLayer.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
namespace application.pl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IRepository<Brand> BrandRepo;
        private readonly IRepository<BrandCategory> BrandCategoryRepo;
        private readonly IMapper Mapper;

        public BrandController(IRepository<Brand> brandRepo, IMapper mapper, IRepository<BrandCategory> brandCategoryRepo)
        {
            BrandRepo = brandRepo;
            Mapper = mapper;
            BrandCategoryRepo = brandCategoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Brands = await BrandRepo.GetAll();
            var BrandsDTO = Mapper.Map<IEnumerable<BrandDTO>>(Brands);

            foreach (var brand in BrandsDTO)
            {
                var brandCategoriesList = await BrandCategoryRepo.Get(c => c.BrandID == brand.BrandID);

                if (brand.CategoryIDs == null)
                {
                    brand.CategoryIDs = new List<int>();
                }

                foreach (var BrandCategory in brandCategoriesList)
                {
                    brand.CategoryIDs.Add(BrandCategory.CategoryID);
                }
            }

            if (Brands == null)
                return BadRequest();
            return Ok(BrandsDTO);
        }

        //Pagenation
        [HttpGet("{pagenumber:int}/{pagesize:int}")]
        public async Task<IActionResult> GetAll(int pagenumber, int pagesize)
        {
            var Brands = await BrandRepo.GetAll(pagenumber, pagesize);
            var BrandsDTO = Mapper.Map<IEnumerable<BrandDTO>>(Brands);
            var count = BrandRepo.GetCount();


            foreach (var brand in BrandsDTO)
            {
                var brandCategoriesList = await BrandCategoryRepo.Get(c => c.BrandID == brand.BrandID);

                if (brand.CategoryIDs == null)
                {
                    brand.CategoryIDs = new List<int>();
                }

                foreach (var BrandCategory in brandCategoriesList)
                {
                    brand.CategoryIDs.Add(BrandCategory.CategoryID);
                }
            }

            if (Brands == null)
                return BadRequest();
            return Ok(new { Count = count, Brands = BrandsDTO });

        }

        [HttpGet("{id:int}", Name = "GetBrandByIdRoute")]
        public async Task<IActionResult> GetById(int id)
        {
            var Brand = await BrandRepo.GetById(id);
            var BrandDTO = Mapper.Map<BrandDTO>(Brand);


            var brandCategoriesList = await BrandCategoryRepo.Get(c => c.BrandID == BrandDTO.BrandID);

            if (BrandDTO.CategoryIDs == null)
            {
                BrandDTO.CategoryIDs = new List<int>();
            }

            foreach (var BrandCategory in brandCategoriesList)
            {
                BrandDTO.CategoryIDs.Add(BrandCategory.CategoryID);
            }

            if (Brand == null)
                return NotFound("Brand not found");
            return Ok(BrandDTO);
        }


        [HttpGet("{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var Brands = await BrandRepo.Get(x => x.BrandName == name);
            var BrandsDTO = Mapper.Map<IEnumerable<BrandDTO>>(Brands);

            foreach (var brand in BrandsDTO)
            {
                var brandCategoriesList = await BrandCategoryRepo.Get(c => c.BrandID == brand.BrandID);

                if (brand.CategoryIDs == null)
                {
                    brand.CategoryIDs = new List<int>();
                }

                foreach (var BrandCategory in brandCategoriesList)
                {
                    brand.CategoryIDs.Add(BrandCategory.CategoryID);
                }
            }

            if (Brands == null)
                return BadRequest();
            return Ok(BrandsDTO);

        }


        [HttpPost("/api/Brand/Upload")]
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
        public async Task<IActionResult> Add(BrandDTO brandDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Brand brand = Mapper.Map<Brand>(brandDTO);
                    await BrandRepo.Insert(brand);

                    if (brandDTO.CategoryIDs != null)
                    {
                        foreach (var categoryId in brandDTO.CategoryIDs)
                        {
                            var BrandCategory = new BrandCategory() { BrandID = brand.BrandID , CategoryID = categoryId };
                            await BrandCategoryRepo.Insert(BrandCategory);
                        }
                    }

                    string url = Url.Link("GetBrandByIdRoute", new { id = brand.BrandID })!;
                    return Created(url, brand);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        public async Task<IActionResult> Update(BrandDTO brandDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingBrand = await BrandRepo.GetById(brandDTO.BrandID);

           

            if (existingBrand == null)
            {
                return NotFound("Brand not found");
            }
            
            Mapper.Map(brandDTO, existingBrand);

            try
            {
                await BrandRepo.Update(existingBrand);
                return NoContent();
            }



            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Brand = await BrandRepo.GetById(id);
            var BrandCategory = await BrandCategoryRepo.Get(x=>x.BrandID == id);
            if (Brand == null)
                return NotFound("Brand not found");

            foreach(var brandcat in BrandCategory)
            {
                await BrandCategoryRepo.Delete(brandcat);
            }

            try
            {
                //await BrandCategoryRepo.Delete(BrandCategory);
                await BrandRepo.Delete(Brand);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }


}
