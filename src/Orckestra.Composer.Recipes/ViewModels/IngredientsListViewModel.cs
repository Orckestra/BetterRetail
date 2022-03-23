using Orckestra.Composer.Recipes.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orckestra.Composer.Recipes.ViewModels
{
    public class IngredientsListViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public bool HideTitle { get; set; }
        public List<IIngredient> Ingredients { get; set; }
    }
}
