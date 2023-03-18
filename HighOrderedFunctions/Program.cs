// See https://aka.ms/new-console-template for more information


namespace HighOrderedFunctions;

[Serializable]
public sealed class FilterMovieException : Exception
{
    public FilterMovieException()
    {
    }

    public FilterMovieException(string message) : base(message)
    {
    }

    public FilterMovieException(string message, Exception inner) : base(message, inner)
    {
    }
}   

public static class Program
{
    public static void Main(string[] argc )
    {

        var reqYear = 1997;
        Func<IEnumerable<Movie>, Exception> abc = movies =>
        {
            return !movies.Any()
                ? new FilterMovieException($"List of movies neobsahuje zadny zaznam pro rok {reqYear}")
                : new FilterMovieException($"List of movies obsahuje duplicity pro rok {reqYear}");
        };
        
        var reqMovie = GetAllMovies().SingleElseException(movie => movie.Year == reqYear, abc);

        Console.WriteLine($"{reqMovie.Name}");
    }

    private static IEnumerable<Movie> GetAllMovies()
    {
        yield return new Movie("Star Wars 1", "SciFi", 1997);
        yield return new Movie("Star Wars 2", "SciFi", 1998);
        yield return new Movie("Star Wars 3", "SciFi", 1999);
        yield return new Movie("Star Wars 4", "SciFi", 1997);
        yield return new Movie("Star Wars 5", "SciFi", 1996);
        yield return new Movie("Star Wars 6", "SciFi", 1993);
    }

}

internal static class EnumerableExtensions
{
    public static T SingleElseException<T>(this IEnumerable<T> sequence, Func<T, bool> predicate, Func<IEnumerable<T>, Exception> exceptionFactory)
    {
        var found = sequence.Where(predicate).ToList();

        if (found.Count == 1)
            return found[0];

        throw exceptionFactory(found);
    }
}