// See https://aka.ms/new-console-template for more information


using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

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
public sealed class AnotherFilterMovieException : Exception
{
    public AnotherFilterMovieException()
    {
    }

    public AnotherFilterMovieException(string message) : base(message)
    {
    }

    public AnotherFilterMovieException(string message, Exception inner) : base(message, inner)
    {
    }
}

[MemoryDiagnoser]
public class Program
{
    public static void Main()
    {

        // BenchmarkRunner.Run<Program>();

        var program = new Program();

        program.UsingClassMethod();

        // Using lambda
        program.UsingLambda();

        program.UsingLocalFunc();
        

    }

    [Benchmark]
    public void UsingClassMethod()
    {
        var reqYear = 1997;
        Movie? reqMovie;

        // Using local method
        try
        {
            reqMovie = GetAllMovies().SingleElseException(movie => movie.Year == reqYear, movies => FunctionCall(movies, reqYear));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    [Benchmark]
    public void UsingLambda()
    {
        var reqYear = 1997;
        Movie? reqMovie;

        // Using local method
        try
        {
            reqMovie = GetAllMovies().SingleElseException(movie => movie.Year == reqYear, movies =>
            {
                if (!movies.Any())
                {
                    return new FilterMovieException($"List of movies neobsahuje zadny zaznam pro rok {reqYear}");
                }
                else
                {
                    return new FilterMovieException($"List of movies obsahuje duplicity pro rok {reqYear}");
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    [Benchmark]
    public void UsingLocalFunc()
    {
        var reqYear = 1997;

        try
        {
            var reqMovie = GetAllMovies().SingleElseException(movie => movie.Year == reqYear, LocalFunc);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        // Declaration of local function !!!
        Exception LocalFunc(IEnumerable<Movie> movies)
        {
            if (!movies.Any())
            {
                return new AnotherFilterMovieException($"List of movies neobsahuje zadny zaznam pro rok {reqYear}");
            }
            else
            {
                return new AnotherFilterMovieException($"List of movies obsahuje duplicity pro rok {reqYear}");
            }
        }
    }

    private static Exception FunctionCall(IEnumerable<Movie> movies, int reqYear)
    {
        if (!movies.Any())
        {
            return new FilterMovieException($"List of movies neobsahuje zadny zaznam pro rok {reqYear}");
        }
        else
        {
            return new FilterMovieException($"List of movies obsahuje duplicity pro rok {reqYear}");
        }
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