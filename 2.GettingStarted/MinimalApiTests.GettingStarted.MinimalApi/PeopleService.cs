namespace MinimalApiTests.GettingStarted.MinimalApi;

public class PeopleService
{
    private readonly Person[] _people = {
        new Person("Nick Crow"),
        new Person("Mick Bowmer"),
        new Person("Tim Cook")
    };

    public IEnumerable<Person> Search(string searchTerm)
    {
        return _people.Where(x => x.FullName.Contains(searchTerm, StringComparison.Ordinal));
    }
}