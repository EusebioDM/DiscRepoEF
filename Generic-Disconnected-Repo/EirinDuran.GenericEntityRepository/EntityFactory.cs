using System;
using System.Collections.Generic;
using System.Text;

namespace EirinDuran.GenericEntityRepository
{
    public class EntityFactory<Entity>
    {
        private Func<Entity> createEntity;

        public EntityFactory(Func<Entity> createEntity)
        {
            this.createEntity = createEntity;
        }

        public Entity CreateEmptyEntity() => createEntity.Invoke();
    }
}
