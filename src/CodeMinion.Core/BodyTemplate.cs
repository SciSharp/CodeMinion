using System.Text;
using CodeMinion.Core.Models;

namespace CodeMinion.Core
{

    public abstract class BodyTemplate
    {
        public abstract void GenerateBody(Declaration decl, StringBuilder s);
    }
}
