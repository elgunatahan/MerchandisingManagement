using Application.Commands.CreateCategory;

namespace Api.V1.Dtos
{
    public class CreateCategoryInput
    {
        public int MinStockQuantity { get; set; }
        
        public string Name { get; set; }

        internal CreateCategoryCommand ToCommand()
        {
            return new CreateCategoryCommand
            {
                Name = Name,
                MinStockQuantity = MinStockQuantity,
            };
        }
    }
}
