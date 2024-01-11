
namespace TMWalks.API;

public class LocalImageRepository : IImageRepository
{
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly TMWalksDbContext dbContext;

    public LocalImageRepository(IWebHostEnvironment webHostEnvironment,
                                IHttpContextAccessor httpContextAccessor,
                                TMWalksDbContext dbContext)
    {
        this.webHostEnvironment = webHostEnvironment;
        this.httpContextAccessor = httpContextAccessor;
        this.dbContext = dbContext;
    }

    public async Task<Image> Upload(Image image)
    {
        var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath,
                                         "Images",
                                         $"{image.FileName}{image.FileExtension}");

        // Upload Image to Local Path
        using var stream = new FileStream(localFilePath, FileMode.Create);
        await image.File.CopyToAsync(stream);

        // https://localhosssst:1234/images/image.jpg

        var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtension}";

        image.FilePath = urlFilePath;

        // Add Image to the Images table
        await dbContext.AddAsync(image);
        await dbContext.SaveChangesAsync();

        return image;
    }
}
