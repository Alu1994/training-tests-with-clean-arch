using TrainingTDDWithCleanArch.Application.Inputs;
using TrainingTDDWithCleanArch.Application.UseCases;

namespace TrainingTDDWithCleanArch.Presentation.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductUseCases _productUseCases;

        public ProductController(ILogger<ProductController> logger, IProductUseCases productUseCases)
        {
            _logger = logger;
            _productUseCases = productUseCases;
        }

        [HttpGet(Name = "GetAllProducts")]
        public async Task<IResult> GetAll(CancellationToken cancellation)
        {
            const string errorMessage = "Error while getting all products.";

            var result = await _productUseCases.GetProducts(cancellation);

            return result.Match(x =>
                Results.Ok(x),
                x => Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorMessage,
                    detail: x.ToSeq().Head.Message,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [HttpGet("{productId:Guid}", Name = "GetProductById")]
        public async Task<IResult> Get(Guid productId, CancellationToken cancellation)
        {
            const string errorMessage = "Error while getting product by id.";

            var result = await _productUseCases.GetProductById(productId, cancellation);

            return result.Match(x => 
                Results.Ok(x),
                x => Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorMessage,
                    detail: x.ToSeq().Head.Message,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [HttpGet("by-names/{productName}", Name = "GetProductByName")]
        public async Task<IResult> Get(string productName, CancellationToken cancellation)
        {
            const string errorMessage = "Error while getting product by name.";

            var result = await _productUseCases.GetProductByName(productName, cancellation);

            return result.Match(x =>
                Results.Ok(x),
                x => Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorMessage,
                    detail: x.ToSeq().Head.Message,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }

        [HttpPost(Name = "CreateProduct")]
        public async Task<IResult> Post(CreateProductInput product, CancellationToken cancellation)
        {
            const string errorMessage = "Error while creating new product.";

            var result = await _productUseCases.CreateProduct(product, cancellation);

            return result.Match(x =>
                Results.Ok(x),
                x => Results.Problem(
                    type: HttpStatusCode.BadRequest.ToString(),
                    title: errorMessage,
                    detail: x.ToSeq().Head.Message,
                    statusCode: StatusCodes.Status400BadRequest
                )
            );
        }
    }
}
