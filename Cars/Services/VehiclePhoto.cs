using Microsoft.AspNetCore.Http;

namespace AUTODOK.Services;

public static class VehiclePhoto
{
    public const int MaxBytes = 2 * 1024 * 1024;

    public static async Task<string?> ReadAsync(IFormFile? file, bool required)
    {
        if (file == null || file.Length == 0)
        {
            if (required) throw new ArgumentException("Choose a vehicle photo to upload.");
            return null;
        }
        if (file.Length > MaxBytes) throw new ArgumentException("The photo must be 2 MB or smaller.");

        using MemoryStream stream = new();
        await file.CopyToAsync(stream);
        byte[] bytes = stream.ToArray();
        if (bytes.Length == 0 || bytes.Length > MaxBytes)
            throw new ArgumentException("The photo must be 2 MB or smaller.");

        string? mime = DetectImageType(bytes);
        if (mime == null) throw new ArgumentException("Upload a PNG, JPEG, or WebP image.");
        return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
    }

    private static string? DetectImageType(byte[] bytes)
    {
        if (bytes.Length >= 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E &&
            bytes[3] == 0x47 && bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A)
            return "image/png";
        if (bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return "image/jpeg";
        if (bytes.Length >= 12 && bytes[0] == 'R' && bytes[1] == 'I' && bytes[2] == 'F' &&
            bytes[3] == 'F' && bytes[8] == 'W' && bytes[9] == 'E' && bytes[10] == 'B' && bytes[11] == 'P')
            return "image/webp";
        return null;
    }
}
