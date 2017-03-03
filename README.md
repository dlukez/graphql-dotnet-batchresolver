GraphQL.BatchResolver
=====================

Resolve your collections in batches and prevent multiple round-trips to the DB.

[![NuGet](https://img.shields.io/nuget/v/GraphQL.BatchResolver.svg)](https://nuget.org/packages/GraphQL.BatchResolver)
[![MyGet Pre Release](https://img.shields.io/myget/dlukez/vpre/GraphQL.BatchResolver.svg?label=myget)](https://www.myget.org/feed/dlukez/package/nuget/GraphQL.BatchResolver)
[![MyGet Build Status](https://www.myget.org/BuildSource/Badge/dlukez?identifier=265cd302-0184-43af-abc8-6041143cfc91)](https://www.myget.org/feed/dlukez/package/nuget/GraphQL.BatchResolver)

API
---

```csharp
Field<ListGraphType<DroidType>>()
    .Batch(d => d.DroidId)
    .Resolve(ctx =>
    {
        var ids = ctx.Source; // Source contains the keys collection
        return db.Droids.Where(d => ids.Contains(d.DroidId)).ToListAsync();
    });
```
