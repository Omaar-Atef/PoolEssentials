using DataLayer.Entities;
using DataLayer.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Buss.Layer.DTOs;
namespace application.pl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> ProductRepo;
        private readonly IMapper Mapper;

        public ProductController(IRepository<Product> productRepo, IMapper mapper)
        {
            ProductRepo = productRepo;
            Mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await ProductRepo.GetAll();
            var productsDTO = Mapper.Map<IEnumerable<ProductDTO>>(products);

            if (products == null)
                return BadRequest();
            return Ok(productsDTO);
        }


        //Pagenation
        [HttpGet("{pagenumber:int}/{pagesize:int}")]
        public async Task<IActionResult> GetAll(int pagenumber, int pagesize)
        {
            var Products = await ProductRepo.GetAll(pagenumber, pagesize);
            var count = ProductRepo.GetCount();
            var productsDTO = Mapper.Map<IEnumerable<ProductDTO>>(Products);

            if (Products == null)
                return BadRequest();

            return Ok(new { Count = count, Products = productsDTO });
        }

        [HttpGet("{brandid:int}/{categoryid}/{pagenumber:int}/{pagesize:int}")]
        public async Task<IActionResult> GetAllBrandCategoriesProduct(int brandid,int categoryid ,int pagenumber, int pagesize)
        {
            var AllProducts = await ProductRepo.Get(x => x.BrandId == brandid && x.CategoryId == categoryid);
            var BrandCategoriesProduct = AllProducts.Skip((pagenumber - 1) * pagesize).Take(pagesize);
            
            var ProductsDTO = Mapper.Map<IEnumerable<ProductDTO>>(BrandCategoriesProduct);

            var count = AllProducts.Count();

            if (BrandCategoriesProduct == null)
                return BadRequest();
            return Ok(new { Count = count, Products = ProductsDTO });

        }

        [HttpGet("{categoryid:int}/{pagenumber:int}/{pagesize:int}")]
        public async Task<IActionResult> GetAllProductsInCategory(int categoryid, int pagenumber, int pagesize)
        {
            var AllProductsInCategory = await ProductRepo.Get(x => x.CategoryId == categoryid);
            var ProductsInCategory = AllProductsInCategory.Skip((pagenumber - 1) * pagesize).Take(pagesize);
           
            var PrdDTO = Mapper.Map<IEnumerable<ProductDTO>>(ProductsInCategory);
            var count = AllProductsInCategory.Count();

            if (ProductsInCategory == null)
                return BadRequest();
            return Ok(new { Count = count, Products = PrdDTO });

        }

        [HttpGet("{id:int}", Name = "GetByIdRoute")]
        public async Task<IActionResult> GetById(int id)
        {
            var Product = await ProductRepo.GetById(id);
            var productDTO = Mapper.Map<ProductDTO>(Product);

            if (Product == null)
                return NotFound("Product not found");
            return Ok(productDTO);
        }


        [HttpPost("/api/Product/Upload")]
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
        public async Task<IActionResult> Add(ProductDTO productDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Product product = Mapper.Map<Product>(productDTO);
                    await ProductRepo.Insert(product);
                    string url = Url.Link("GetByIdRoute", new { id = product.ProductId })!;
                    return Created(url, product);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest(ModelState);
        }


        [HttpGet("{name}/GetByName")]
        public async Task<IActionResult> GetByName(string name)
        {
            var Products = await ProductRepo.Get(x => x.Name == name);
            var ProductsDTO = Mapper.Map<IEnumerable<ProductDTO>>(Products);


            if (Products == null)
                return BadRequest();
            return Ok(ProductsDTO);

        }

        [HttpGet("{productcode}", Name = "GetByProductCode")]
        public async Task<IActionResult> GetByProductCode(string productcode)
        {
            var Products = await ProductRepo.Get(x => x.ProductCode == productcode);
            var ProductsDTO = Mapper.Map<IEnumerable<ProductDTO>>(Products);


            if (Products == null)
                return BadRequest();
            return Ok(ProductsDTO);

        }


        [HttpPut]
        public async Task<IActionResult> Update(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await ProductRepo.GetById(productDTO.ProductId);

            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }

            Mapper.Map(productDTO, existingProduct);

            try
            {
                await ProductRepo.Update(existingProduct);
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
            var Product = await ProductRepo.GetById(id);
            if (Product == null)
                return NotFound("Product not found");

            try
            {
                await ProductRepo.Delete(Product);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }


}
