using CloudinaryDotNet;

namespace SaveUp.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        // ✅ Generate transformed image URL
        public string GetExerciseImage(string publicId)
        {
            return _cloudinary.Api.UrlImgUp
                /*.Transform(new Transformation()
                    .Width(300)
                    .Height(300)
                    .Crop("fill")
                    .Quality("auto")
                    .FetchFormat("auto"))*/
                .Secure(true)
                .BuildUrl(publicId);
        }
    }
}
