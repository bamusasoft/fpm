using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace FlopManager.Services.ViewModelInfrastructure
{
    public interface IEntityMapper<in TEntity>
    {
        void MapTo(TEntity entity, bool ignoreKey);
        void MapFrom(TEntity entity);
    }
}
