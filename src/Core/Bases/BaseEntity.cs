using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Bases;

public abstract class BaseEntity : BaseEntity<Guid>
{
    protected BaseEntity() => (Id, CreatedAt) = (Guid.NewGuid(), DateTimeOffset.Now);
}

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    public DateTimeOffset CreatedAt { get; protected set; } = default;
}
