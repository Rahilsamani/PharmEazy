using PharmEazy.Models;
using PharmEazy.Models.DTO;

namespace PharmEazy.DAL.Contacts
{
    public interface ICartItemRepository
    {
        public Task<(bool status, string message)> CreateCartItem(CartItem cart);
        public Task<List<CartItemDTO>> GetAllCartItems(string userId);
        public Task<(bool status, string message)> DeleteCartItem(int id);
        public Task<(bool status, string message)> BuyAllCartItems(List<CartItemDTO> cartItems, string userId);
    }
}
