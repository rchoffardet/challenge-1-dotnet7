if(args[0] == args[1])
{
    Console.WriteLine("0");
    return;
}

var peopleByName = File
    .ReadLines("./people.txt")
    .Select(x => {
        var parts = x.Split(':');
        return (id: int.Parse(parts[0]), name: parts[1]);
    })
    .Where(x => x.name == args[0] || x.name == args[1])
    .Take(2)
    .Select(x => x.id)
    .ToArray();

var id1 = peopleByName[0];
var id2 = peopleByName[1];

var friendship1 = File.ReadLines("./friendship.txt")
    .AsParallel()
    .Select(x => {
        var parts = x.Split(':');
        return (from: int.Parse(parts[0]), to: int.Parse(parts[1]));
    })
    .ToLookup(x => x.from, x => x.to);

var friendship2 = friendship1
    .AsParallel()
    .SelectMany(x => x.Select(y => (from: x.Key, to: y)))
    .ToLookup(x => x.to, x => x.from);

var visited = new HashSet<int>();
Console.WriteLine(FindRelation(id1, new[] {id2}, 1));

int FindRelation(int needle, IEnumerable<int> haystack, int degree)
{
    var relations = haystack
        .SelectMany(x => friendship1[x].Concat(friendship2[x]))
        .Where(x => !visited.Contains(x));
        
    var toVisit = new List<int>();

    foreach(var relation in relations)
    {
        visited.Add(relation);
        if(relation == needle)
        {
            return degree;
        }
        else
        {
            toVisit.Add(relation);
        }
    }

    if(toVisit.Count == 0)
    {
        return -1;
    }
    else
    {
        return FindRelation(needle, toVisit, degree + 1);
    }
}
