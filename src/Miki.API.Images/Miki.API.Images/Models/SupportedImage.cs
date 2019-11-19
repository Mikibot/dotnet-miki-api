namespace Miki.API.Images.Models
{
    internal struct SupportedImage
    {
        public bool Supported { get; private set; }
        public string Prefix { get; private set; }

        public SupportedImage(bool suported, string prefix)
        {
            this.Supported = suported;
            this.Prefix = prefix;
        }
    }
}
