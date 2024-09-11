# Multiplayer Tank Shooting Game

## Giới thiệu

Multiplayer Tank Shooting Game là một trò chơi đa người chơi, được phát triển với việc sử dụng giao thức UDP để truyền tải dữ liệu giữa các máy tính, trong đó các người chơi (tối đa 4 người) sẽ được chọn 1 trong 3 bản đồ, nơi họ sẽ điều khiển các xe tăng màu sắc riêng biệt. Mục tiêu của trò chơi là tiêu diệt tất cả đối thủ và trở thành người cuối cùng còn sống sót.

## Luật chơi

- Người chơi sẽ tham gia vào các phòng chơi, có sự tham gia từ 2 đến 4 người.
- Khi có đủ 2 người trở lên trong phòng, trò chơi có thể được bắt đầu.
- Người chơi được chọn 1 trong 3
- Các người chơi sẽ được đưa vào một bản đồ và họ sẽ sử dụng xe tăng của mình cùng các vật phẩm để tấn công và tiêu diệt đối thủ.
- Người cuối cùng còn lại trên bản đồ sẽ chiến thắng trò chơi.

## Công nghệ sử dụng

- Sử dụng Unity Engine (C#) cho việc phát triển trò chơi + Network For Gameobject (Multi Player).
- Sử dụng giao thức UDP để truyền tải dữ liệu giữa các máy tính (Plugins hỗ trợ)
  
- Các Gameobject có thể giao tiếp được trên mạng nhờ kế thừa NetworkMonobehaviour và được gắn thêm NetworkObject component.
  Như vậy nó sẽ được đánh dấu là 1 object trong mạng.
  
  ![image](https://github.com/jnp2018/g6_proj-297486569/assets/80816285/c4b3dd18-ba20-4369-8e1c-f4614a192e7d)

  ### Kiến trúc:
    - 1 Client sẽ cần có NetworkManager, hỗ trợ việc xử lý kết nối mạng và quản lý các Object trong mạng.
    - Hỗ trợ các phương thức để Start Host, Start Server, hoặc Start Client
    - Cung cấp các hàm kiểm tra trong mạng như: IsServer, IsClient, IsHost, IsOwner LocalNetworkId, ...
    - Các Object được đánh dấu là nằm trong mạng, sẽ được quản lý bởi NetworkManager, sử dụng các hàm hỗ trợ bên trên để xử lý logic
    - Muốn spawn 1 Object tới tất cả các Client, Object đó được Spawn trên Server và sẽ tạo các Instance của Object đó trên tất cả các Client:

      VD: có 2 Player là Tank join vào game
      Nghĩa là trên cả 2 máy client đều có 2 Object Tank, 1 Tank chính là của Client đó (Owner), 1 Tank sẽ là Instance của Client còn lại.

(url)![image](https://github.com/jnp2018/g6_proj-297486569/assets/80816285/0820e4b1-6fee-450b-a968-5eeb8b451f43)

### Giao tiếp:
- Client và Server có thể giao tiếp với nhau qua các RPC (Remote Procedure Call)

- ServerRpc là một phương thức được gọi từ một NetworkObject, nó được đưa vào hàng đợi và chờ tới cuối frame để gửi đi
  Ngay khi nhận được ServerRpc, nó được xử lý tại NetworkObject đó tại Server.
  
<img width="657" alt="Screenshot 2023-11-23 at 09 37 18" src="https://github.com/jnp2018/g6_proj-297486569/assets/83466578/bfdbe962-9197-415c-b49c-075d6b3e0f4c">

  - Cú pháp của 1 hàm ServerRpc
<img width="562" alt="Screenshot 2023-11-23 at 09 51 07" src="https://github.com/jnp2018/g6_proj-297486569/assets/83466578/b21a96b6-80e1-4c2a-bb5b-a623d7b24e9d">

- Mặc định của ServerRpc là được gọi từ Owner (tức máy Cient sở hữu NetworkObject gọi hàm), nhưng cũng có thể set RequireOwnership = false để có thể gọi được hàm tự do hơn.
  <img width="697" alt="Screenshot 2023-11-23 at 09 53 37" src="https://github.com/jnp2018/g6_proj-297486569/assets/83466578/a369d269-cb49-4527-a091-4d901c332afa">

- ClientRpc là một phương thức được gọi từ server, và xử lý trên Instance của NetworkObject đó trên tất cả các Client.
  
<img width="675" alt="Screenshot 2023-11-23 at 09 57 37" src="https://github.com/jnp2018/g6_proj-297486569/assets/83466578/1c51401f-4c6d-40f1-b19e-553fa64be7c1">

- ClientRpc và ServerRpc được dùng để xử lý logic mạng tương đối nhiều trong game.
  VD đơn giản là: khi một Client A ấn nút bắn, nó gọi ServerRpc để request bắn đạn, sau đó Server lại gọi ClientRpc để xử lý việc bắn ra viên đạn từ xe tăng của client A trên tất cả các máy A, B, C...

 
## Preview
  
![Image Preview](/demo-image/demo.jpg)

### 1. Kết nối

![image](https://github.com/jnp2018/g6_proj-297486569/assets/80816285/cd0f1679-16c7-4161-b981-577b42cc48b8)

![image](https://github.com/jnp2018/g6_proj-297486569/assets/80816285/75a167c4-d405-43fe-ad26-8268b9176982)

![image](https://github.com/jnp2018/g6_proj-297486569/assets/80816285/e7ebf92a-a6ad-4978-8586-2fd21924728e)

### 2. Gameplay

![image](https://github.com/jnp2018/g6_proj-297486569/assets/80816285/8187d250-0c97-4e44-93d6-127e4fb6d932)

![image](https://github.com/jnp2018/g6_proj-297486569/assets/80816285/3308cab2-7e44-4111-8a91-19bc2f286481)

![image](https://github.com/jnp2018/g6_proj-297486569/assets/80816285/e37bb658-77da-4bb8-a692-10bf82bc9d85)

![image](https://github.com/jnp2018/g6_proj-297486569/assets/80816285/c5b635af-8601-41bd-8e5a-29f9c2722014)

## Hướng dẫn cài đặt

1. Tải tệp APK và cài đặt trò chơi lên thiết bị của bạn.
2. Tham gia vào một phòng chơi.
3. Nhập địa chỉ IP của máy chủ để tham gia sảnh chờ.
4. Chờ đợi đủ số lượng người chơi và bắt đầu trò chơi.

<!-- ## Thành viên nhóm

<div style="display: flex; flex-direction: column; gap: 10px">
    <div style="display: flex; gap: 10px; align-items: center">
    <div style="width: 40px; height: 40px; overflow: hidden; border-radius: 50%;">
        <img src="https://avatars.githubusercontent.com/u/80816285?s=80&v=4" alt="Ảnh cá nhân">
    </div>
    <a href="https://github.com/NgTienHungg" target="">
        [Nguyễn Tiến Hùng]
    </a>
    </div>
    <div style="display: flex; gap: 10px; align-items: center">
        <div style="width: 40px; height: 40px; overflow: hidden; border-radius: 50%;">
            <img src="https://avatars.githubusercontent.com/u/83466578?s=80&v=4" alt="Ảnh cá nhân">
        </div>
        <a href="https://github.com/nhikiu" target="">
            [Đỗ Ngọc Nhi]
        </a>
    </div>
    <div style="display: flex; gap: 10px; align-items: center">
        <div style="width: 40px; height: 40px; overflow: hidden; border-radius: 50%;">
            <img src="https://avatars.githubusercontent.com/u/86521181?s=80&v=4" alt="Ảnh cá nhân">
        </div>
        <a href="https://github.com/quynhniee" target="">
            [Nguyễn Như Quỳnh]
        </a>
    </div>
</div> -->
