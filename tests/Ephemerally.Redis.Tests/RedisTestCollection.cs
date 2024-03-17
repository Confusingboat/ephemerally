using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ephemerally.Redis.Tests;

[CollectionDefinition(Name, DisableParallelization = true)]
public class RedisTestCollection
{
    public const string
        Name = "Redis";
}