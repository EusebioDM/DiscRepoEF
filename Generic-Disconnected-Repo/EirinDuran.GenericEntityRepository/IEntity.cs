using System;
using System.Collections.Generic;
using System.Text;

namespace EirinDuran.GenericEntityRepository
{
    public interface IEntity<Model>
    {
        void UpdateWith(Model model);

        Model ToModel();
    }
}
