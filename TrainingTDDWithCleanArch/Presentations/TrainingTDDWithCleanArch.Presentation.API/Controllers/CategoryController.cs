using TrainingTDDWithCleanArch.Application.Inputs;
using TrainingTDDWithCleanArch.Application.UseCases;

namespace TrainingTDDWithCleanArch.Presentation.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController(ILogger<CategoryController> logger, ICategoryUseCases categoryUseCases) : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger = logger;
        private readonly ICategoryUseCases _categoryUseCases = categoryUseCases;

        [HttpGet(Name = "GetAllCategories")]
        public async Task<IResult> GetAll(CancellationToken cancellation)
        {
            const string errorMessage = "Error while getting all categories.";

            var result = await _categoryUseCases.GetCategories(cancellation);
            
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

        [HttpGet("{categoryId:Guid}", Name = "GetCategoryById")]
        public async Task<IResult> Get(Guid categoryId, CancellationToken cancellation)
        {
            const string errorMessage = "Error while getting category by id.";

            var result = await _categoryUseCases.GetCategoryById(categoryId, cancellation);

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

        [HttpGet("by-name/{categoryName}", Name = "GetCategoryByName")]
        public async Task<IResult> Get(string categoryName, CancellationToken cancellation)
        {
            const string errorMessage = "Error while getting category by name.";

            var result = await _categoryUseCases.GetCategoryByName(categoryName, cancellation);

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

        [HttpPost(Name = "CreateCategory")]
        public async Task<IResult> Post(CreateCategoryInput category, CancellationToken cancellation)
        {
            const string errorMessage = "Error while creating new category.";

            var result = await _categoryUseCases.CreateCategory(category, cancellation);

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
