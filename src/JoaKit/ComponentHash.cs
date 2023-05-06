using Microsoft.Extensions.DependencyInjection;

namespace JoaKit;


public record struct ComponentHash(string? Key, int LineNumber, string FilePath);