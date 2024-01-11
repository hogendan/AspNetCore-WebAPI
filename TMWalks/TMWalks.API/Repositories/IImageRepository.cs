namespace TMWalks.API;

public interface IImageRepository
{
    Task<Image> Upload(Image image);
}
