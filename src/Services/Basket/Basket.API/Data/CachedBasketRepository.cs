using System.Security.Cryptography;

namespace Basket.API.Data;

/*
 * repository: Tham chiếu đến một kho dữ liệu giỏ hàng gốc (có thể là một cơ sở dữ liệu hoặc một nguồn dữ liệu nào đó).
 * cache: Tham chiếu đến một bộ nhớ đệm phân tán để lưu trữ tạm thời dữ liệu giỏ hàng nhằm tăng tốc độ truy xuất.
 * */

public class CachedBasketRepository
    (IBasketRepository repository, IDistributedCache cache)
    : IBasketRepository
{
    public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
    {
        var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);
        if (!string.IsNullOrEmpty(cachedBasket))
            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;

        var basket = await repository.GetBasket(userName, cancellationToken);
        await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket), cancellationToken);
        return basket;
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        await repository.StoreBasket(basket, cancellationToken);

        await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket), cancellationToken);

        return basket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        await repository.DeleteBasket(userName, cancellationToken);

        await cache.RemoveAsync(userName, cancellationToken);

        return true;
    }
}

/* Constructor
* sử dụng hai dịch vụ: một dịch vụ kho dữ liệu (repository) và một dịch vụ bộ nhớ đệm phân tán (distributed cache).
* Lớp này triển khai giao diện IBasketRepository, nghĩa là nó cung cấp các phương thức để làm việc với giỏ hàng (basket).
* */

/* GetBasket: Phương thức này truy xuất giỏ hàng của người dùng dựa trên userName.
Đầu tiên, nó kiểm tra bộ nhớ đệm để xem có dữ liệu giỏ hàng đã lưu hay không:
* Nếu có dữ liệu trong bộ nhớ đệm (cachedBasket không rỗng), nó sẽ giải mã (deserialize) dữ liệu đó và trả về giỏ hàng.
* Nếu không có dữ liệu trong bộ nhớ đệm, nó truy xuất giỏ hàng từ kho dữ liệu gốc (repository.GetBasket).
Sau đó, nó lưu trữ giỏ hàng vào bộ nhớ đệm để sử dụng cho các lần truy xuất sau (cache.SetStringAsync).
Cuối cùng, giỏ hàng được trả về.
 * */

/* StoreBasket : Phương thức này lưu trữ giỏ hàng của người dùng.
* Đầu tiên, nó lưu giỏ hàng vào kho dữ liệu gốc (repository.StoreBasket).
* Sau đó, nó cập nhật giỏ hàng trong bộ nhớ đệm với dữ liệu mới (cache.SetStringAsync).
* Cuối cùng, nó trả về giỏ hàng đã lưu trữ.
 */

/* DeleteBasket: Phương thức này xóa giỏ hàng của người dùng dựa trên userName.
* Đầu tiên, nó xóa giỏ hàng từ kho dữ liệu gốc (repository.DeleteBasket).
* Sau đó, nó xóa giỏ hàng khỏi bộ nhớ đệm (cache.RemoveAsync).
* Cuối cùng, nó trả về true để chỉ báo rằng giỏ hàng đã được xóa thành công.
 * */