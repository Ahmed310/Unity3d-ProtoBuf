# Unity3d-ProtoBuf
Protobuf Interigation in unity3d + Editor window to generate C# source code from protobuf files


## How To Use
* Place proto-buf folder at root level in asset folder
* Open ProtoBuf Editor Window Tools->ProtoBuf
* Set field in Editor window and you are good to go, click Generate C# source code from Proto code button


set field in Editor Window
![image](https://github.com/Ahmed310/Unity3d-ProtoBuf/blob/master/screenshots/screen1.PNG)

Drag proto files in green area
![image](https://github.com/Ahmed310/Unity3d-ProtoBuf/blob/master/screenshots/screen3.PNG)

![image](https://github.com/Ahmed310/Unity3d-ProtoBuf/blob/master/screenshots/screen2.PNG)




## Example


### ProtoBuf code

#### Model.proto

```csharp
message Card {
    required string id = 1;
}


message Player {
    required string id = 1;
    required int32 health = 2;
	repeated Item items = 4; 
}


message Item {
    required string item_id = 1;    
    required string name = 3;
    required string description = 4;
    optional string catalog_name = 5;
    optional string catalog_image_url = 6;
    optional string item_image_url = 7;
    required double price = 9;

}
```

#### client_messages.proto

```csharp
import "models.proto";

message ClientMessage {
    

    required ClientMessageType type = 22;

    optional RegisterUserRequest register_user_request = 2;
    optional LoginUserRequest login_user_request = 3;
    optional AttackRequest attack_request = 4;

}

enum ClientMessageType {
    REGISTER_USER_REQUEST = 2;
    LOGIN_USER_REQUEST = 3;
	ATTACK_REQUEST = 4;
   
}
message RegisterUserRequest {
    required string user_name = 1;
    required string hashed_password = 2;
    required string first_name = 3;
    required string last_name = 4;
    required string avatar_id = 5;
    required string device_id = 6;
    required int32 device_type = 7;
    required string google_id = 8;
}


message LoginUserRequest {
    required string user_name = 1;
    required string hashed_password = 2;
}



message AttackRequest {
   
    required int32 damage = 1;
    required string target_id = 2;
}



```


#### server_messages.proto

```csharp
import "models.proto";


message ServerMessage {
   
    optional ServerMessageType type = 24;

    //responses
    optional RegisterUserResponse register_user_response = 1;
    optional LoginUserResponse login_user_response = 2;
    optional AttackResponse attack_response = 3;
   

}

enum ServerMessageType {
    REGISTER_USER_RESPONSE = 1;
    LOGIN_USER_RESPONSE = 2;
    ATTACK_RESPONSE = 3;
    
}

message RegisterUserResponse {
    required int32 metadata_version = 1;
    optional string user_name = 2;
    optional string password = 3;

}

message LoginUserResponse {
    required int32 metadata_version = 1;
    required int32 game_version = 2;
}



message AttackResponse {
    required string player_id = 1;
    required string target_id = 2;
    required float target_hp_after_attack = 3;
}


```

### Unity3d Source Code
#### Serialize & De-serialize login Request message

```csharp
  private void LoginRequest()
    {

        // client side code
        var msg = ClientMessage.CreateBuilder().LoginUserRequest.ToBuilder()
                                    .SetUserName("test user")
                                    .SetHashedPassword("123456789")
                                    .Build();

        ClientMessage builder = ClientMessage.CreateBuilder()
            .SetLoginUserRequest(msg)
            .SetType(ClientMessageType.LOGIN_USER_REQUEST)
            .Build();

        // now login msg is ready to send over network to server (seralize)
        var loginBytes = builder.ToByteArray();

        // deseralize
        var loginObject = ClientMessage.ParseFrom(loginBytes);

        Debug.Log(loginObject.Type);
        Debug.Log(loginObject.LoginUserRequest.UserName);
    }
```

#### Serialize & De-serialize login response message

```csharp
   private void LoginResponse()
    {

        var msg = ServerMessage.CreateBuilder().LoginUserResponse.ToBuilder()
            .SetMetadataVersion(1)
            .SetGameVersion(1)
            .Build();

        ServerMessage loginResponseBuilder = ServerMessage.CreateBuilder()
           .SetLoginUserResponse(msg)
           .SetType(ServerMessageType.LOGIN_USER_RESPONSE)
           .Build();



        // seralize
        var loginRes = loginResponseBuilder.ToByteArray();

        // de-seralize
        var serverMsgReceived = ServerMessage.ParseFrom(loginRes);

        Debug.Log(serverMsgReceived.Type);
        Debug.Log(serverMsgReceived.LoginUserResponse.GameVersion);
    }
```