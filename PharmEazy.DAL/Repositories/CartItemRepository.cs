using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PharmEazy.DAL.Contacts;
using PharmEazy.DAL.Data;
using PharmEazy.Models;
using PharmEazy.Models.DTO;

namespace PharmEazy.DAL.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly string? cs;
        private readonly IInvoiceRepository _invoiceRepository;

        public CartItemRepository(ApplicationDbContext context, IConfiguration configuration, IInvoiceRepository invoiceRepository)
        {
            _context = context;
            cs = configuration.GetConnectionString("UserDbContextConnection");
            _invoiceRepository = invoiceRepository;
        }

        /// <summary>
        /// Creates Invoice Of All The Cart Items For The Specified User
        /// </summary>
        /// <param name="cartItems"></param>
        /// <param name="userId"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> BuyAllCartItems(List<CartItemDTO> cartItems, string userId)

        {
            try
            {
                // check availability of all the items
                foreach (var cartItem in cartItems)
                {
                    Stock? stock = await _context.Stock.FindAsync(cartItem.StockId);

                    if (stock?.Quantity - cartItem.Quantity < 0)
                    {
                        return (false, $"Only {stock?.Quantity} Items Of {cartItem.Name} having Expiry {cartItem.ExpiryDate.ToString("dd/MM/yyyy")} Available");
                    }
                }

                // Create Group Based on Seller Id
                var cartDic = cartItems.GroupBy(c => c.SellerId);

                List<Invoice> invoices = new();

                // Creates Invoices based on seller Id
                foreach (IGrouping<string, CartItemDTO> group in cartDic)
                {
                    List<InvoiceDetails> detailsList = new List<InvoiceDetails>() { };
                    double totalPrice = 0;

                    foreach (CartItemDTO cart in group)
                    {
                        InvoiceDetails invoiceDetails = new InvoiceDetails()
                        {
                            MedicineId = cart.MedicineId,
                            Price = cart.Price,
                            Quantity = cart.Quantity,
                            StockId = cart.StockId
                        };

                        totalPrice += cart.Price * cart.Quantity;
                        detailsList.Add(invoiceDetails);
                    }

                    Invoice invoice = new Invoice()
                    {
                        Userid = userId,
                        SellerId = group.Key,
                        TotalAmount = totalPrice,
                        InvoiceDetails = detailsList
                    };

                    invoices.Add(invoice);
                }

                bool result = await _invoiceRepository.CreateInvoice(invoices);

                if (!result)
                    return (false, "Something Went Wrong while Creating Invoice");

                // remove cart items after buying
                foreach (var cartItem in cartItems)
                {
                    await DeleteCartItem(cartItem.Id);
                }

                return (true, "All Cart Items Buyed Successfully");
            }
            catch (Exception ex)
            {
                return (false, "Something Went Wrong while Creating Invoice");
            }
        }


        /// <summary>
        /// Creates Cart Item
        /// </summary>
        /// <param name="cart"></param>
        /// <returns>Success Status And Message In JSON Format</returns>
        public async Task<(bool status, string message)> CreateCartItem(CartItem cart)
        {
            try
            {
                Stock? stock = await _context.Stock.FindAsync(cart.StockId);

                // check stock is avialble or not
                if (stock?.Quantity == 0)
                {
                    return (false, "Stock Item is not available");
                }
                // when user selects item more than available
                else if (cart.Quantity > stock?.Quantity)
                {
                    return (false, $"Only {stock.Quantity} Items are available");
                }

                // check user has already same stock medicine or not
                CartItem? existingCart = await _context.CartItem.FirstOrDefaultAsync(c => c.UserId == cart.UserId && c.StockId == cart.StockId);

                // when user dont have item inside the cart
                if (existingCart == null)
                {
                    await _context.CartItem.AddAsync(cart);
                }
                else
                {
                    // when user has already has available quantity in cart
                    if (existingCart.Quantity == stock?.Quantity)
                    {
                        return (false, "You Already have available items inside cart");
                    }
                    // when user has some inside and cart and some are available
                    else if (existingCart.Quantity + cart.Quantity > stock?.Quantity)
                    {
                        return (false, $"You can add only {stock.Quantity - existingCart.Quantity} Items inside cart as you already have");
                    }

                    existingCart.Quantity += cart.Quantity;

                    _context.CartItem.Update(existingCart);
                }

                await _context.SaveChangesAsync();
                return (true, "Item Added to Cart");
            }
            catch (Exception ex)
            {
                return (false, "Something went wrong while adding item in cart");
            }
        }

        /// <summary>
        /// Deletes Cart Item provided By User
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Success Status And Message In JSON Format</returns>>
        public async Task<(bool status, string message)> DeleteCartItem(int id)
        {
            try
            {
                CartItem? cart_Item = await _context.CartItem.FindAsync(id);

                if (cart_Item == null)
                    return (false, "Cart Item Not Found");

                _context.CartItem.Remove(cart_Item);
                await _context.SaveChangesAsync();
                return (true, "Cart Item Deleted Successfully");
            }
            catch (Exception ex)
            {
                return (false, "Something Went wrong while Deleting Cart Item");
            }
        }


        /// <summary>
        /// Gets All The Cart Items For The Speficied User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List Of Cart Items</returns>
        public async Task<List<CartItemDTO>> GetAllCartItems(string userId)
        {
            string sql = "spGetCartItems";
            List<CartItemDTO> cartItems = new List<CartItemDTO>() { };

            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    cartItems = (await connection.QueryAsync<CartItemDTO>(sql, new { userId }, commandType: System.Data.CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
            }

            return cartItems;
        }
    }
}
