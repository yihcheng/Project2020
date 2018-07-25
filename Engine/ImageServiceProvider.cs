using CommonContracts;
using ImageServiceProxy;

namespace Engine
{
    internal static class ImageServiceProvider
    {
        public static IImageService GetImageService()
        {
            // return an object which provide APIs of image service
            return new ImageService();
        }
    }
}
