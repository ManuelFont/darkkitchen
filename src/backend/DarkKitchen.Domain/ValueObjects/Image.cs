namespace DarkKitchen.Domain.ValueObjects;

public sealed record Image
{
    public string Url { get; }
    public int Position { get; }

    public Image(string url, int position)
    {
        ValidateUrl(url);
        ValidatePosition(position);

        Url = url;
        Position = position;
    }

    private static void ValidateUrl(string url)
    {
        if(string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Image URL cannot be null or whitespace.");
        }
    }

    private static void ValidatePosition(int position)
    {
        if(position is < 0 or > 2)
        {
            throw new ArgumentException("Image position must be between zero and two.");
        }
    }
}
